﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    #region enum
    public enum ConclusionType
    {
        Sell,
        Buy,
        None,
    } 
    #endregion

    [Serializable]
    public class StockConclusion : Conclusion
    {
        [BsonElement("CTy")]
        public ConclusionType ConclusionType { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(ConclusionType)}: {ConclusionType}");

            return sb.ToString();
        }
    }
}
