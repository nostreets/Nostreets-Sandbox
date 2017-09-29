using Nostreets_Services.Domain.Base;
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
    public class Asset : DBObject
    {
        [Key]
        [Required]
        public int Id { get; set; }



        [Required]
        public ScheduleTypes PaySchedule { get; set; }

        [Required]
        public DateTime? TimePaid { get; set; }

        [NotMapped]
        public DateTime? NextPayDay { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public bool IsHiddenOnChart { get; set; }



    }
}
