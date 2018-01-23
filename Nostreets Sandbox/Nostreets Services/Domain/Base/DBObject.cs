using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Base
{
    public abstract partial class DBObject
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public virtual string UserId { get; set; }

        [Required]
        public virtual string Name { get; set; }
    }
}
