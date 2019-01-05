using Microsoft.Owin;
using Nostreets_Sandbox.Classes;
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
            app.HangfireStart(ConfigKeys.HangfireConnectionString);
            OBL_Website.HangfireConfig.RegisterJobs();
        }

    }
}
