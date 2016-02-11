using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geta.EpiRatingAlloySite.Models.ViewModels
{
    public class RatingViewModel
    {
        public string ContentId { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IgnorePublish { get; set; }
    }
}