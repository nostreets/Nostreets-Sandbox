using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Domain.Newsletter.Lists
{
    public class ListStats
    {
        public int Member_Count { get; set; }
        public int Unsubscribe_Count { get; set; }
        public int Cleaned_Count { get; set; }
        public int Member_Count_Since_Send { get; set; }
        public int Unsubscribe_Count_Since_Send { get; set; }
        public int Cleaned_Count_Since_Send { get; set; }
        public int Campaign_Count { get; set; }
        public string Campaign_Last_Sent { get; set; }
        public int Merge_Field_Count { get; set; }
        public int Avg_Sub_Rate { get; set; }
        public int Avg_Unsub_Rate { get; set; }
        public int Target_Sub_Rate { get; set; }
        public int Open_Rate { get; set; }
        public int Click_Rate { get; set; }
        public string Last_Sub_Date { get; set; }
        public string Last_Unsub_Date { get; set; }
    }

}