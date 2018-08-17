using Nostreets_Sandbox.App_Start;
using NostreetsEntities;
using NostreetsExtensions.DataControl.Classes;
using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //+EF Config
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFDBContext<Error>>());

        }

        protected void Application_End()
        {
            WindsorConfig.GetContainer().Dispose();
        }
    }
}
