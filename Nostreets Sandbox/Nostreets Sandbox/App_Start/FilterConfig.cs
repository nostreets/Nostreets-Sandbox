﻿using Nostreets.Extensions.DataControl.Attributes;
using System.Web;
using System.Web.Mvc;

namespace Nostreets_Sandbox
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GzipAttribute());
        }
    }
}
