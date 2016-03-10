using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public class StockMastering
    {
        public bool AllDone => DaishinDone && EbestDone;
        public bool DaishinDone { get; set; } = false;
        public bool EbestDone { get; set; } = false;

        public StockMaster Stock { get; set; } = new StockMaster();
    }
}
