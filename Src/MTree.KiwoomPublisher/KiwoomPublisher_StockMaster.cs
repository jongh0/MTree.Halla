using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Threading;

namespace MTree.KiwoomPublisher
{
    public partial class KiwoomPublisher
    {
        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

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

                kiwoomObj.SetInputValue("종목코드", code);

                ret = kiwoomObj.CommRqData("주식기본정보", "OPT10001", 0, GetScrNum());

                if (ret == 0)
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
                    logger.Error($"Quoting request fail, Code: {code}, Quoting result: {ret}. Message: {ErrorMessageUtility.GetErrorMessage(ret)}");
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

        private void OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            // OPT1001 : 주식기본정보
            if (e?.sRQName == "주식기본정보")
            {
                try
                {
                    int nCnt = kiwoomObj.GetRepeatCnt(e.sTrCode, e.sRQName);
                    if (nCnt != 1)
                    {
                        logger.Error("Multiple response received for single request");
                        QuotingStockMaster.Code = string.Empty;
                        return;
                    }

                    string rxCode = kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "종목코드").Trim();
                    if (QuotingStockMaster.Code != rxCode)
                    {
                        logger.Error($"Received code({rxCode}) is different from requested({QuotingStockMaster.Code})");
                        QuotingStockMaster.Code = string.Empty;
                        return;
                    }

                    QuotingStockMaster.PER = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "PER").Trim());
                    QuotingStockMaster.EPS = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "EPS").Trim());
                    QuotingStockMaster.PBR = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "PBR").Trim());
                    QuotingStockMaster.BPS = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "BPS").Trim());

                    string roe = kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "ROE").Trim();
                    if (roe != string.Empty)
                        QuotingStockMaster.ROE = Convert.ToDouble(roe);

                    string ev = kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "EV").Trim();
                    if (ev != string.Empty)
                        QuotingStockMaster.EV = Convert.ToDouble(ev);
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

        public override StockMaster GetStockMaster(string code)
        {
            try
            {
                var stockMaster = new StockMaster();
                stockMaster.Code = code;

                if (GetQuote(code, ref stockMaster) == false)
                    stockMaster.Code = string.Empty;

                return stockMaster;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
    }
}
