using Nostreets_Sandbox.App_Start;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace Nostreets_Sandbox
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {

           // AreaRegistration.RegisterAllAreas();

            WindsorConfig.RegisterInterfaces(GlobalConfiguration.Configuration);
            //UnityConfig.RegisterInterfaces(GlobalConfiguration.Configuration);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFDBContext<User>>());

        }

        protected void Application_End()
        {
            WindsorConfig.GetContainer().Dispose();
        }

        //protected void Application_PostResolveRequestCache(object sender, EventArgs e)
        //{
        //    HttpApplication _sender = (HttpApplication)sender;
        //}
    }
}
