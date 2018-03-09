using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Base
{
    public class Token : DBObject
    {
        public string Name { get; set; }

        public Guid Value { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsDisabled { get; set; }

        public TokenType Type { get; set; }
    }
}
