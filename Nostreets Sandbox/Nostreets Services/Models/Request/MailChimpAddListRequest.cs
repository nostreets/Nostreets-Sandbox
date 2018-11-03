using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Nostreets_Services.Domain.Newsletter.Lists;

namespace Nostreets_Services.Models.Requests
{
	public class MailChimpAddListRequest
	{
		[Required]
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("contact")]
		[Required]
		public Contact Contact { get; set; }


		[JsonProperty("permission_reminder")]
		[Required]
		public string PermissionReminder { get; set; }


		[JsonProperty("campaign_defaults")]
		[Required]
		public CampaignDefaults CampaignDefaults { get; set; }

		[JsonProperty("email_type_option")]
		[Required]
		public bool EmailTypeOption { get; set; }

	}
}




