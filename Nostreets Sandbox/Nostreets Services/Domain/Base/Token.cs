using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Base
{
    public class Token : DBObject<string>
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime ExpirationDate { get; set; }

        public TokenType Type { get; set; }
    }
}
