using MongoDB.Bson.Serialization.Attributes;
using MTree.DataStructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    public interface ICodeMap
    {
        string Name { get; set; }
    }

    [Serializable]
    public class CodeMapHead : ICodeMap
    {
        [BsonElement("CL")]
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
