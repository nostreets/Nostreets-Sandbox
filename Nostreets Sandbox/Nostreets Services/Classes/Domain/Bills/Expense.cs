using Nostreets_Services.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Nostreets_Services.Classes.Domain.Bills
{
    public class Expense : FinicialAsset
    {
        [Required]
        public ExpenseType ExpenseType { get; set; }

    }
}
