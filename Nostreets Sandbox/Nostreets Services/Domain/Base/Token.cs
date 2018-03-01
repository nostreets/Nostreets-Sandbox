using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Base
{
    public class Token
    {
        public string UserId { get; set; }

        public Guid Value { get; set; }

        public DateTime ExpirationDate { get; set; }

        public bool IsDisabled { get; set; }
    }
}
