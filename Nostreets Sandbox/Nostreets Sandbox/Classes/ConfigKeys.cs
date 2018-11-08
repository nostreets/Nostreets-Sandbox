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


        public static string DBConnectionString
        {
            get
            {
#if DEBUG
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
#else
                //return ConfigurationManager.ConnectionStrings["GoogleConnection"].ConnectionString;
                return ConfigurationManager.ConnectionStrings["AWSConnection"].ConnectionString;
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