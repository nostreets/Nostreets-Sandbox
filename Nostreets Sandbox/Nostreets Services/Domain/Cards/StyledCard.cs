using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Cards
{
    public class StyledCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CardSize Size { get; set; }
        public ContentType HeaderType { get; set; }
        public AlignmentType HeaderAlignment { get; set; }
        public ContentType MainType { get; set; }
        public AlignmentType MainAlignment { get; set; }
        public ContentType FooterType { get; set; }
        public AlignmentType FooterAlignment { get; set; }

        public string Html { get; set; }

    }
}
