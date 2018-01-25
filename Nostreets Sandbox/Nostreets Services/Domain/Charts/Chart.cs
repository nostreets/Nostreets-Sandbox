using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostreets_Services.Domain.Charts
{
    public class Chart<T>
    {
        public int ChartId { get; set; }
        public ChartType TypeId { get; set; }
        public string Name { get; set; }
        public List<T> Series { get; set; }
        public List<string> Legend { get; set; }
        public List<string> Labels { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string UserId { get; set; }

        public Dictionary<string, List<T>> GetSeriesWithKey()
        {
            Dictionary<string, List<T>> dic = new Dictionary<string, List<T>>();
            for (int i = 0; i < Series.Count; i++)
            {
                dic.Add(Legend[i], Series);
            }
            return dic;
        }
    }



}