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
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

#if DEBUG
            BundleTable.EnableOptimizations = true;
#else
            BundleTable.EnableOptimizations = true;
#endif
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //+EF Config
            EFDBService<Error>.Migrate(ConfigKeys.PortfolioConnectionString);

        }

        protected void Application_End()
        {
            WindsorConfig.GetContainer().Dispose();
        }

    }
}