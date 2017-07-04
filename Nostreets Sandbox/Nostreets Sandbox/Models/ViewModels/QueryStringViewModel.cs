using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Sandbox.Models.ViewModels
{
    public class QueryStringViewModel : BaseViewModel
    {

        public QueryStringViewModel(Dictionary<string, string> queryString)
        {
            Query = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> i in queryString)
            {
                Query.Add(i.Key, i.Value); 
            }
        }
        public Dictionary<string, string> Query { get; set; }
    }
}