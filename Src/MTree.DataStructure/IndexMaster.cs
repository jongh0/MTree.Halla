﻿using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class IndexMaster : Subscribable
    {
        [BsonElement("N")]
        public string Name { get; set; }

        [BsonElement("BP")]
        public float BasisPrice { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(IndexMaster), excludeProperties);
        }
    }
}
