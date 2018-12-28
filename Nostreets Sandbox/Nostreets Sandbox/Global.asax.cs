using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Nostreets_Sandbox.App_Start;
using Nostreets_Sandbox.Classes;

using NostreetsEntities;

using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.Extend.Web;
using NostreetsExtensions.Helpers;

namespace Nostreets_Sandbox
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //+IOC (Inversion of Control) Config
            WindsorConfig.RegisterInterfaces(GlobalConfiguration.Configuration);
            //UnityConfig.RegisterInterfaces(GlobalConfiguration.Configuration);

            //+ASP.NET Configs
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);


            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.RegisterApiExternalRoute("OBL_Website");


            RouteConfig.RegisterRoutes(RouteTable.Routes);
            RouteTable.Routes.RegisterMvcExternalRoute("OBL_Website");

#if DEBUG
            BundleTable.EnableOptimizations = true;
#else
            BundleTable.EnableOptimizations = true;
#endif

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            OBL_Website.BundleConfig.RegisterOBLBundles(BundleTable.Bundles);

            //+EF Config
            EFDBService<Error>.Migrate(ConfigKeys.WebsiteConnectionString);

        }

        protected void Application_End()
        {
            WindsorConfig.GetContainer().Dispose();
        }

    }
}