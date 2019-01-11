using System;
using System.Linq;

namespace Nostreets_Services.Classes.Domain.Mailchimp.Lists
{
    public class CampaignDefaults
    {
        public string From_Name { get; set; }
        public string From_Email { get; set; }
        public string Subject { get; set; }
        public string Language { get; set; }
    }
}