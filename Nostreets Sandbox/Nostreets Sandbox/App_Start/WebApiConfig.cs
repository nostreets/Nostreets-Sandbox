using Newtonsoft.Json.Serialization;
using NostreetsExtensions.Extend.Web;
using System.Web.Http;

namespace Nostreets_Sandbox
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Web API configuration and services

            //+Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //+GlobalConfiguration.Configuration
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //var container = App_Start.UnityConfig.GetContainer();
            //config.DependencyResolver = new App_Start.UnityResolver(container);
        }
    }
}
