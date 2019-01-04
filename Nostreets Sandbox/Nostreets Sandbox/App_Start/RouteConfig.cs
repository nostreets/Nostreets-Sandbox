using Nostreets_Sandbox.Classes;
using NostreetsExtensions.Extend.Web;
using RazorGenerator.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nostreets_Sandbox
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //CSharpRazorViewEngine engine = new CSharpRazorViewEngine();

            ////Remove All View Engine  
            //ViewEngines.Engines.Clear();
            ////Add Custom C# Razor View Engine  
            //ViewEngines.Engines.Add(engine);


            //PrecompiledMvcEngine engine = new PrecompiledMvcEngine(typeof(RazorGeneratorMvcStart).Assembly);
            //ViewEngines.Engines.Insert(0, engine);



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
