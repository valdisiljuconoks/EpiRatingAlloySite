using EPiServer.Core;

namespace Geta.EpiRatingAlloySite.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}
