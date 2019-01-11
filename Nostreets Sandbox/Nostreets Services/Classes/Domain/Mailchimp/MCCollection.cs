using Nostreets_Services.Classes.Domain.Mailchimp.Lists;
using Nostreets_Services.Classes.Domain.Mailchimp.Members;
using System.Collections.Generic;
using System.Linq;

namespace Nostreets_Services.Classes.Domain.Mailchimp
{
    public abstract class MCCollection<T>
    {
        public int Total_Items { get; set; }
        public List<Link> _Links { get; set; }

    }
    public class ListCollection : MCCollection<MailChimpList>
    {
        public List<MailChimpList> Lists { get; set; }
    }

    public class MemberCollection : MCCollection<Member>
    {
        public List<Member> Members { get; set; }
    }

}
