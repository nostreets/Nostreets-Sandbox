using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nostreets_Services.Domain.Newsletter.Lists
{
    public class MailChimpList
    {
        public int Id { get; set; }
        [JsonProperty("web_id")]
        public int WebId { get; set; }
        public string Name { get; set; }
        public Contact Contact { get; set; }
        [JsonProperty("permission_reminder")]
        public string PermissionReminder { get; set; }
        [JsonProperty("use_archive_bar")]
        public bool UseArchiveBar { get; set; }
        [JsonProperty("campaign_defaults")]
        public CampaignDefaults CampaignDefaults { get; set; }
        [JsonProperty("notify_on_subscribe")]
        public string NotifyOnSubscribe { get; set; }
        [JsonProperty("notify_on_unsubscribe")]
        public string NotifyOnUnsubscribe { get; set; }
        [JsonProperty("date_created")]
        public string DateCreated { get; set; }
        [JsonProperty("list_rating")]
        public int ListRating { get; set; }
        [JsonProperty("email_type_option")]
        public bool EmailTypeOption { get; set; }
        [JsonProperty("subscribe_url_short")]
        public string SubscribeUrlShort { get; set; }
        [JsonProperty("subscribe_url_long")]
        public string SubscribeUrlLong { get; set; }
        [JsonProperty("beamer_address")]
        public string BeamerAddress { get; set; }
        public string Visibility { get; set; }
        public List<object> Modules { get; set; }
        public ListStats Stats { get; set; }
        [JsonProperty("_links")]
        public List<Link> Links { get; set; }
    }
}