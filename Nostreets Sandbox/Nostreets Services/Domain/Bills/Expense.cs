using Nostreets_Services.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Nostreets_Services.Domain.Bills
{
    public class Expense : Asset
    {
        [Required]
        public ExpenseType ExpenseType { get; set; }

    }
}
