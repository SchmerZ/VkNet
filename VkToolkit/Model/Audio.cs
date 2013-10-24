using System;
using Newtonsoft.Json;

namespace VkToolkit.Model
{
    public class Audio
    {
        [JsonProperty("aid")]
        public long Id
        {
            get; 
            set;
        }

        [JsonProperty("owner_id")]
        public long OwnerId
        {
            get; 
            set;
        }

        public string Artist
        {
            get; 
            set;
        }

        public string Title
        {
            get; 
            set;
        }

        public int Duration
        {
            get; 
            set;
        }

        public Uri Url
        {
            get; 
            set;
        }

        [JsonProperty("lyrics_id")]
        public long? LyricsId
        {
            get; 
            set;
        }

        [JsonProperty("album")]
        public long? AlbumId
        {
            get; 
            set;
        }

        public string Performer
        {
            get; 
            set;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", Artist, Title);
        }
    }
}