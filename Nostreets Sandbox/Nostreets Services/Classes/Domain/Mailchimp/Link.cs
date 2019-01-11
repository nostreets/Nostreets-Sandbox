﻿using System;
using System.Linq;

namespace Nostreets_Services.Classes.Domain.Mailchimp
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