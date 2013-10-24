using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace VkToolkit.Utils
{
    public static class WebRequestFactory
    {
        internal static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();

            return client;
        }

        internal static HttpRequestMessage CreatePostRequest(string requestUrl, MediaTypeWithQualityHeaderValue acceptHeaderValue)
        {
            return CreateUpdateRequest(requestUrl, HttpMethod.Post, acceptHeaderValue);
        }

        internal static HttpRequestMessage CreatePostRequest(string requestUrl, MediaTypeWithQualityHeaderValue acceptHeaderValue, Stream stream, MediaTypeHeaderValue contentType)
        {
            return CreateUpdateRequest(requestUrl, HttpMethod.Post, acceptHeaderValue, stream, contentType);
        }

        internal static HttpRequestMessage CreatePutRequest(string requestUrl, MediaTypeWithQualityHeaderValue acceptHeaderValue)
        {
            return CreateUpdateRequest(requestUrl, HttpMethod.Put, acceptHeaderValue);
        }

        internal static HttpRequestMessage CreateDeleteRequest(string requestUrl, MediaTypeWithQualityHeaderValue acceptHeaderValue)
        {
            return CreateUpdateRequest(requestUrl, HttpMethod.Delete, acceptHeaderValue);
        }

        internal static HttpRequestMessage CreateGetRequest(string requestUrl, MediaTypeWithQualityHeaderValue acceptHeaderValue)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Accept.Add(acceptHeaderValue);

            return request;
        }

        private static HttpRequestMessage CreateUpdateRequest(string requestUrl, HttpMethod httpMethod, MediaTypeWithQualityHeaderValue acceptHeaderValue)
        {
            var request = new HttpRequestMessage(httpMethod, requestUrl);
            request.Headers.Accept.Add(acceptHeaderValue);

            return request;
        }

        private static HttpRequestMessage CreateUpdateRequest(string requestUrl, HttpMethod httpMethod, MediaTypeWithQualityHeaderValue acceptHeaderValue, Stream stream, MediaTypeHeaderValue contentType)
        {
            var request = CreateUpdateRequest(requestUrl, httpMethod, acceptHeaderValue);

            request.Content = new StreamContent(stream);
            request.Content.Headers.ContentType = contentType;
            
            return request;
        }
    }
}