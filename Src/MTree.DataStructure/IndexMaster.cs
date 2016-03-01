using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class IndexMaster : Subscribable
    {
        public string Name { get; set; }

        public double PreviousClosedPrice { get; set; }

        public long PreviousVolume { get; set; }

        public long PreviousTradeCost { get; set; }
    }
}
