using System.Collections.Generic;
using System.Net;

namespace VkToolkit.Utils
{
    public class Response<T> : ResponseBase
    {
        public Response(ResponseBase responseBase)
            : base(responseBase)
        { }

        public Response(string responseUrl, HttpStatusCode statusCode, string statusDescription)
            : base(responseUrl, statusCode, statusDescription)
        { }

        public Response(string responseUrl)
            : base(responseUrl)
        { }

        public Response(string responseUrl, HttpStatusCode statusCode, string statusDescription, IEnumerable<string> varyHeader, T resource, Error error)
            : base(responseUrl, statusCode, statusDescription, varyHeader, resource, error)
        { }

        public T Resource
        {
            get { return (T)ResourceObject; }
        }
    }
}