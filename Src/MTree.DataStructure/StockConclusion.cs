﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
    public enum ConclusionTypes
    {
        Unknown,
        Sell,
        Buy,
    } 
    #endregion

    [Serializable]
    public class StockConclusion : Conclusion
    {
        [BsonElement("CT")]
        public ConclusionTypes ConclusionType { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(StockConclusion).GetProperties())
                {
                    sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
        public override string ToString(bool excludeId = true, bool excludeRxTime = true)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(StockConclusion).GetProperties())
                {
                    if (excludeId == true && property.Name != "Id")
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                    else if (excludeRxTime == true && property.Name != "ReceivedTime")
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
