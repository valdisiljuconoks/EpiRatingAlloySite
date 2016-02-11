using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Geta.EpiRatingAlloySite.Api.Models
{
    public class RatingListDto
    {
        [JsonProperty("ratingData")]
        public IEnumerable<RatingTableData> RatingData { get; set; }

         

    }


    public class RatingTableData
    {
        [JsonProperty("pageName")]
        public string PageName { get; set; }
        [JsonProperty("rating")]
        public int Rating { get; set; }
        [JsonProperty("comments")]
        public IEnumerable<string> Comments { get; set; }
        [JsonProperty("ratingEnabled")]
        public bool RatingEnabled { get; set; }
        [JsonProperty("contentId")]
        public string ContentId { get; set; }
        [JsonProperty("contentUrl")]
        public string ContentUrl { get; set; }
    }
}