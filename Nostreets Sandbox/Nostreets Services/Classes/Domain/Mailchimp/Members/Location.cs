using Newtonsoft.Json;
using System;
using System.Linq;

namespace Nostreets_Services.Classes.Domain.Mailchimp.Members
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [JsonProperty("gmtoff")]
        public int GmtOff { get; set; }
        [JsonProperty("dstoff")]
        public int DstOff { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        public string Timezone { get; set; }
    }
}