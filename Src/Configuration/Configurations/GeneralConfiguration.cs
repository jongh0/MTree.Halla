using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Configuration
{
    public class GeneralConfiguration
    {
        [JsonIgnore]
        public string DateFormat => "yyyy-MM-dd";

        [JsonIgnore]
        public string TimeFormat => "HH:mm:ss.fff";

        [JsonIgnore]
        public string DateTimeFormat => "yyyy-MM-dd HH:mm:ss.ffff";

        [JsonIgnore]
        public string DateNow => DateTime.Now.ToString(DateFormat);

        [JsonIgnore]
        public string TimeNow => DateTime.Now.ToString(TimeFormat);

        [JsonIgnore]
        public string DateTimeNow => DateTime.Now.ToString(DateTimeFormat);

        [JsonIgnore]
        public string CurrencyFormat => "#,###.##";

        [JsonIgnore]
        public string PercentFormat => "P2";

        [JsonIgnore]
        public DateTime DefaultStartDate { get; } = new DateTime(1999, 1, 1);

        public bool ExcludeEbest { get; set; } = false;

        public bool ExcludeKiwoom { get; set; } = true;

        public bool LaunchStrategyManager { get; set; } = true;

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

        [JsonIgnore]
        public string AccountPw
        {
            get
            {
                switch (Config.General.TraderType)
                {
                    case TraderTypes.Ebest:
                        return Config.Ebest.AccountPw;

                    case TraderTypes.EbestSimul:
                        return "0000";

                    case TraderTypes.Kiwoom:
                    case TraderTypes.KiwoomSimul:
                        return Config.Kiwoom.AccountPw;

                    default:
                        return string.Empty;
                }
            }
        }
    }
}
