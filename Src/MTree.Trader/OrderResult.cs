using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class OrderResult
    {
        public string Code { get; set; }

        public int ConcludedAmount { get; set; }

        public int ConcludedPrice { get; set; }

        public int OrderedAmount { get; set; }

        public int OrderedPrice { get; set; }

        public ulong OrderNumber { get; set; }

        public OrderTypes OrderType { get; set; }
    }
}
