using Hangfire;
using NostreetsExtensions.Extend.Basic;

namespace Nostreets_Sandbox
{
    public class HangfireConfig
    {

        private HangfireConfig() { }

        private static HangfireConfig _instance = new HangfireConfig();

        public static void RegisterJobs()
        {
            RecurringJob.AddOrUpdate(() => _instance.Test(), Cron.Minutely());
        }


        private void Test()
        {
            "This is a test method for Hangfire...".LogInDebug();
        }

    }
}