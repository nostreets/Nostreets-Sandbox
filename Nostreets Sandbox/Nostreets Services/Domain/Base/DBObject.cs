using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Base
{
    public partial class DBObject
    {
        [Required]
        public virtual string UserId { get; set; }

        [Required]
        public virtual string Name { get; set; }
    }
}
