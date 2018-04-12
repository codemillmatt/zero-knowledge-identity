using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Reviewer.SharedModels
{
    public class Review
    {
        public Review()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("businessId")]
        public string BusinessId { get; set; }

        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("authorId")]
        public string AuthorId { get; set; }

        [JsonProperty("reviewText")]
        public string ReviewText { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("photos")]
        public List<string> Photos { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }
    }
}

