using System.Web.Mvc;
using EPiServer;
using EPiServer.Web.Mvc;
using Geta.EpiRatingAlloySite.Models.Pages;
using EPiServer.Web.Routing;
using EPiServer.XForms.Util;
using EPiServer.Web.Mvc.XForms;
using Geta.EpiRatingAlloySite.Models.ViewModels;

namespace Geta.EpiRatingAlloySite.Controllers
{
    public class StandardPageController : PageController<StandardPage>
    {
        private readonly PageRouteHelper _pageRouteHelper;
        private readonly XFormPageUnknownActionHandler _xformHandler;
        private string _contentId;

        public StandardPageController(PageRouteHelper pageRouteHelper)
        {
            _pageRouteHelper = pageRouteHelper;
            _xformHandler = new XFormPageUnknownActionHandler();
            _contentId = string.Empty;
        }

        public ActionResult Index(StandardPage currentPage)
        {
            /* Implementation of action. You can create your own view model class that you pass to the view or
             * you can pass the page type for simpler templates */

            // Create postback url  
            if (currentPage.FormField != null && _pageRouteHelper.Page != null)
            {
                var actionUri = "XFormPost/";
                actionUri = UriSupport.AddQueryString(actionUri, "failedAction", "Failed");
                actionUri = UriSupport.AddQueryString(actionUri, "successAction", "Success");
                currentPage.ActionUri = actionUri;
            }


            var model = new PageViewModel<StandardPage>(currentPage);

            return View(model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult XFormPost(XFormPostedData xFormpostedData, string contentId)
        {
            _contentId = contentId;
            return _xformHandler.HandleAction(this);
        }
    }
}