using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Reviewer.SharedModels
{
    public class Business
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public Address Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("recentReviews")]
        public List<Review> RecentReviews { get; set; }

        [JsonIgnore()]
        public string FirstInitial { get => Name.Substring(0, 1); }
    }
}
