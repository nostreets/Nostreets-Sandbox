using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Domain.Charts
{
    public class Chart
    {
        public int ChartId { get; set; }
        public ChartTypes TypeId { get; set; }
        public string Name { get; set; }
        public List<List<int>> Series { get; set; }
        public List<string> Legend { get; set; }
        public List<string> Labels { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public Dictionary<string, List<int>> GetSeriesWithKey()
        {
            Dictionary<string, List<int>> dic = new Dictionary<string, List<int>>();
            for (int i = 0; i < Series.Count; i++)
            {
                dic.Add(Legend[i], Series[i]);
            }
            return dic;
        }
    }


    public class Chart<T> : Chart
    {
        public new List<T> Series { get; set; }
    }
}