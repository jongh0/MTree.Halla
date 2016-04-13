using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataCompare
{
    public class DataValidator
    {
        private BeyondCompare comparator { get; set; } = new BeyondCompare();

        private IDataCollector source = new DbCollector(DbAgent.Instance);
        private IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);

        private const string compareResultPath = "CompareResult";
        private const string codeCompareResultFile = "CodeCompare.txt";
        public DataValidator()
        {
        }

        public bool ValidateCodeList()
        {
            List<string> srcCodes = source.GetCodeList();
            List<string> destCodes = destination.GetCodeList();
            bool result = comparator.DoCompareItem(srcCodes, destCodes, true);
            comparator.MakeReport(srcCodes, destCodes, Path.Combine(compareResultPath, codeCompareResultFile));
            return result;
        }

    }
}
