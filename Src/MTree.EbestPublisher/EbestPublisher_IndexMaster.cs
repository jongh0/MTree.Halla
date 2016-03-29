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

        private void IndexMasterReceived()
        {
            try
            {
                if (QuotingIndexMaster == null)
                    return;

                string temp = indexQuotingObj.GetFieldData("t1511OutBlock", "jniljisu", 0);
                if (temp == "") temp = "0";
                QuotingIndexMaster.PreviousClosedPrice = Convert.ToDouble(temp); // 현재가

                temp = indexQuotingObj.GetFieldData("t1511OutBlock", "jnilvolume", 0);
                if (temp == "") temp = "0";
                QuotingIndexMaster.PreviousVolume = Convert.ToInt64(temp); //전일거래량
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
