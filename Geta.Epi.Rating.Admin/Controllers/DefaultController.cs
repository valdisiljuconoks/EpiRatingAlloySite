using System.Web.Mvc;

namespace Geta.Epi.Rating.Admin.Controllers
{
    [EPiServer.PlugIn.GuiPlugIn(Area = EPiServer.PlugIn.PlugInArea.AdminMenu, DisplayName = "Page Rating Overview", UrlFromModuleFolder = "Default")]
    public class DefaultController: Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PageComment()
        {
            ViewBag.ContentId = Request.QueryString["ContentId"];
            return View("PageComment");
        }

    }
}