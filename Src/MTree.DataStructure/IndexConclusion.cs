﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class IndexConclusion : Conclusion
    {
        [BsonElement("MC")]
        public long MarketCapitalization { get; set; }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(IndexConclusion).GetProperties())
                {
                    sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
        public override string ToString(bool excludeId = true)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (excludeId == true)
                {
                    foreach (var property in typeof(IndexConclusion).GetProperties())
                    {
                        if (property.Name != "Id")
                            sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                    }
                }
                else
                {
                    return ToString();
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
