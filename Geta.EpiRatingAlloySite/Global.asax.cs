using System;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Geta.EpiRatingAlloySite
{
    public class EPiServerApplication : EPiServer.Global
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(Register);
            //Tip: Want to call the EPiServer API on startup? Add an initialization module instead (Add -> New Item.. -> EPiServer -> Initialization Module)
        }


        private static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
        }
    }
}