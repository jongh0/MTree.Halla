using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
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

    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class ETFConclusion : Subscribable
    {
        [BsonElement("P")]
        public float Price { get; set; }

        [BsonElement("A")]
        public long Amount { get; set; }

        [BsonElement("Co")]
        public double Comparision { get; set; }

        [BsonElement("NI")]
        public long NAVIndex { get; set; }

        [BsonElement("NC")]
        public double NAVComparision { get; set; }

        [BsonElement("TER")]
        public double TracingErrorRate { get; set; }

        [BsonElement("DR")]
        public double DisparateRatio { get; set; }

        [BsonElement("EIC")]
        public double ReferenceIndexComparision { get; set; }

        [BsonElement("EI")]
        public float ReferenceIndex { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(GetType(), excludeProperties);
        }

    }
}
