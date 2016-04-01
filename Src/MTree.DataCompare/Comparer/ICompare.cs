using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataCompare
{
    public interface ICompare
    {
        string ResultOutput { get; set; }

        bool DoCompareItem(List<Subscribable> source, List<Subscribable> dest);

        bool DoCompareItem(Subscribable source, Subscribable dest);
    }
}
