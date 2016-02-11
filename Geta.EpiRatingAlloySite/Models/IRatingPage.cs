using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geta.EpiRatingAlloySite.Models
{
    public interface IRatingPage
    {
        bool RatingEnabled { get; set; }

        bool IgnorePublish { get; set; }
    }
}
