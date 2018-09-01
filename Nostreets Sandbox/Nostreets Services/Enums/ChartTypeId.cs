using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Enums
{

    public enum ChartType : int
    {
        Line = 1,
        Bar = 2,
        Pie = 3
    }

    public enum ChartLibraryType : int
    {
        Chartist = 1,
        PivotTable = 2
    }
}