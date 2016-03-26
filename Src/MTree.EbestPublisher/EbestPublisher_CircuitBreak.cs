﻿using System;
using MTree.DataStructure;

namespace MTree.EbestPublisher
{
    public partial class EbestPublisher
    {
        public override bool SubscribeCircuitBreak(string code)
        {
            if (WaitLogin() == false)
            {
                logger.Error("Not loggedin state");
                return false;
            }

            try
            {
                viSubscribingObj.SetFieldData("InBlock", "shcode", code);
                viSubscribingObj.AdviseRealData();
                subscribeCount++;

                dviSubscribingObj.SetFieldData("InBlock", "shcode", code);
                dviSubscribingObj.AdviseRealData();
                subscribeCount++;

                logger.Info($"Subscribe circuit break success, Code: {code}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Subscribe circuit break fail, Code: {code}");
            return false;
        }

        public override bool UnsubscribeCircuitBreak(string code)
        {
            if (WaitLogin() == false)
            {
                logger.Error("Not loggedin state");
                return false;
            }

            try
            {
                viSubscribingObj.SetFieldData("InBlock", "shcode", code);
                viSubscribingObj.UnadviseRealData();
                subscribeCount++;

                dviSubscribingObj.SetFieldData("InBlock", "shcode", code);
                dviSubscribingObj.UnadviseRealData();
                subscribeCount++;

                logger.Info($"Subscribe circuit break success, Code: {code}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Subscribe circuit break fail, Code: {code}");
            return false;
        }

        private void VolatilityInterruptionReceived(string szTrCode)
        {
            try
            {
                var circuitBreak = new CircuitBreak();
                circuitBreak.Time = DateTime.Now;
                circuitBreak.Code = viSubscribingObj.GetFieldData("OutBlock", "shcode");

                var circuitBreakType = Convert.ToInt32(viSubscribingObj.GetFieldData("OutBlock", "vi_gubun"));
                switch (circuitBreakType)
                {
                    case 0:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Clear;
                        circuitBreak.BasePrice = 0;
                        break;

                    case 1:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticInvoke;
                        circuitBreak.BasePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                        break;

                    case 2:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.DynamicInvoke;
                        circuitBreak.BasePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "dvi_recprice"));
                        break;

                    default:
                        logger.Error($"Unknown circuit break type: {circuitBreakType}");
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Unknown;
                        circuitBreak.BasePrice = 0;
                        break;
                }

                circuitBreak.InvokePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "vi_trgprice"));

                CircuitBreakQueue.Enqueue(circuitBreak);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void AfterVolatilityInterruptionReceived(string szTrCode)
        {
            try
            {
                var circuitBreak = new CircuitBreak();
                circuitBreak.Time = DateTime.Now;
                circuitBreak.Code = dviSubscribingObj.GetFieldData("OutBlock", "shcode");

                var circuitBreakType = Convert.ToInt32(dviSubscribingObj.GetFieldData("OutBlock", "vi_gubun"));
                switch (circuitBreakType)
                {
                    case 0:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Clear;
                        circuitBreak.BasePrice = 0;
                        break;

                    case 1:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticInvoke;
                        circuitBreak.BasePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                        break;

                    case 2:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.DynamicInvoke;
                        circuitBreak.BasePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "dvi_recprice"));
                        break;

                    default:
                        logger.Error($"Unknown circuit break type: {circuitBreakType}");
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Unknown;
                        circuitBreak.BasePrice = 0;
                        break;
                }

                circuitBreak.InvokePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "vi_trgprice"));

                CircuitBreakQueue.Enqueue(circuitBreak);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}