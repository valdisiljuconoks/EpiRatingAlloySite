using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace Geta.Epi.Rating.Admin
{
    [InitializableModule]
    public class InitializeModule: IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RegisterRoute(RouteTable.Routes, "Default");
            ViewEngines.Engines.Add(new CustomViewEngine());
        }

        public void Uninitialize(InitializationEngine context)
        {

        }


        private void RegisterRoute(RouteCollection routes, string controllerName)
        {
            routes.MapRoute("RatingAdmin_" + controllerName,
                            "modules/_protected/" + ModuleConst.ModuleName + "/" + controllerName + "/{action}/{id}",
                            new { controller = controllerName, action = "Index", id = UrlParameter.Optional },
                            new[] { "Geta.Epi.Rating.Admin.Controllers" }).DataTokens["UseNamespaceFallback"] = false;
        }
    }
}