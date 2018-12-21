using System;
using Hangfire;
using NostreetsExtensions.Extend.Basic;

namespace Nostreets_Sandbox
{
    public class HangfireConfig
    {

        public HangfireConfig() { }

        private static HangfireConfig _instance = new HangfireConfig();

        public static void RegisterJobs()
        {
           // RecurringJob.AddOrUpdate(() => _instance.Test(), Cron.Minutely(), TimeZoneInfo.Local);
        }


        public void Test()
        {
            "This is a test method for Hangfire...".LogInDebug();
        }

    }
}