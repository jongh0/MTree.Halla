﻿using MongoDB.Bson.Serialization.Attributes;
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

    [Serializable]
    public class Conclusion : Subscribable
    {
        [BsonElement("A")]
        public long Amount { get; set; }

        [BsonElement("MTT")]
        public MarketTimeTypes MarketTimeType { get; set; }

        [BsonElement("P")]
        public float Price { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(Conclusion).GetProperties())
                {
                    if (excludeProperties.Contains(property.Name) == false)
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
