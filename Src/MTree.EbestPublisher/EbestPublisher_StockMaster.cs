using System;
using System.Threading;
using MTree.DataStructure;
using MTree.Configuration;

namespace MTree.EbestPublisher
{
    public partial class EbestPublisher
    {
        public override StockMaster GetStockMaster(string code)
        {
            var stockMaster = new StockMaster();
            stockMaster.Code = code;

            if (GetQuote(code, ref stockMaster) == false)
                stockMaster.Code = string.Empty;

            return stockMaster;
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            try
            {
                QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1102");
                WaitQuoteInterval();

                logger.Info($"Start quoting, Code: {code}");
                QuotingStockMaster = stockMaster;
                QuotingStockMaster.Code = code;

                stockQuotingObj.SetFieldData("t1102InBlock", "shcode", 0, code);
                var ret = stockQuotingObj.Request(false);
                if (ret < 0)
                {
                    logger.Error($"Quoting request error, {GetLastErrorMessage(ret)}");
                    return false;
                }

                if (WaitQuoting() == false)
                    return false;

                if (QuotingStockMaster.Code != string.Empty)
                {
                    logger.Info($"Quoting done, Code: {code}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(QuoteLock);
            }

            logger.Error($"Quoting fail, Code: {code}");
            return false;
        }

        private void StockQuotingObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");

                if (QuotingStockMaster == null)
                    return;

                string cvStr = stockQuotingObj.GetFieldData("t1102OutBlock", "abscnt", 0);
                long cv = 0;
                if (long.TryParse(cvStr, out cv) == false)
                    logger.Error($"Stock master circulating volume error, {cvStr}");

                //유통주식수
                QuotingStockMaster.CirculatingVolume = cv * 1000;  

                // 상장일
                string listDateStr = stockQuotingObj.GetFieldData("t1102OutBlock", "listdate", 0); 
                int listDate = 0;
                if (int.TryParse(listDateStr, out listDate) == true)
                    QuotingStockMaster.ListedDate = new DateTime(listDate / 10000, listDate / 100 % 100, listDate % 100);
                else
                    QuotingStockMaster.ListedDate = Config.General.DefaultStartDate;

                string valueAltered = stockQuotingObj.GetFieldData("t1102OutBlock", "info1", 0);
                if (valueAltered == "권배락")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExRightDividend;
                else if (valueAltered == "권리락")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExRight;
                else if (valueAltered == "배당락")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExDividend;
                else if (valueAltered == "액면분할")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.SplitFaceValue;
                else if (valueAltered == "액면병합")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.MergeFaceValue;
                else if (valueAltered == "주식병합")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.Consolidation;
                else if (valueAltered == "기업분할")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.Divestiture;
                else if (valueAltered == "감자")
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.CapitalReduction;
                else
                    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.None;

                string suspended = stockQuotingObj.GetFieldData("t1102OutBlock", "info3", 0);
                QuotingStockMaster.TradingSuspend = (suspended == "suspended");

                // 관리
                QuotingStockMaster.AdministrativeIssue = WarningList[nameof(WarningTypes1.AdministrativeIssue)].Contains(QuotingStockMaster.Code);

                // 불성실공시
                QuotingStockMaster.UnfairAnnouncement = WarningList[nameof(WarningTypes1.UnfairAnnouncement)].Contains(QuotingStockMaster.Code);

                // 투자유의
                QuotingStockMaster.InvestAttention = WarningList[nameof(WarningTypes1.InvestAttention)].Contains(QuotingStockMaster.Code);

                // 투자환기
                QuotingStockMaster.CallingAttention = WarningList[nameof(WarningTypes1.CallingAttention)].Contains(QuotingStockMaster.Code);

                // 경고
                QuotingStockMaster.InvestWarning = WarningList[nameof(WarningTypes2.InvestWarning)].Contains(QuotingStockMaster.Code);

                // 매매정지
                QuotingStockMaster.TradingHalt = WarningList[nameof(WarningTypes2.TradingHalt)].Contains(QuotingStockMaster.Code);

                // 정리매매
                QuotingStockMaster.CleaningTrade = WarningList[nameof(WarningTypes2.CleaningTrade)].Contains(QuotingStockMaster.Code);

                // 주의
                QuotingStockMaster.InvestCaution = WarningList[nameof(WarningTypes2.InvestCaution)].Contains(QuotingStockMaster.Code);

                // 위험
                QuotingStockMaster.InvestRisk = WarningList[nameof(WarningTypes2.InvestRisk)].Contains(QuotingStockMaster.Code);

                // 위험예고
                QuotingStockMaster.InvestRiskNoticed = WarningList[nameof(WarningTypes2.InvestRiskNoticed)].Contains(QuotingStockMaster.Code);

                // 단기과열
                QuotingStockMaster.Overheated = WarningList[nameof(WarningTypes2.Overheated)].Contains(QuotingStockMaster.Code);

                // 단기과열지정예고
                QuotingStockMaster.OverheatNoticed = WarningList[nameof(WarningTypes2.OverheatNoticed)].Contains(QuotingStockMaster.Code);
            }
            catch (Exception ex)
            {
                QuotingStockMaster.Code = string.Empty;
                logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }
    }
}
