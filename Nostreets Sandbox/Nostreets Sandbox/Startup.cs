using Microsoft.Owin;
using Nostreets_Sandbox.Classes;
using NostreetsExtensions.DataControl.Attributes;
using NostreetsExtensions.Extend.Web;
using Owin;

[assembly: OwinStartup(typeof(Nostreets_Sandbox.Startup))]
namespace Nostreets_Sandbox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            app.HangfireStart(ConfigKeys.HangfireConnectionString, true, "/admin/hangire/dashboard", 
                new Hangfire.DashboardOptions { Authorization = new[] { new HangfireAuthorizationFilter() } }
                );

            OBL_Website.HangfireConfig.RegisterJobs();
        }

    }
}
