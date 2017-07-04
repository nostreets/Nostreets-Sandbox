using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Domain.Newsletter
{
    public class Link
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Method { get; set; }
        public string TargetSchema { get; set; }
        public string Schema { get; set; }
    }
}