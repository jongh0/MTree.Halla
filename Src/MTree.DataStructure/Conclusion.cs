﻿using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
    public enum MarketTimeTypes
    {
        Unknown,
        Normal,
        NormalExpect,
        BeforeOffTheClock,
        BeforeExpect,
        AfterOffTheClock,
        AfterExpect,
    }
    #endregion

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(200, typeof(StockConclusion))]
    [ProtoInclude(201, typeof(IndexConclusion))]
    [Serializable]
    public class Conclusion : Subscribable
    {
        [BsonElement("A")]
        public long Amount { get; set; }

        [BsonElement("MTT")]
        public MarketTimeTypes MarketTimeType { get; set; }

        [BsonElement("P")]
        public float Price { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(Conclusion).GetProperties())
                {
                    sb.AppendLine($"{property.Name}: {property.GetValue(this)}");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
