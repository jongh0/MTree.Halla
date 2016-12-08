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
    public class CodeMapDbObject : Subscribable
    {
        public string CodeMap { get; set; }
    }
}
