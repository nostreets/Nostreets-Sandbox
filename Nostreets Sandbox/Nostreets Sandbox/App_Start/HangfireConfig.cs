using System;
using System.Reflection;
using NostreetsExtensions.Extend.Basic;
using NostreetsExtensions.Helpers;

namespace Nostreets_Sandbox
{
    public class HangfireConfig
    {

        private HangfireConfig() { }

        private static HangfireConfig _instance = new HangfireConfig();

        public static void RegisterJobs()
        {
            ((Action)_instance.Test).RegisterJob();
        }


        private void Test()
        {
            "This is a test method".LogInDebug();
        }
        
    }
}