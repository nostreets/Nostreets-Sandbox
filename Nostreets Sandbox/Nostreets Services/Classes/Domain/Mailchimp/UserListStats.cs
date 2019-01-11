using Nostreets_Services.Classes.Domain.Mailchimp.Lists;
using System;
using System.Linq;

namespace Nostreets_Services.Classes.Domain.Mailchimps
{
    public class UserListStats
    {
        public string UserId { get; set; }

        public string ListId { get; set; }

        public DateTime DateTaken { get; set; }

        public ListStats Stats { get; set; }

    }
}