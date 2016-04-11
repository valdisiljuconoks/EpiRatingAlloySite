using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using Geta.EPi.Rating.Core;
using Geta.EpiRatingAlloySite.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using EPiServer.Web.Routing;
using NuGet;

namespace Geta.EpiRatingAlloySite.Models
{
    [PropertyDefinitionTypePlugIn]
    public class CommentListProperty : PropertyList<CommentModel>
    {
        private readonly IObjectSerializer _objectSerializer;
        private Injected<ObjectSerializerFactory> _objectSerializerFactory;

        //private ContentReference _pageRef;// = new ContentReference(6);

        public CommentListProperty()
        {
            _objectSerializer = this._objectSerializerFactory.Service.GetSerializer("application/json");
            this.List.Add(new CommentModel() { Comment = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus", CommentDate = DateTime.Now.AddMonths(-1).AddDays(-4).ToString() });
            this.List.Add(new CommentModel() { Comment = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus", CommentDate = DateTime.Now.AddMonths(-1).AddDays(-4).ToString() });
            //var page = HttpContext.Current.Handler as EPiServer.PageBase;

        }



        //protected override void SetDefaultValue()
        //{
        //    ThrowIfReadOnly();
        //    //base.SetDefaultValue();

        //    //this.List.Add(new CommentModel()
        //    //{
        //    //    Comment =
        //    //        "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Aenean vulputate eleifend tellus",
        //    //    CommentDate = DateTime.Now.AddMonths(-1).AddDays(-4).ToString()
        //    //});

        //    ContentAssetFolder assetFolder = null;
        //    //PageRouteHelper routHelper = null;

        //    try
        //    {
        //        PageRouteHelper routHelper = null;
        //        if (HttpContext.Current != null && ServiceLocator.Current.TryGetExistingInstance(out routHelper))
        //        {
        //            _pageRef = routHelper.PageLink;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }


        //    //if (ServiceLocator.Current.TryGetExistingInstance(out routHelper) && routHelper.PageLink != null)
        //    //{
        //    if (_pageRef != null)
        //    {
        //        var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
        //        assetFolder = contentAssetHelper.GetOrCreateAssetFolder(_pageRef);
        //    }

        //    //var contentRef = new ContentReference(57); //, pageRef.WorkID);

        //    if (assetFolder != null)
        //    {
        //        var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
        //        var reviews = loader.GetChildren<Review>(assetFolder.ContentLink); //reviewService.GetReviews(pageRef);
        //        var comments = reviews.Select(x => new CommentModel() { Comment = x.Text, CommentDate = x.Created.ToString() });
        //        this.List = new List<CommentModel>(comments);
        //    }
        //}

        //public override void InitializeData(PropertyDataCollection properties)
        //{
        //    base.InitializeData(properties);

        //    ContentAssetFolder assetFolder = null;
        //    //PageRouteHelper routHelper = null;
        //    ContentReference _pageRef = null;
        //    try
        //    {
        //        PageRouteHelper routHelper = null;
        //        if (HttpContext.Current != null && ServiceLocator.Current.TryGetExistingInstance(out routHelper))
        //        {
        //            _pageRef = routHelper.PageLink;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }


        //    //if (ServiceLocator.Current.TryGetExistingInstance(out routHelper) && routHelper.PageLink != null)
        //    //{
        //    if (_pageRef != null)
        //    {
        //        var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
        //        assetFolder = contentAssetHelper.GetOrCreateAssetFolder(_pageRef);
        //    }

        //    //var contentRef = new ContentReference(57); //, pageRef.WorkID);

        //    if (assetFolder != null)
        //    {
        //        var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
        //        var reviews = loader.GetChildren<Review>(assetFolder.ContentLink); //reviewService.GetReviews(pageRef);
        //        var comments = reviews.Select(x => new CommentModel() { Comment = x.Text, CommentDate = x.Created.ToString() });
        //        this.List = new List<CommentModel>(comments);
        //    }


            //if (properties.Any(p => p.Name == "RatingEnabled"))
            //{
            //    var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            //    //var reviewService = ServiceLocator.Current.GetInstance<IReviewService>();
            //    //var currentPage = repo.Get<StandardPage>(contentType.GUID);


            //    EPiServer.Core.PageReference pageRef;

            //    properties.TryGetPropertyValue("PageLink", out pageRef);
            //    ContentAssetFolder assetFolder = null;



            //    var thread = new Thread(() =>
            //    {
            //        var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            //        assetFolder = contentAssetHelper.GetAssetFolder(new ContentReference(pageRef.ID));
            //    });

            //    thread.Start();
            //    thread.Join();

            //    //var contentRef = new ContentReference(57); //, pageRef.WorkID);

            //    var reviews = loader.GetChildren<Review>(assetFolder.ContentLink, CultureInfo.CurrentUICulture); //reviewService.GetReviews(pageRef);

            //    var comments = reviews.Select(x => new CommentModel() { Comment = x.Text, CommentDate = x.Created.ToString() });

            //    //this.List.Add(new CommentModel() { Comment = "bla bla blasdfasdfasd fasdf asdf as dfasd fasd fasd fasda", CommentDate = DateTime.Now.ToString() });

            //    this.List = new List<CommentModel>(comments);
            //}
        //}

        public override PropertyData ParseToObject(string value)
        {
            ParseToSelf(value);
            return this;
        }

        protected override CommentModel ParseItem(string value)
        {
            return _objectSerializer.Deserialize<CommentModel>(value);
        }
    }
}