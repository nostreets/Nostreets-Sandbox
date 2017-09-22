using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Bills
{
    public class Expenses
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]

        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public ExpenseTypes ExpenseType { get; set; }

        [Required]
        public ScheduleTypes PaySchedule { get; set; }

        [Required]
        public DateTime TimePaid { get; set; }

        [NotMapped]
        public DateTime NextPayDay { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public bool IsHiddenOnChart { get; set; }

    }
}
