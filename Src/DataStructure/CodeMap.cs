using MongoDB.Bson.Serialization.Attributes;
using DataStructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace DataStructure
{
    public interface ICodeMap
    {
        string Name { get; set; }
    }

    public class CodeMapHead : ICodeMap
    {
        [BsonElement("CML")]
        public List<ICodeMap> CodeMapList { get; set; }

        [BsonElement("N")]
        public string Name { get; set; }

        public CodeMapHead(string name = "")
        {
            Name = name;
            CodeMapList = new List<ICodeMap>();
        }

        public void Add(ICodeMap codeMap)
        {
            CodeMapList.Add(codeMap);
        }
    }
}
