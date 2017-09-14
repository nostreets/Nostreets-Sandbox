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
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int PayRate { get; set; }

        [Required]
        public ScheduleTypes PaySchedule { get; set; }

        [Required]
        public IncomeTypes Type { get; set; }

        public DateTime TimePaid { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

    }

}
