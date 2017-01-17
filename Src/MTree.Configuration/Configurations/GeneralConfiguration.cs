using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace MTree.Configuration
{
    public enum TraderTypes
    {
        Virtual,
        Ebest,
        EbestSimul,
        Kiwoom,
        KiwoomSimul,
    }

    public class GeneralConfiguration
    {
        [JsonIgnore]
        public string DateFormat { get; } = "yyyy-MM-dd";

        [JsonIgnore]
        public string TimeFormat { get; } = "HH:mm:ss.fff";

        [JsonIgnore]
        public string DateTimeFormat { get; } = "yyyy-MM-dd HH:mm:ss.ffff";

        [JsonIgnore]
        public string DateNow { get { return DateTime.Now.ToString(DateFormat); } }

        [JsonIgnore]
        public string TimeNow { get { return DateTime.Now.ToString(TimeFormat); } }

        [JsonIgnore]
        public string DateTimeNow { get { return DateTime.Now.ToString(DateTimeFormat); } }

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

        [JsonConverter(typeof(StringEnumConverter))]
        public TraderTypes TraderType { get; set; } = TraderTypes.Ebest;

        public bool OfflineMode { get; set; } = false;

        public bool SkipMastering { get; set; } = false;

        public bool SkipCodeBuilding { get; set; } = true;

        public bool SkipBiddingPrice { get; set; } = false;

        public bool SkipETFConclusion { get; set; } = false;

        public bool VerifyOrdering { get; set; } = false;

        public bool VerifyLatency { get; set; } = false;

        public bool VerifyEnqueueLatency { get; set; } = false;

        public bool LaunchDashboard { get; set; } = true;

        public bool LaunchHistorySaver { get; set; } = true;

        public bool KeepAliveDashboardAfterMarketEnd { get; set; } = true;

        public bool UseShutdown { get; set; } = false;

        public int ShutdownHour { get; set; } = 24;

        // 0보다 큰 값일 때만 동작한다.
        public int LogFolderKeepDays { get; set; } = 2;

        // 0보다 큰 값일 때만 동작한다.
        public int LogZipFileKeepDays { get; set; } = 15;
    }
}
