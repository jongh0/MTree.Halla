﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTree.Configuration
{
    public enum ServerTypes
    {
        Real,
        Simul,
    }

    public class EbestConfiguration
    {
        public string UserId { get; set; } = string.Empty;

        public string UserPw { get; set; } = string.Empty;

        public string CertPw { get; set; } = string.Empty;

        public string AccountPw { get; set; } = string.Empty;

        [JsonIgnore]
        public string RealServerAddress { get { return "Hts.etrade.co.kr"; } }

        [JsonIgnore]
        public string SimulServerAddress { get { return "demo.etrade.co.kr"; } }

        [JsonIgnore]
        public int ServerPort { get; } = 20001;
    }
}
