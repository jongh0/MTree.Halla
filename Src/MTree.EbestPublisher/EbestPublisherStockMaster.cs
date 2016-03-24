using System;
using System.Threading;
using MTree.DataStructure;

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
            if (WaitWarninglistUpdated() == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Warning list update is not done yet.");
                return false;
            }

            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1102");

            int ret = -1;

            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Quoting failed, Code: {code}, Not loggedin state");
                    return false;
                }

                WaitQuoteInterval();

                logger.Info($"Start quoting, Code: {code}");
                QuotingStockMaster = stockMaster;
                QuotingStockMaster.Code = code;

                stockQuotingObj.SetFieldData("t1102InBlock", "shcode", 0, code);
                ret = stockQuotingObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        if (QuotingStockMaster.Code != string.Empty)
                        {
                            logger.Info($"Quoting done, Code: {code}");
                            return true;
                        }

                        logger.Error($"Quoting fail, Code: {code}");
                    }

                    logger.Error($"Quoting timeout, Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail, Code: {code}, result: {ret}");
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

            return false;
        }

        private void StockMasterReceived()
        {
            try
            {
                if (QuotingStockMaster == null)
                    return;

                string cvStr = stockQuotingObj.GetFieldData("t1102OutBlock", "abscnt", 0);
                long cv = 0;
                if (long.TryParse(cvStr, out cv) == false)
                    logger.Error($"Stock master circulating volume error, {cvStr}");

                QuotingStockMaster.CirculatingVolume = cv * 1000;  //유통주식수

                string listDateStr = stockQuotingObj.GetFieldData("t1102OutBlock", "listdate", 0); // 상장일
                int listDate = 0;
                if (int.TryParse(listDateStr, out listDate) == true)
                {
                    QuotingStockMaster.ListedDate = new DateTime(listDate / 10000, listDate / 100 % 100, listDate % 100);
                }

                string valueAltered = stockQuotingObj.GetFieldData("t1102OutBlock", "info1", 0);
                if (valueAltered == "권배락") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExRightDividend;
                else if (valueAltered == "권리락") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExRight;
                else if (valueAltered == "배당락") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExDividend;
                else if (valueAltered == "액면분할") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.SplitFaceValue;
                else if (valueAltered == "액면병합") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.MergeFaceValue;
                else if (valueAltered == "주식병합") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.Consolidation;
                else if (valueAltered == "기업분할") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.Divestiture;
                else if (valueAltered == "감자") QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.CapitalReduction;
                else QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.None;

                string suspended = stockQuotingObj.GetFieldData("t1102OutBlock", "info3", 0);
                if (suspended == "suspended") QuotingStockMaster.TradingSuspend = true;
                else QuotingStockMaster.TradingSuspend = false;

                // 관리
                if (WarningList["AdministrativeIssue"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.AdministrativeIssue = true;
                else
                    QuotingStockMaster.AdministrativeIssue = false;

                // 불성실공시
                if (WarningList["UnfairAnnouncement"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.UnfairAnnouncement = true;
                else
                    QuotingStockMaster.UnfairAnnouncement = false;

                // 투자유의
                if (WarningList["InvestAttention"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestAttention = true;
                else
                    QuotingStockMaster.InvestAttention = false;

                // 투자환기
                if (WarningList["CallingAttention"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.CallingAttention = true;
                else
                    QuotingStockMaster.CallingAttention = false;

                // 경고
                if (WarningList["InvestWarning"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestWarning = true;
                else
                    QuotingStockMaster.InvestWarning = false;

                // 매매정지
                if (WarningList["TradingHalt"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.TradingHalt = true;
                else
                    QuotingStockMaster.TradingHalt = false;

                // 정리매매
                if (WarningList["CleaningTrade"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.CleaningTrade = true;
                else
                    QuotingStockMaster.CleaningTrade = false;

                // 주의
                if (WarningList["InvestCaution"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestCaution = true;
                else
                    QuotingStockMaster.InvestCaution = false;

                // 위험
                if (WarningList["InvestmentRisk"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestmentRisk = true;
                else
                    QuotingStockMaster.InvestmentRisk = false;

                // 위험예고
                if (WarningList["InvestmentRiskNoticed"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestmentRiskNoticed = true;
                else
                    QuotingStockMaster.InvestmentRiskNoticed = false;

                // 단기과열
                if (WarningList["Overheated"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.Overheated = true;
                else
                    QuotingStockMaster.Overheated = false;

                // 단기과열지정예고
                if (WarningList["OverheatNoticed"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.OverheatNoticed = true;
                else
                    QuotingStockMaster.OverheatNoticed = false;

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
