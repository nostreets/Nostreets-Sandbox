using NostreetsExtensions.Extend.Basic;
using NostreetsExtensions.Utilities;
using RazorGenerator.Mvc;
using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Nostreets_Sandbox.ViewEngineConfig), "Start")]

namespace Nostreets_Sandbox
{
    public static class ViewEngineConfig
    {
        public static void Start()
        {
            ViewEngines.Engines.Clear();

            //+ Add the current project compiled view engine
            AddPrecompiledViewEngine(typeof(ViewEngineConfig).Assembly);

            //+ Add the common library compiled view engine
            AddPrecompiledViewEngine(AppDomain.CurrentDomain.GetAssembly("OBL_Website"));

            ViewEngines.Engines.Add(new CSharpRazorViewEngine());
        }

        private static void AddPrecompiledViewEngine(Assembly assembly)
        {
            PrecompiledViewEngine engine = new PrecompiledViewEngine(assembly);
            ViewEngines.Engines.Add(engine);
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
