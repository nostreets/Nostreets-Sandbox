using Nostreets_Services.Enums;
using NostreetsExtensions.DataControl.Classes;
using System;
using System.Collections.Generic;

namespace Nostreets_Services.Classes.Domain.Charts
{
    public class Chart<T> : DBObject
    {
        public int ChartId { get; set; }
        public ChartType TypeId { get; set; }
        public string Name { get; set; }
        public List<T> Series { get; set; }
        public List<string> Legend { get; set; }
        public List<string> Labels { get; set; }

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