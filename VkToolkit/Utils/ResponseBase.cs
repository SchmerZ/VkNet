using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;

namespace VkToolkit.Utils
{
    public class ResponseBase
    {
        public ResponseBase(string responseUrl, HttpStatusCode statusCode, string statusDescription)
        {
            ResponseUrl = responseUrl;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        internal ResponseBase(string responseUrl, HttpStatusCode statusCode, string statusDescription, IEnumerable<string> varyHeader, object resource, Error error)
        {
            ResponseUrl = responseUrl;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            VaryHeader = varyHeader;
            ResourceObject = resource;
            Error = error;
        }

        public ResponseBase(string responseUrl)
        {
            ResponseUrl = responseUrl;
        }

        protected ResponseBase(ResponseBase responseBase)
        {
            StatusCode = responseBase.StatusCode;
            StatusDescription = responseBase.StatusDescription;
            ServerException = responseBase.ServerException;
            VaryHeader = responseBase.VaryHeader;
            Language = responseBase.Language;
            ResponseUrl = responseBase.ResponseUrl;
            ResourceObject = responseBase.ResourceObject;
        }

        internal ResponseBase(ResponseBase responseBase, object resourceObject)
        {
            StatusCode = responseBase.StatusCode;
            StatusDescription = responseBase.StatusDescription;
            ServerException = responseBase.ServerException;
            VaryHeader = responseBase.VaryHeader;
            Language = responseBase.Language;
            ResponseUrl = responseBase.ResponseUrl;
            ResourceObject = resourceObject;
        }

        public HttpStatusCode? StatusCode { get; private set; }
        public string StatusDescription { get; private set; }
        public XElement ServerException { get; private set; }
        public IEnumerable<string> VaryHeader { get; private set; }
        public string Language { get; private set; }
        public string ResponseUrl { get; private set; }
        public object ResourceObject { get; private set; }
        public Error Error { get; set; }
    }
}