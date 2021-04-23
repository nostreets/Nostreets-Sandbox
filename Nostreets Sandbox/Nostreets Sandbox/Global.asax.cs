using Nostreets_Sandbox.App_Start;
using Nostreets_Sandbox.Classes;
using Nostreets_Services.Classes.Domain.Web;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NostreetsEntities;
using Nostreets.Extensions.DataControl.Classes;

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

            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);


#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //+EF Config
            EFDBService<Error>.Migrate(ConfigKeys.WebsiteConnectionString);
            EFDBService<WebRequestError>.Migrate(ConfigKeys.WebsiteConnectionString);


        }



        protected void Application_End()
        {
            WindsorConfig.GetContainer().Dispose();
        }


    }
}