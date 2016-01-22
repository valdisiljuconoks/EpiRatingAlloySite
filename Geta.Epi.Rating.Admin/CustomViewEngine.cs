using System.Web.Mvc;

namespace Geta.Epi.Rating.Admin
{
    public class CustomViewEngine : RazorViewEngine
    {
        public CustomViewEngine()
        {
            var basepath = "~/modules/_protected/" + ModuleConst.ModuleName;

            ViewLocationFormats = new[]
            {
                basepath + "/Views/{1}/{0}.cshtml"
            };

            MasterLocationFormats = new[]
            {
                basepath + "/Views/Shared/{0}.cshtml"
            };

            PartialViewLocationFormats = new[]
            {
                basepath + "/Views/Shared/{0}.cshtml"
            };
        }
    }
}