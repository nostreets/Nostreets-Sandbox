using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace Nostreets_Sandbox
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //+Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //+GlobalConfiguration.Configuration
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();



            //+Web API Services
            //config.Services.Replace(typeof(System.Web.Http.Dispatcher.IAssembliesResolver), new AssemblyResolver("OBL_Webiste"));
            //config.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerSelector), new ControllerSelector(GlobalConfiguration.Configuration, "OBL_Webiste"));



            //config.RegisterApiExternalRoute("OBL_Website.Controllers.Api");

        }
    }
}
