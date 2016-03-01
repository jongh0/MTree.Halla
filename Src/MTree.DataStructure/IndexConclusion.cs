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
        public double Index { get; set; }

        public double Volume { get; set; }

        public double Value { get; set; }

        public int UpperLimitedItemCount { get; set; }

        public int IncreasingItemCount { get; set; }

        public int SteadyItemCount { get; set; }

        public int DecreasingItemCount { get; set; }

        public int LowerLimitedItemCount { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}
