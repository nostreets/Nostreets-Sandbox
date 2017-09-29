using Nostreets_Services.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Nostreets_Services.Domain.Bills
{
    public class Expenses : Asset
    {
        [Required]
        public ExpenseTypes ExpenseType { get; set; }

        [Required]
        public decimal Price { get; set; }

    }
}
