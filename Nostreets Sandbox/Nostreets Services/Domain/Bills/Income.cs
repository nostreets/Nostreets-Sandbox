using Nostreets_Services.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Nostreets_Services.Domain.Bills
{
    public class Income : Asset
    {
        //[Required]
        //public float PayRate { get; set; }

        [Required]
        public IncomeTypes IncomeType { get; set; }

    }

}
