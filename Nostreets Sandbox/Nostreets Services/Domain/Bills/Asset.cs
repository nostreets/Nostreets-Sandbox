using Nostreets_Services.Domain.Base;
using Nostreets_Services.Enums;
using NostreetsExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Domain.Bills
{
    public abstract class FinicialAsset : DBObject
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public ScheduleTypes PaySchedule { get; set; }

        [Required]
        public DateTime TimePaid { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ExperationDate { get; set; }

        public bool IsHiddenOnChart { get; set; }

        public float Cost { get; set; }

        public ScheduleTypes Rate { get; set; }

        public float RateMultilplier { get => _rateMultilplier; set => _rateMultilplier = value; }

        public override string UserId { get => SessionManager.Get<bool>(SessionState.IsLoggedOn) ? SessionManager.Get<string>(SessionState.UserId) : null; }
        
        public string Style { get; set; }

        private float _rateMultilplier = 1;

    }
}
