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

        [JsonIgnore]
        public bool ExcludeEbest { get; } = false;

        [JsonIgnore]
        public bool ExcludeKiwoom { get; } = false;

        public bool SimulTrade { get; set; } = false;

        public bool VirtualTrade { get; set; } = false;

        public bool OfflineMode { get; set; } = false;

        public bool SkipMastering { get; set; } = false;

        public bool SkipBiddingPrice { get; set; } = false;
    }
}
