using System.Web.Http;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Geta.EpiRatingAlloySite.Api.Models;
using Geta.EPi.Rating.Core;
using Geta.EPi.Rating.Core.Models;

namespace Geta.EpiRatingAlloySite.Api.Controllers
{
    [RoutePrefix("api/rating")]
    public class PageRatingController : ApiController
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger _logger = LogManager.GetLogger();

        public PageRatingController()
        {
            _reviewService = ServiceLocator.Current.GetInstance<IReviewService>();
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

                SendNotificationEmail();
            }
            else
            {
                _logger.Log(Level.Error, $"Error parsing content reference {ratingData.ContentId}");
            }
        }

        private void SendNotificationEmail()
        {
            _logger.Log(Level.Information, "Notification sent successfully");

        }
    }
}