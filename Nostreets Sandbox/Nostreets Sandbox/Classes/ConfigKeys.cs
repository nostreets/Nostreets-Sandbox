using System.Configuration;

namespace Nostreets_Sandbox.Classes
{
    public class ConfigKeys
    {

        public static string ServerDomain {
            get
            {
#if DEBUG
                return ConfigurationManager.ConnectionStrings["ProdDomain"].ConnectionString;
#else
                return ConfigurationManager.ConnectionStrings["DebugDomain"].ConnectionString;
#endif
            }
        }

        public static string WebsiteConnectionKey
        {
            get
            {
#if DEBUG
                return "DefaultConnection";
#else
                return "AWS_Portfolio_Connection";
#endif
            }
        }


        public static string WebsiteConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[WebsiteConnectionKey].ConnectionString;
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


        public static string ShopifyDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["Shopify.Domain"];
            }
        }


        public static string ShopifyApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["Shopify.ApiKey"];
            }
        }

        public static string PrintfulDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["Printful.Domain"];
            }
        }


        public static string PrintfulApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["Printful.ApiKey"];
            }
        }

    }
}