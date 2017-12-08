using Nostreets_Services.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Nostreets_Services.Domain.Cards
{
    public class StyledCard
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public ContentType HeaderType { get; set; }
        public AlignmentType HeaderAlignment { get; set; }
        public ContentType MainType { get; set; }
        public AlignmentType MainAlignment { get; set; }
        public ContentType FooterType { get; set; }
        public AlignmentType FooterAlignment { get; set; }
        public string _HTML { get; set; }

        public Newsletter.Lists.MailChimpList TestTable { get; set; }
    }
}
