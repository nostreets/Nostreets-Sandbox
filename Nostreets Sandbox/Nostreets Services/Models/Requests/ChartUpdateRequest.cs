using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nostreets_Services.Models.Request
{
    public class ChartUpdateRequest
    {
        [Required]
        public int ChartId { get; set; }
        public ChartTypes TypeId { get; set; }
        public string Name { get; set; }
        public List<List<int>> Series { get; set; }
        public List<string> Legend { get; set; }
        public List<string> Labels { get; set; }

    }

    public class ChartUpdateRequest<T> : ChartUpdateRequest
    {
        public new List<T> Series { get; set; }
    }
}