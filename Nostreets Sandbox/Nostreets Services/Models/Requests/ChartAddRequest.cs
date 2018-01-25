using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Models.Request
{
    public class ChartAddRequest
    {
        [Required]
        public ChartType TypeId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public List<List<int>> Series { get; set; }

        [Required]
        public List<string> Legend { get; set; }

        [Required]
        public List<string> Labels { get; set; }

        public string UserId { get; set; }
    }

    public class ChartAddRequest<T> : ChartAddRequest
    {
        public new List<T> Series { get; set; }
    }
}