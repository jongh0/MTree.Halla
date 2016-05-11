using DSCBO1Lib;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataValidator
{
    public class DaishinCollector: IDataCollector
    {
        private StockBidClass ConclusionHistoryQueryObj;

        public DaishinCollector()
        {
            ConclusionHistoryQueryObj = new StockBidClass();
        }

        public List<string> GetStockCodeList()
        {
            throw new NotImplementedException();
        }

        public List<string> GetIndexCodeList()
        {
            throw new NotImplementedException();
        }

        public StockMaster GetMaster(DateTime targetDate, string code)
        {
            throw new NotImplementedException();
        }

        public List<Subscribable> GetIndexConclusions(DateTime targetDate, string code, bool normalOnly = true)
        {
            throw new NotImplementedException();
        }

        public List<Subscribable> GetStockConclusions(DateTime targetDate, string code, bool normalOnly = true)
        {
            Stack<StockConclusion> conclusions = new Stack<StockConclusion>();
            List<Subscribable> ret = new List<Subscribable>();

            ConclusionHistoryQueryObj.SetInputValue(0, "A" + code);
            ConclusionHistoryQueryObj.SetInputValue(2, 80);
            ConclusionHistoryQueryObj.SetInputValue(3, 'C');
            ConclusionHistoryQueryObj.SetInputValue(4, "1900");
            
            do
            {
                ConclusionHistoryQueryObj.BlockRequest();
                int count = Convert.ToInt32(ConclusionHistoryQueryObj.GetHeaderValue(2));
                for (int i = 0; i < count; i++)
                {
                    char timeType = Convert.ToChar(ConclusionHistoryQueryObj.GetDataValue(10, i));
                    if (timeType == '1') continue;

                    StockConclusion conclusion = new StockConclusion();
                    conclusion.Code = ((string)ConclusionHistoryQueryObj.GetHeaderValue(0)).Substring(1);
                    conclusion.Price = Convert.ToSingle(ConclusionHistoryQueryObj.GetDataValue(4, i));
                    conclusion.Amount = Convert.ToInt64(ConclusionHistoryQueryObj.GetDataValue(6, i));
                    
                    char concludeType = Convert.ToChar(ConclusionHistoryQueryObj.GetDataValue(7, i));
                    if (concludeType == '1') conclusion.ConclusionType = ConclusionTypes.Buy;
                    else if (concludeType == '2') conclusion.ConclusionType = ConclusionTypes.Sell;

                    int concludedTime = Convert.ToInt32(ConclusionHistoryQueryObj.GetDataValue(9, i));
                    conclusion.Time = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day,
                        (int)concludedTime / 10000, (int)(concludedTime / 100) % 100, (int)concludedTime % 100);
                    
                    if (conclusion.Time < new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 8, 50, 0))
                        conclusion.MarketTimeType = MarketTimeTypes.BeforeOffTheClock;
                    else
                        conclusion.MarketTimeType = MarketTimeTypes.Normal;

                    conclusions.Push(conclusion);
                }
            } while (ConclusionHistoryQueryObj.Continue == 1);

            while (conclusions.Count > 0)
            {
                StockConclusion c = conclusions.Pop();
                if (normalOnly == true && c.MarketTimeType == MarketTimeTypes.Normal &&
                    c.Time < new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 15, 00, 0))
                    ret.Add(c);
            }

            return ret;
        }

        public List<Subscribable> GetCircuitBreaks(DateTime targetDatex, string code)
        {
            throw new NotImplementedException();
        }
    }
}
