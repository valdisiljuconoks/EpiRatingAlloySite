using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geta.EpiRatingAlloySite.Api.Models
{
    public class RatingSwitchDto
    {
        public string ContentId { get; set; }
        public bool RatingEnabled { get; set; }
    }
}