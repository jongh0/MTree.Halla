﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    //CpSvrNew7224S
    //0 - (string) 종목코드
    //1 - (long) 수신 시간(HHMMSS)
    //2 - (long) 현재가
    //3 - (char) 대비부호
    //4 - (long) 대비
    //5 - (long) 거래량
    //6 - (long) NAV 지수
    //7 - (char) NAV대비부호
    //8 - (long) NAV 대비
    //9 - (char) 추적오차율부호
    //10 - (long) 추적오차율
    //11 - (char) 괴리율부호
    //12 - (long) 괴리율
    //13 - (char) 해당 ETF 지수 대비 부호
    //14 - (long) 해당 ETF 지수 대비
    //15 - (long) 해당 ETF 지수

    // char 부호 + long 비율 => double로 통합

    [DataContract]
    public class ETFConclusion : Subscribable
    {
        [BsonElement("P")]
        [DataMember(Name = "P")]
        public float Price { get; set; }

        [BsonElement("A")]
        [DataMember(Name = "A")]
        public long Amount { get; set; }

        [BsonElement("Co")]
        [DataMember(Name = "Co")]
        public double Comparision { get; set; }

        [BsonElement("NI")]
        [DataMember(Name = "NI")]
        public long NAVIndex { get; set; }

        [BsonElement("NC")]
        [DataMember(Name = "NC")]
        public double NAVComparision { get; set; }

        [BsonElement("TER")]
        [DataMember(Name = "TER")]
        public double TracingErrorRate { get; set; }

        [BsonElement("DR")]
        [DataMember(Name = "DR")]
        public double DisparateRatio { get; set; }

        [BsonElement("EIC")]
        [DataMember(Name = "EIC")]
        public double ReferenceIndexComparision { get; set; }

        [BsonElement("EI")]
        [DataMember(Name = "EI")]
        public float ReferenceIndex { get; set; }
    }
}
