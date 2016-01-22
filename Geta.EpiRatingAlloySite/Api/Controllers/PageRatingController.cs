using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Geta.EpiRatingAlloySite.Api.Models;
using Geta.EpiRatingAlloySite.Models;
using Geta.EPi.Rating.Core;
using Geta.EPi.Rating.Core.Models;
using NuGet;
using ILogger = EPiServer.Logging.ILogger;

namespace Geta.EpiRatingAlloySite.Api.Controllers
{
    [RoutePrefix("api/rating")]
    public class PageRatingController : ApiController
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly IContentLoader _repo;
        private ContentAssetHelper contentAssetHelper;

        public PageRatingController()
        {
            _reviewService = ServiceLocator.Current.GetInstance<IReviewService>();
            _repo = ServiceLocator.Current.GetInstance<IContentLoader>();
            contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            //_logger = logger;
        }

        [Route("ratepage")]
        [HttpPost]
        public void RatePage(RatingDto ratingData)
        {
            ContentReference reviewPageReference;
            if (ContentReference.TryParse(ratingData.ContentId, out reviewPageReference))
            {
                var review = new ReviewModel()
                {
                    Rating = ratingData.Rating ? 1 : -1,
                    ReviewText = ratingData.Comment,
                    ReviewOwnerContentLink = reviewPageReference
                };

                _reviewService.Create(review);

                AddCookie(ratingData.ContentId);

                SendNotificationEmail();
            }
            else
            {
                _logger.Log(Level.Error, $"Error parsing content reference {ratingData.ContentId}");
            }
        }


        [Route("getratings")]
        [HttpGet]
        public RatingListDto GetRatings()
        {
            var filter = new FilterContentForVisitor();
            var pages = GetChildPages(ContentReference.StartPage).ToList();
            filter.Filter(pages);
            var ratingList = new List<RatingTableData>();

            foreach (var ratingPage in pages.OfType<IRatingPage>())
            {
                var ratings = _reviewService.GetReviews(((IContent)ratingPage).ContentLink);

                if (ratings == null)
                {
                    continue;
                }

                var ratingTableData = new RatingTableData()
                {
                    PageName = ((IContent)ratingPage).Name,
                    Comments = ratings.Select(r => r.Text).Where(r => !string.IsNullOrEmpty(r)),
                    Rating = (int)ratings.Select(r => r.Rating).Sum(),
                    RatingEnabled = ratingPage.RatingEnabled
                };

                ratingList.Add(ratingTableData);
            }

            var ratingInfo = new RatingListDto()
            {
                RatingData = ratingList
            };

            return ratingInfo;
        }

        private IEnumerable<IContent> GetChildPages(ContentReference levelRootLink, ICollection<IContent> pages = null)
        {
            if (pages == null)
            {
                pages = new List<IContent>();
            }

            var children = _repo.GetChildren<IContent>(levelRootLink).ToList();

            if (children.Any())
            {
                pages.AddRange(children);
            }

            foreach (var levelItems in children)
            {
                GetChildPages(levelItems.ContentLink, pages);
            }

            return pages;
        }

        private void SendNotificationEmail()
        {
            _logger.Log(Level.Information, "Notification sent successfully");

        }

        private void AddCookie(string contentId)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie($"IsRated_{contentId}", "1"));
            }
        }
    }
}