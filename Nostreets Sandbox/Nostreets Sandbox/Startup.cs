using Hangfire;
using Microsoft.Owin;
using Nostreets_Sandbox.Classes;
using Owin;

[assembly: OwinStartup(typeof(Nostreets_Sandbox.Startup))]
namespace Nostreets_Sandbox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Hangfire_Start(app);
            HangfireConfig.RegisterJobs();
        }

        private void Hangfire_Start(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigKeys.HangfireConnectionString);

            app.UseHangfireDashboard();
            app.UseHangfireServer();

        }

    }
}
