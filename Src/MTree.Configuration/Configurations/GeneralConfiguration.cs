﻿using Newtonsoft.Json;
using System;

namespace MTree.Configuration
{
    public class GeneralConfiguration
    {
        [JsonIgnore]
        public string DateFormat { get; } = "yyyy-MM-dd";

        [JsonIgnore]
        public string TimeFormat { get; } = "HH:mm:ss.fff";

        [JsonIgnore]
        public string DateTimeFormat { get; } = "yyyy-MM-dd HH:mm:ss.fff";

        [JsonIgnore]
        public string CurrencyFormat { get; } = "#,###.##";

        [JsonIgnore]
        public string PercentFormat { get; } = "P2";

        [JsonIgnore]
        public DateTime DefaultStartDate { get; } = new DateTime(1999, 1, 1);

        public bool OfflineMode { get; set; } = false;

        public bool TestMode { get; set; } = false;

        public bool SkipMastering { get; set; } = false;

        public bool SkipBiddingPrice { get; set; } = false;
    }
}
