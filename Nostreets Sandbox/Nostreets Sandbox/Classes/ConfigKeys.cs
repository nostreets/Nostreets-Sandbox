using System.Configuration;

namespace Nostreets_Sandbox.Classes
{
    public class ConfigKeys
    {

        public static string DBConnectionKey
        {
            get
            {
#if DEBUG
                return "DefaultConnection";
#else
                return "GoogleConnection";
#endif
            }
        }


        public static string PortfolioConnectionString
        {
            get
            {
#if DEBUG
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
#else
                return ConfigurationManager.ConnectionStrings["AWS_Portfolio_Connection"].ConnectionString;
#endif
            }
        }


        public static string HangfireConnectionString
        {
            get
            {
#if DEBUG
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
#else
                return ConfigurationManager.ConnectionStrings["AWS_Hangfire_Connection"].ConnectionString;
#endif
            }
        }


        public static string SendGridApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["SendGrid.ApiKey"];
            }
        }

    }
}