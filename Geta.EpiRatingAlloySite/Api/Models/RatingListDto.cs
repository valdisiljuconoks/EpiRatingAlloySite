using System.Collections.Generic;
using Newtonsoft.Json;

namespace Geta.EpiRatingAlloySite.Api.Models
{
    public class RatingListDto
    {
        [JsonProperty("ratingData")]
        public IEnumerable<RatingTableDataDto> RatingData { get; set; }
    }
}