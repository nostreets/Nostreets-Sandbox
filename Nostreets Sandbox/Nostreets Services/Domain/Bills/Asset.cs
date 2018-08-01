using Nostreets_Services.Enums;
using NostreetsExtensions.DataControl.Classes;
using System;
using System.ComponentModel.DataAnnotations;

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

        public string Style { get; set; }

        private float _rateMultilplier = 1;

    }
}
