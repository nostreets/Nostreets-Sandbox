using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostreets_Services.Domain.Newsletter.Members
{
    public class Member
    {
        public string Id { get; set; }
        [JsonProperty("email_address")]
        public string EmailAddress { get; set; }
        [JsonProperty("unique_email_id")]
        public string UniqueEmailId { get; set; }
        [JsonProperty("email_type")]
        public string EmailType { get; set; }
        public string Status { get; set; }
        [JsonProperty("status_if_new")]
        public string StatusIfNew { get; set; }
        [JsonProperty("merge_fields")]
        public Dictionary<string, string> MergeFields { get; set; }
        public List<GroupInterest> Interests { get; set; }
        public MemberStats Stats { get; set; }
        [JsonProperty("ip_signup")]
        public string IpSignup { get; set; }
        [JsonProperty("timestamp_signup")]
        public DateTime? TimestampSignup { get; set; }
        [JsonProperty("ip_opt")]
        public string IpOpt { get; set; }
        [JsonProperty("timestamp_opt")]
        public DateTime? TimestampOpt { get; set; }
        [JsonProperty("member_rating")]
        public int MemberRating { get; set; }
        [JsonProperty("last_changed")]
        public DateTime? LastChanged { get; set; }
        public string Language { get; set; }
        public bool Vip { get; set; }
        [JsonProperty("email_client")]
        public string EmailClient { get; set; }
        public Location Location { get; set; }
    }
}
