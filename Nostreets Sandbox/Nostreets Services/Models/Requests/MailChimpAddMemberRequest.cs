using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Models.Requests
{
    public class MailChimpAddMemberRequest
    {
        Dictionary<string, string> _mergeFields;

        [JsonProperty("email_address")]
        [Required]
        public string Email_Address { get; set; }
        [JsonProperty("status")]
        [Required]
        public string Status { get; set; }
        [JsonProperty("merge_fields")]
        public Dictionary<string, string> Merge_Fields
        {
            get { return _mergeFields; }
            set
            {
                if (value["FNAME"] == null) { value.Add("FNAME", "N/A"); }
                if (value["LNAME"] == null) { value.Add("LNAME", "N/A"); }
                _mergeFields = value;
            }
        }
    }

}