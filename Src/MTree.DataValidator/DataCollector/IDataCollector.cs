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
        
        StockMaster GetMaster(string code, DateTime targetDate);

        List<Subscribable> GetIndexConclusions(string code, DateTime targetDate, bool normalOnly = true);

        List<Subscribable> GetStockConclusions(string code, DateTime targetDate, bool normalOnly = true);

        List<Subscribable> GetCircuitBreaks(string code, DateTime targetDatex);
    }
}
