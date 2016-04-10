using System;
using System.Threading;
using MTree.DataStructure;

namespace MTree.EbestPublisher
{
    public partial class EbestPublisher
    {
        public bool GetQuote(string code, ref IndexMaster indexMaster)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1511");

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
                QuotingIndexMaster = indexMaster;

                indexQuotingObj.SetFieldData("t1511InBlock", "upcode", 0, code);
                ret = indexQuotingObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        if (QuotingIndexMaster.Code != string.Empty)
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
                    logger.Error($"Quoting request fail, Code: {code}, Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingIndexMaster = null;
                Monitor.Exit(QuoteLock);
            }

            return false;
        }

        private void indexQuotingObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");

                if (QuotingIndexMaster == null)
                    return;

                // 현재가
                string basisPriceStr = indexQuotingObj.GetFieldData("t1511OutBlock", "jniljisu", 0);
                float basisPrice = 0;
                if (float.TryParse(basisPriceStr, out basisPrice) == true)
                {
                    QuotingIndexMaster.BasisPrice = basisPrice;
                }
                else
                {
                    logger.Error($"Basis price error: {basisPriceStr}");
                    QuotingIndexMaster.BasisPrice = 0;
                }
            }
            catch (Exception ex)
            {
                QuotingIndexMaster.Code = string.Empty;
                logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }
    }
}
