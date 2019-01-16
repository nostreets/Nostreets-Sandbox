using NostreetsExtensions.Extend.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nostreets_Sandbox
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Sandbox", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Nostreets_Sandbox.Controllers" }
            );




            routes.RegisterMvcExternalRoute("OBL_Website.Controllers");

        }




    }
}
