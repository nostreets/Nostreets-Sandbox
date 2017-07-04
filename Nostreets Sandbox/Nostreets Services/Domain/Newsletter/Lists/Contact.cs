using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Domain.Newsletter.Lists
{
    public class Contact
    {
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
    }
}