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
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            

            RouteConfig.RegisterRoutes(RouteTable.Routes);


            GlobalConfiguration.Configure(WebApiConfig.Register);
           

            BundleTable.EnableOptimizations = true;
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //+EF Config
            EFDBService<Error>.Migrate(ConfigKeys.WebsiteConnectionString);

        }

        protected void Application_End()
        {
            WindsorConfig.GetContainer().Dispose();
        }

    }
}