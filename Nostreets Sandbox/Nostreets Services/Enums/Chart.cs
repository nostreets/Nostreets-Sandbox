using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Enums
{

    public enum ChartType : int
    {
        Line,
        Bar,
        Pie
    }

    public enum ChartLibraryType : int
    {
        Chartist,
        Google
    }
}