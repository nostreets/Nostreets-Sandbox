using Nostreets_Services.Domain.Newsletter.Lists;
using System;
using System.Linq;

namespace Nostreets_Services.Domain.Newsletters
{
    public class UserListStats
    {
        public string UserId { get; set; }

        public string ListId { get; set; }

        public DateTime DateTaken { get; set; }

        public ListStats Stats { get; set; }

    }
}