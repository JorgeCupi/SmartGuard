using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartGuard.Core.Facebook.Classes
{
    public class FacebookUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("picture")]
        public Picture Picture { get; set; }

        [JsonProperty("installed")]
        public bool UsesApp { get; set; }

        public string fullDetail { get; set; }
    }

    public class Paging
    {
        [JsonProperty("next")]
        public string next { get; set; }
    }

    public class Picture
    {
        [JsonProperty("data")]
        public PictureData data { get; set; }
    }

    public class PictureData
    {
        [JsonProperty("url")]
        public string url { get; set; }

        [JsonProperty("is_silhouette")]
        public bool isSilhouette { get; set; }
    }

    public class FacebookData
    {
        [JsonProperty("data")]
        public List<FacebookUser> friends { get; set; }

        [JsonProperty("paging")]
        public Paging paging { get; set; }
    }
}