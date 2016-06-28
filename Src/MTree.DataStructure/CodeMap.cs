﻿using MTree.DataStructure;
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

    public class CodeMapHead : ICodeMap
    {
        public List<ICodeMap> CodeMapList { get; set; }

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
