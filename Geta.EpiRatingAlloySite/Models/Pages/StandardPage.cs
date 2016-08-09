using EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;
using EPiServer.XForms;
using Geta.EPi.Rating.Core;
using Geta.EpiRatingAlloySite.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Geta.EpiRatingAlloySite.Models.Pages
{
    /// <summary>
    /// Used for the pages mainly consisting of manually created content such as text, images, and blocks
    /// </summary>
    [SiteContentType(GUID = "9CCC8A41-5C8C-4BE0-8E73-520FF3DE8267")]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-standard.png")]
    public class StandardPage : SitePageData, IRatingPage
    {
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            //var repo = ServiceLocator.Current.GetInstance<IContentLoader>();
            //var reviewService = ServiceLocator.Current.GetInstance<IReviewService>();
            //var currentPage = repo.Get<StandardPage>(contentType.GUID);

            //var contentRef = new ContentReference(contentType.ID);

            //var reviews = reviewService.GetReviews(contentRef);

            //var comments = reviews.Select(x => new CommentModel() { Comment = x.Text, CommentDate = x.Created.ToString() });

            //Comments = new List<CommentModel>(comments);
        }

        public StandardPage()
        {

        }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 310)]
        [CultureSpecific]
        public virtual XhtmlString MainBody { get; set; }

        [Display(
            GroupName = SystemTabNames.Content,
            Order = 320)]
        public virtual ContentArea MainContentArea { get; set; }

        [Display(GroupName = "Rating")]
        public virtual bool RatingEnabled { get; set; }

        [Display(GroupName = "Rating")]
        public virtual bool IgnorePublish { get; set; }

        [UIHint(EPiServer.Web.UIHint.Textarea)]
        [Editable(false)]
        public virtual string CommentsLink { get; set; }

        [Display(GroupName = "Rating")]
        [Editable(true)]
        [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<CommentModel>))]
        public virtual IList<CommentModel> Comments { get; set; }

        [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<LinkModel>))]
        public virtual IList<LinkModel> LinkList { get; set; }

        public virtual XForm FormField { get; set; }

        [Ignore]
        public virtual string ActionUri { get; set; }


        [Display(Name = "Rating data", Description = "Main Title", GroupName = "Rating", Order = 200)]
        [UIHint("RatingProperty")]
        public virtual string MainTitle { get; set; }


        public virtual string SomeProperString { get; set; }


        //[Ignore]
        [ScaffoldColumn(false)]
        public virtual IList<string> Bla { get; set; }
    }
}
