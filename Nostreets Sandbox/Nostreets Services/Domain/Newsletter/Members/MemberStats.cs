﻿using Newtonsoft.Json;
using System;
using System.Linq;

namespace Nostreets_Services.Domain.Newsletter.Members
{
    public class MemberStats
    {
        [JsonProperty("avg_open_rate")]
        public int AvgOpenRate { get; set; }
        [JsonProperty("avg_click_rate")]
        public int AvgClickRate { get; set; }
    }
}