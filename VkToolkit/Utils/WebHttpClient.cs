using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VkToolkit.Utils
{
    public enum MediaType
    {
        Xml,
        Json
    }

    public class WebHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly MediaTypeWithQualityHeaderValue _mediaTypeWithQualityHeaderValue;
        private readonly MediaTypeFormatter _mediaTypeFormatter;

        private static readonly MediaTypeFormatter JsonMediaTypeFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    IgnoreSerializableAttribute = true
                },
                TypeNameHandling = TypeNameHandling.Objects,
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Ignore
            }
        };

        private static readonly XmlMediaTypeFormatter XmlMediaTypeFormatter = new XmlMediaTypeFormatter
        {
            UseXmlSerializer = true
        };

        private readonly MediaTypeFormatter[] _mediaTypeFormatters;

        public WebHttpClient(MediaType mediaType, HttpClient webHttpClient = null)
            : this(mediaType == MediaType.Xml ? "application/xml" : "application/json"
            , mediaType == MediaType.Xml ? XmlMediaTypeFormatter : JsonMediaTypeFormatter
            , new[] { JsonMediaTypeFormatter, XmlMediaTypeFormatter }
            , webHttpClient)
        {
            MediaType = mediaType;
        }

        public MediaType MediaType
        {
            get;
            private set;
        }

        private WebHttpClient(string mediaTypeValue
            , MediaTypeFormatter mediaTypeFormatter
            , MediaTypeFormatter[] mediaTypeFormatters
            , HttpClient webHttpClient = null)
        {
            _httpClient = webHttpClient ?? WebRequestFactory.CreateHttpClient();

            _mediaTypeWithQualityHeaderValue = new MediaTypeWithQualityHeaderValue(mediaTypeValue);
            _mediaTypeFormatters = mediaTypeFormatters;
            _mediaTypeFormatter = mediaTypeFormatter;
        }

        private Error GetError(HttpResponseMessage response)
        {
            if (response.Content == null || !(response.Content.Headers.ContentLength > 0))
                return null;

            if (response.Content.Headers.ContentType.MediaType.Contains("text/html"))
                return null;

            if (response.Content.Headers.ContentType.MediaType.Contains("text/plain"))
            {
                return new Error
                {
                    Message = response.Content.ReadAsStringAsync().Result
                };
            }

            if (response.Content.Headers.ContentType.MediaType.Contains("json"))
            {
                Error result = null;

                using (var stream = response.Content.ReadAsStreamAsync().Result)
                {
                    stream.Position = 0;
                    var container =
                        JsonMediaTypeFormatter.ReadFromStreamAsync(typeof (ErrorContainer), stream, response.Content, null).Result as ErrorContainer;
                    result = container == null ? null : container.Error;
                }

                return result;
            }

            throw new NotSupportedException("Not supported error response type.");
        }

        private static IEnumerable<string> GetVaryHeader(HttpResponseMessage response)
        {
            return response.Headers.Vary;
        }

        private T GetResourceFromContent<T>(HttpResponseMessage response) where T : class
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            if (response.Content.Headers.ContentType.MediaType.Contains("json"))
            {
                var data = response.Content.ReadAsAsync<JsonResponse<T>>(_mediaTypeFormatters).Result;

                return data.Response;
            }

            T resource;

            using (var responseStream = response.Content.ReadAsStreamAsync().Result)
            {
                var reader = XmlReader.Create(responseStream);
                resource = ReadAndDeserializeNode<T>(reader);
            }

            return resource;
        }

        private T ReadAndDeserializeNode<T>(XmlReader reader)
        {
            do
            {
            } while (reader.Read() && reader.NodeType != XmlNodeType.Element);

            var nodeXml = reader.ReadSubtree();

            var serializer = GetElementXmlSerializer<T>();
            var node = (T)serializer.Deserialize(nodeXml);

            return node;
        }

        private XmlSerializer GetElementXmlSerializer<T>()
        {
            return new XmlSerializer(typeof(T));
        }

        public Response<Stream> GetContentStream(string url)
        {
            var request = WebRequestFactory.CreateGetRequest(url, _mediaTypeWithQualityHeaderValue);

            return ReadResourceResponse<Stream>(request);
        }

        public Response<string> GetContentAsString(string url)
        {
            var request = WebRequestFactory.CreateGetRequest(url, _mediaTypeWithQualityHeaderValue);

            return ReadResourceResponse<string>(request);
        }

        public Response<T> Get<T>(string url) where T : class
        {
            var request = WebRequestFactory.CreateGetRequest(url, _mediaTypeWithQualityHeaderValue);

            return ReadResourceResponse<T>(request);
        }

        public Stream GetStream(string url)
        {
            var request = WebRequestFactory.CreateGetRequest(url, _mediaTypeWithQualityHeaderValue);

            return ReadStreamResponse(request);
        }

        public long GetResponseLength(string url)
        {
            HttpResponseMessage response = null;

            var factory = new TaskFactory(TaskScheduler.Default);
            factory.StartNew(() => response = _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result).Wait();

            //Task.Factory
            //    .StartNew(() => response = _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result,
            //              CancellationToken.None,
            //              TaskCreationOptions.LongRunning, // guarantees separate thread
            //              TaskScheduler.Default)
            //    .Wait();

            return response.Content.Headers.ContentLength.GetValueOrDefault();
        }

        public Response<T> Put<T>(string url, T requestContent) where T : class
        {
            var request = WebRequestFactory.CreatePutRequest(url, _mediaTypeWithQualityHeaderValue);

            request.Content = GetRequestObjectContent(requestContent);

            return ReadResourceResponse<T>(request);
        }

        public ResponseBase Put(string url, object requestContent = null)
        {
            var request = WebRequestFactory.CreatePutRequest(url, _mediaTypeWithQualityHeaderValue);

            if (requestContent != null)
                request.Content = GetRequestObjectContent(requestContent);

            return ReadResourceResponse(request);
        }

        private ObjectContent GetRequestObjectContent(object requestContent)
        {
            return new ObjectContent(requestContent.GetType(), requestContent, _mediaTypeFormatter, _mediaTypeWithQualityHeaderValue);
        }

        public Response<T> Put<T>(string url, object requestContent = null) where T : class
        {
            var request = WebRequestFactory.CreatePutRequest(url, _mediaTypeWithQualityHeaderValue);

            if (requestContent != null)
                request.Content = GetRequestObjectContent(requestContent);

            return ReadResourceResponse<T>(request);
        }

        public Response<T> Post<T>(string url, object requestContent) where T : class
        {
            var request = WebRequestFactory.CreatePostRequest(url, _mediaTypeWithQualityHeaderValue);

            if (requestContent != null)
                request.Content = GetRequestObjectContent(requestContent);

            return ReadResourceResponse<T>(request);
        }

        public ResponseBase Post(string url, object requestContent = null)
        {
            var request = WebRequestFactory.CreatePostRequest(url, _mediaTypeWithQualityHeaderValue);

            if (requestContent != null)
                request.Content = GetRequestObjectContent(requestContent);

            return ReadResourceResponse(request);
        }

        public ResponseBase Post(string url, Stream stream, MediaTypeHeaderValue contentType)
        {
            var request = WebRequestFactory.CreatePostRequest(url, _mediaTypeWithQualityHeaderValue, stream, contentType);

            return ReadResourceResponse(request);
        }

        public Response<T> Post<T>(string url, Stream stream, MediaTypeHeaderValue contentType) where T : class
        {
            var request = WebRequestFactory.CreatePostRequest(url, _mediaTypeWithQualityHeaderValue, stream, contentType);

            return ReadResourceResponse<T>(request);
        }

        public ResponseBase Delete(string url)
        {
            var request = WebRequestFactory.CreateDeleteRequest(url, _mediaTypeWithQualityHeaderValue);

            return ReadResourceResponse<string>(request);
        }

        private Response<T> ReadResourceResponse<T>(HttpRequestMessage request) where T : class
        {
            try
            {
                var response = _httpClient.SendAsync(request).Result;

                if (response.StatusCode >= HttpStatusCode.InternalServerError)
                    return new Response<T>(response.RequestMessage.RequestUri.ToString(), response.StatusCode, response.ReasonPhrase);

                if (response.StatusCode >= HttpStatusCode.BadRequest)
                    return new Response<T>(response.RequestMessage.RequestUri.ToString(), response.StatusCode, response.ReasonPhrase);

                T resource = default(T);

                if (typeof(T) == typeof(Stream))
                    resource = response.Content.ReadAsStreamAsync().Result as T;
                else if (typeof(T) == typeof(string))
                    resource = response.Content.ReadAsStringAsync().Result as T;
                else
                    resource = GetResourceFromContent<T>(response);

                return new Response<T>(
                    response.RequestMessage.RequestUri.ToString(),
                    response.StatusCode, response.ReasonPhrase,
                    GetVaryHeader(response),
                    resource, GetError(response));
            }
            catch (AggregateException e)
            {
                return new Response<T>(request.RequestUri.ToString());
            }
        }

        private ResponseBase ReadResourceResponse(HttpRequestMessage request)
        {
            try
            {
                var response = _httpClient.SendAsync(request).Result;

                if (response.StatusCode >= HttpStatusCode.InternalServerError)
                    return new ResponseBase(response.RequestMessage.RequestUri.ToString(), response.StatusCode, response.ReasonPhrase);

                if (response.StatusCode >= HttpStatusCode.BadRequest)
                    return new ResponseBase(response.RequestMessage.RequestUri.ToString(), response.StatusCode, response.ReasonPhrase);

                return new ResponseBase(response.RequestMessage.RequestUri.ToString(), response.StatusCode, response.ReasonPhrase);
            }
            catch (AggregateException e)
            {
                return new ResponseBase(request.RequestUri.ToString());
            }
        }

         private Stream ReadStreamResponse(HttpRequestMessage request)
         {
             try
             {
                 Stream response = null;

                 var factory = new TaskFactory(TaskScheduler.Default);
                 factory.StartNew(() => response = _httpClient.GetStreamAsync(request.RequestUri).Result).Wait();

                 //Task.Factory
                 //    .StartNew(() => response = _httpClient.GetStreamAsync(request.RequestUri).Result,
                 //              CancellationToken.None,
                 //              TaskCreationOptions.LongRunning, // guarantees separate thread
                 //              TaskScheduler.Default)
                 //    .Wait();

                 return response;
             }
             catch (AggregateException e)
             {
                 return new MemoryStream();
             }
         }
    }
}