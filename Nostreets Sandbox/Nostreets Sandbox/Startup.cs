using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Nostreets_Sandbox.Startup))]
namespace Nostreets_Sandbox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
