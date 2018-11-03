using Hangfire;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Nostreets_Sandbox.Classes;
using Owin;
using System;

[assembly: OwinStartup(typeof(Nostreets_Sandbox.Startup))]
namespace Nostreets_Sandbox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
            //Hangfire_Start(app);
        }

        private void Hangfire_Start(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(ConfigKeys.DBConnectionString);

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
