using Newtonsoft.Json;

namespace VkToolkit.Utils
{
    public class ErrorContainer
    {
        public Error Error
        {
            get; 
            set;
        }
    }

    public class Error
    {
        [JsonProperty("error_code")]
        public int Code
        {
            get;
            set;
        }

        [JsonProperty("error_msg")]
        public string Message
        {
            get; 
            set;
        }
    }
}