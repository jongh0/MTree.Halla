using System;
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
                CircuitBreak circuitBreak = new CircuitBreak();
                circuitBreak.Time = DateTime.Now;
                circuitBreak.Code = viSubscribingObj.GetFieldData("OutBlock", "shcode");
                circuitBreak.CircuitBreakType = (CircuitBreakTypes)Convert.ToInt32(viSubscribingObj.GetFieldData("OutBlock", "vi_gubun"));

                if (circuitBreak.CircuitBreakType == CircuitBreakTypes.StaticInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                else if (circuitBreak.CircuitBreakType == CircuitBreakTypes.DynamicInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "dvi_recprice"));
                else
                    circuitBreak.BasePrice = 0;

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
                CircuitBreak circuitBreak = new CircuitBreak();
                circuitBreak.Time = DateTime.Now;
                circuitBreak.Code = dviSubscribingObj.GetFieldData("OutBlock", "shcode");
                circuitBreak.CircuitBreakType = (CircuitBreakTypes)Convert.ToInt32(dviSubscribingObj.GetFieldData("OutBlock", "vi_gubun"));

                if (circuitBreak.CircuitBreakType == CircuitBreakTypes.StaticInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                else if (circuitBreak.CircuitBreakType == CircuitBreakTypes.DynamicInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "dvi_recprice"));
                else
                    circuitBreak.BasePrice = 0;

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
