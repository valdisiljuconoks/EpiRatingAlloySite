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
using EPiServer.Web.Routing;
using EPiServer.Editor;
using Newtonsoft.Json;

namespace Geta.EpiRatingAlloySite.Api.Controllers
{
    [RoutePrefix("api/rating")]
    public class PageRatingController : ApiController
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger _logger = LogManager.GetLogger();
        private readonly IContentLoader _loader;
        private readonly IContentRepository _repository;
        private readonly UrlResolver _urlResolver;
        //private readonly ContentAssetHelper _contentAssetHelper;
        private ContentAssetHelper contentAssetHelper;

        public PageRatingController()
        {
            _reviewService = ServiceLocator.Current.GetInstance<IReviewService>();
            _loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            _repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            _urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
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

        public class RatingFilter
        {
            [JsonProperty("dateFrom")]
            public DateTime? DateFrom { get; set; }
            [JsonProperty("dateTo")]
            public DateTime? DateTo { get; set; }
            [JsonProperty("contentId")]
            public string ContentId { get; set; }
        }

        [Route("getratings")]
        [HttpGet]
        public RatingListDto GetRatings([FromUri]RatingFilter filterParams)
        {
            var filter = new FilterContentForVisitor();
            var pages = GetChildPages(ContentReference.StartPage).ToList();
            filter.Filter(pages);
            var ratingList = new List<RatingTableData>();

            foreach (var ratingPage in pages.OfType<IRatingPage>())
            {
                var ratingPageContent = (IContent)ratingPage;

                var ratings = GetReviews(ratingPageContent.ContentLink);

                if (ratings == null)
                {
                    continue;
                }

                if (filterParams != null)
                {
                    ratings =
                        ratings.Where(r => (!filterParams.DateFrom.HasValue || (filterParams.DateFrom.HasValue && r.Created >= filterParams.DateFrom.Value)) &&
                                           (!filterParams.DateTo.HasValue || (filterParams.DateTo.HasValue && r.Created <= filterParams.DateTo.Value)));
                }

                var ratingTableData = new RatingTableData()
                {
                    PageName = ratingPageContent.Name,
                    Rating = (int)ratings.Select(r => r.Rating).Sum(),
                    RatingEnabled = ratingPage.RatingEnabled,
                    ContentId = ratingPageContent.ContentLink.ID.ToString(),
                    ContentUrl = PageEditing.GetEditUrl(ratingPageContent.ContentLink)
                };

                var comments = ratings.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => new RatingCommentDto { CommentText = r.Text, CommentDate = r.Created });
                ratingTableData.Comments = comments;
                ratingTableData.ShortComments = comments.OrderByDescending(c => c.CommentDate).Take(5);
                ratingList.Add(ratingTableData);
            }

            var ratingInfo = new RatingListDto()
            {
                RatingData = ratingList
            };

            return ratingInfo;
        }


        [Route("getpagecomments")]
        [HttpGet]
        public RatingTableData GetPageComments([FromUri] RatingFilter filterParams)
        {
            var ratingTableData = new RatingTableData();

            ContentReference contentRef;

            if (ContentReference.TryParse(filterParams.ContentId, out contentRef))
            {
                var ratingPageContent = _loader.Get<IContent>(contentRef);
                var ratings = GetReviews(contentRef);

                ratingTableData.PageName = ratingPageContent.Name;
                ratingTableData.ContentId = ratingPageContent.ContentLink.ID.ToString();
                ratingTableData.Comments =
                    ratings.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => new RatingCommentDto { CommentText = r.Text, CommentDate = r.Created });
            }
            return ratingTableData;
        }


        public IEnumerable<Review> GetReviews(ContentReference contentReference)
        {
            
            //contentReference.
            //var product = _loader.Get<IContent>(contentReference);

            var assetFolder = contentAssetHelper.GetAssetFolder(contentReference);

            if(assetFolder == null)
            {
                return null;
            }
            return _loader.GetChildren<Review>(assetFolder.ContentLink);
                //.OrderByDescending(m => m.StartPublish);
        }


        [Route("enablerating")]
        [HttpPost]
        public void EnableRating(RatingSwitchDto actionInfo)
        {
            ContentReference reviewPageReference;

            if (ContentReference.TryParse(actionInfo.ContentId, out reviewPageReference))
            {
                var page = _loader.Get<PageData>(reviewPageReference);
                var writablePage = page.CreateWritableClone();
                var ratingPage = writablePage as IRatingPage;

                if (ratingPage != null)
                {
                    ratingPage.RatingEnabled = actionInfo.RatingEnabled;
                }

                _repository.Save(writablePage, EPiServer.DataAccess.SaveAction.Publish);
            }
        }

        [Route("pageispublished")]
        [HttpGet]
        public ResponseDto PageIsPublished(RatingSwitchDto actionInfo)
        {
            ContentReference reviewPageReference;

            var response = new ResponseDto();

            if (ContentReference.TryParse(actionInfo.ContentId, out reviewPageReference))
            {

                var page = _loader.Get<PageData>(reviewPageReference);

                response.PageIsPublished = page.Status == VersionStatus.Published;
            }
            return response;
        }

        private IEnumerable<IContent> GetChildPages(ContentReference levelRootLink, ICollection<IContent> pages = null)
        {
            if (pages == null)
            {
                pages = new List<IContent>();
            }

            var children = _loader.GetChildren<IContent>(levelRootLink).ToList();

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
            if (HttpContext.Current == null)
            {
                return;
            }

            var ratingCookie = HttpContext.Current.Request.Cookies["Ratings"] ?? new HttpCookie("Ratings") { HttpOnly = false };
            ratingCookie.Expires = DateTime.Now.AddYears(1);
            var cookieSubkeyName = $"c_{contentId}";
            var cookieSubkeyValue = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            ratingCookie.Values.Remove(cookieSubkeyName);
            ratingCookie.Values.Add(cookieSubkeyName, cookieSubkeyValue);
            HttpContext.Current.Response.Cookies.Add(ratingCookie);
        }
    }
}