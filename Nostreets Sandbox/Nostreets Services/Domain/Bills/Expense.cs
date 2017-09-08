using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Bills
{
    public class Expenses
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public BillTypes BillType { get; set; }

        public ScheduleTypes PaySchedule { get; set; }

    }
}
