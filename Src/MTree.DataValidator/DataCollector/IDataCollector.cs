using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataValidator
{
    public interface IDataCollector
    {
        List<string> GetStockCodeList();

        List<string> GetIndexCodeList();
        
        StockMaster GetMaster(DateTime targetDate, string code);

        List<Subscribable> GetIndexConclusions(DateTime targetDate, string code, bool normalOnly = true);

        List<Subscribable> GetStockConclusions(DateTime targetDate, string code, bool normalOnly = true);

        List<Subscribable> GetCircuitBreaks(DateTime targetDate, string code);
    }
}
