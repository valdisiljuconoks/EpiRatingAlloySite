using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using EPiServer;
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
using WebGrease.Css.Extensions;

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

        [Route("getratings")]
        [HttpGet]
        public RatingListDto GetRatings([FromUri]RatingFilterDto filterParams)
        {
            var ratingDataList = new RatingListDto();
            var filter = new FilterContentForVisitor();
            var pages = GetChildPages(ContentReference.StartPage).ToList();
            filter.Filter(pages);
            var ratingTableDataList = new List<RatingTableDataDto>();

            var ratingInterfacePages = filterParams != null && filterParams.RatingEnabled ?
                pages.OfType<IRatingPage>().Where(p => p.RatingEnabled == filterParams.RatingEnabled) : pages.OfType<IRatingPage>();

            foreach (var ratingPage in ratingInterfacePages)
            {
                var ratingContent = (IContent)ratingPage;
                var ratings = _reviewService.GetReviews(ratingContent.ContentLink);

                if (ratings == null)
                {
                    continue;
                }

                var ratingsList = ratings.ToList();

                var ratingTableData = new RatingTableDataDto
                {
                    PageName = ratingContent.Name,
                    RatingEnabled = ratingPage.RatingEnabled,
                    ContentId = ratingContent.ContentLink.ID.ToString(),
                    ContentUrl = PageEditing.GetEditUrl(ratingContent.ContentLink),
                    PageFriendlyUrl = _urlResolver.GetUrl(ratingContent.ContentLink)
                };

                if (ratingsList.Any())
                {
                    if (filterParams != null)
                    {
                        ratingsList = ratingsList.Where(r => !filterParams.DateFrom.HasValue || r.Created.Date >= filterParams.DateFrom.Value.Date &&
                                                             (!filterParams.DateTo.HasValue || r.Created.Date <= filterParams.DateTo.Value.Date)).ToList();
                    }

                    var comments =
                        ratingsList.Where(r => !string.IsNullOrEmpty(r.Text))
                                   .Select(r => new RatingCommentDto { CommentText = r.Text, CommentDate = r.Created }).ToList();
                    ratingTableData.Comments = comments;
                    ratingTableData.ShortComments = comments.OrderByDescending(c => c.CommentDate).Take(5);

                    ratingTableData.ShortComments.ForEach(comment =>
                    {
                        if (comment.CommentText.Length > 500)
                        {
                            comment.CommentText = comment.CommentText.Substring(0, 500) + "...";
                        }
                    });

                    ratingTableData.Rating = (int)ratingsList.Select(r => r.Rating).Sum();
                    ratingTableData.LastCommentDate = comments.OrderByDescending(c => c.CommentDate).First().CommentDate;
                    ratingTableData.RatingCount = ratingsList.Count;
                    ratingTableData.PositiveRatingCount = ratingsList.Count(r => r.Rating > 0);
                    ratingTableData.NegativeRatingCount = ratingsList.Count(r => r.Rating < 0);
                }

                if (filterParams == null || !filterParams.OnlyRatedPages || (filterParams.OnlyRatedPages && ratingsList.Any()))
                {
                    ratingTableDataList.Add(ratingTableData);
                }
            }
            ratingDataList.RatingData = ratingTableDataList;

            return ratingDataList;
        }


        [Route("getpagecomments")]
        [HttpGet]
        public RatingListDto GetPageComments([FromUri]RatingFilterDto filterParams)
        {
            var ratingTableData = new RatingTableDataDto();

            ContentReference contentRef;

            if (ContentReference.TryParse(filterParams.ContentId, out contentRef))
            {
                var ratingPageContent = _loader.Get<IContent>(contentRef);
                var ratings = GetReviews(contentRef);

                ratingTableData.PageName = ratingPageContent.Name;
                ratingTableData.ContentId = ratingPageContent.ContentLink.ID.ToString();
                ratingTableData.Comments =
                    ratings.Where(r => !string.IsNullOrEmpty(r.Text)).OrderByDescending(r => r.Created).
                    Select(r => new RatingCommentDto { CommentText = r.Text, CommentDate = r.Created });
            }
            return new RatingListDto { RatingData = new List<RatingTableDataDto> { ratingTableData } };
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
        public void EnableRating(RatingDto actionInfo)
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
        public ResponseDto PageIsPublished(RatingDto actionInfo)
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