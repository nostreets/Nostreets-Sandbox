using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Models.Requests
{
    public class ExpensesAddRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public ExpenseType BillType { get; set; }

        [Required]

        public ScheduleTypes PaySchedule { get; set; }

    }

    public class IncomeAddRequest
    {

        [Required]
        public string UserId { get; set; }

        [Required]
        public int PayRate { get; set; }

        [Required]
        public ScheduleTypes PaySchedule { get; set; }

        [Required]
        public IncomeType Type { get; set; }

    }

    public class ExpensesUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public ExpenseType BillType { get; set; }

        [Required]

        public ScheduleTypes PaySchedule { get; set; }

    }

    public class IncomeUpdateRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int PayRate { get; set; }

        [Required]
        public ScheduleTypes PaySchedule { get; set; }

        [Required]
        public IncomeType Type { get; set; }

    }

}
