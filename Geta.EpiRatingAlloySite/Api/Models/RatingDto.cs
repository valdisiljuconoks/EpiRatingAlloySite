using Newtonsoft.Json;

namespace Geta.EpiRatingAlloySite.Api.Models
{
    public class RatingDto
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("rating")]
        public bool Rating { get; set; }

        [JsonProperty("contentId")]
        public string ContentId { get; set; }
    }
}