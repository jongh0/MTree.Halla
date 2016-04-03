using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataCompare
{
    public interface IDataCollector
    {
        List<string> GetCodeList();
        
        StockMaster GetMaster(string code, DateTime targetDate);

        List<Subscribable> GetStockConclusions(string code, DateTime targetDate, bool normalOnly = true);
    }
}
