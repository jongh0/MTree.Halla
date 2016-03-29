﻿using MTree.DataStructure;
using System;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        public override IndexMaster GetIndexMaster(string code)
        {
            var master = new IndexMaster();
            master.Code = code;

            if (GetQuote(code, ref master) == true)
                master.Code = CodeEntity.RemovePrefix(code);
            else
                master.Code = string.Empty;

            return master;
        }

        public bool GetQuote(string code, ref IndexMaster master)
        {
            if (sessionObj.IsConnect != 1)
            {
                logger.Error("Quoting failed, session not connected");
                return false;
            }

            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            short ret = -1;

            try
            {
                WaitQuoteInterval();

                logger.Info($"Start quoting, Code: {code}");
                QuotingIndexMaster = master;

                stockMstObj.SetInputValue(0, code);
                ret = stockMstObj.BlockRequest();

                if (ret == 0)
                {
                    if (QuotingIndexMaster.Code != string.Empty)
                    {
                        logger.Info($"Quoting done, Code: {code}");
                        return true;
                    }

                    logger.Error($"Quoting fail, Code: {code}");
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

                // 1 - (string) 종목 명
                QuotingIndexMaster.Name = stockMstObj.GetHeaderValue(1).ToString();

                // 10 - (long) 전일종가
                QuotingIndexMaster.PreviousClosedPrice = (int)stockMstObj.GetHeaderValue(10);

                // 27 - (long) basis price (기준가)
                QuotingIndexMaster.BasisPrice = (int)stockMstObj.GetHeaderValue(27);

                // 46 - (long) 전일 거래량
                QuotingIndexMaster.PreviousVolume = Convert.ToInt64(stockMstObj.GetHeaderValue(46));
            }
            catch (Exception ex)
            {
                QuotingIndexMaster.Code = string.Empty;
                logger.Error(ex);
            }
        }
    }
}
