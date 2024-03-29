﻿using System;
using System.Threading;
using DataStructure;
using CommonLib;
using FirmLib.Ebest;
using FirmLib.Ebest.Block;
using FirmLib.Ebest.Query;

namespace EbestPublisher
{
    public partial class EbestPublisher_
    {
        public bool GetQuote(string code, ref IndexMaster indexMaster)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                _logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            try
            {
                QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1511");
                WaitQuoteInterval();

                _logger.Info($"Start quoting, Code: {code}");
                QuotingIndexMaster = indexMaster;

                indexQuotingObj.SetFieldData("t1511InBlock", "upcode", 0, code);
                var ret = indexQuotingObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Quoting request error, {GetLastErrorMessage(ret)}");
                    return false;
                }

                if (WaitQuoting() == false)
                    return false;

                if (QuotingIndexMaster.Code != string.Empty)
                {
                    _logger.Info($"Quoting done, Code: {code}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                QuotingIndexMaster = null;
                Monitor.Exit(QuoteLock);
            }

            _logger.Error($"Quoting fail, Code: {code}");
            return false;
        }

        private void IndexQuotingObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                if (QuotingIndexMaster == null)
                    return;

                if (indexQuotingObj.GetFieldData(out t1511OutBlock block) == false)
                {
                    _logger.Error($"IndexQuoting GetFieldData error");
                    return;
                }

                QuotingIndexMaster.BasisPrice = block.jniljisu;
            }
            catch (Exception ex)
            {
                QuotingIndexMaster.Code = string.Empty;
                _logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }
    }
}
