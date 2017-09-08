using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Bills
{
    public class Income
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        public int PayRate { get; set; }

        public ScheduleTypes PaySchedule { get; set; }

        public IncomeTypes Type { get; set; }

    }

}
