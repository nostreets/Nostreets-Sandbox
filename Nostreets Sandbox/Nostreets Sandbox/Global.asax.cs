using Nostreets_Sandbox.App_Start;
using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Bills;
using NostreetsEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Nostreets_Sandbox
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityConfig.RegisterInterfaces(GlobalConfiguration.Configuration);

            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<UserDbContext>());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFDBContext<Income>>());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFDBContext<Expenses>>());

        }
    }
}
