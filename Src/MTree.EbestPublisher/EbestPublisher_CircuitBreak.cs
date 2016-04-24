using System;
using MTree.DataStructure;
using MongoDB.Bson;
using System.ServiceModel;
using XA_DATASETLib;

namespace MTree.EbestPublisher
{
    public partial class EbestPublisher
    {
        private int _CircuitBreakSubscribeCount = 0;
        public int CircuitBreakSubscribeCount
        {
            get { return _CircuitBreakSubscribeCount; }
            set
            {
                _CircuitBreakSubscribeCount = value;
                NotifyPropertyChanged(nameof(CircuitBreakSubscribeCount));
            }
        }

        public override bool SubscribeCircuitBreak(string code)
        {
            try
            {
                viSubscribingObj.SetFieldData("InBlock", "shcode", code);
                viSubscribingObj.AdviseRealData();

                dviSubscribingObj.SetFieldData("InBlock", "shcode", code);
                dviSubscribingObj.AdviseRealData();

                CircuitBreakSubscribeCount++;
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
            try
            {
                viSubscribingObj.SetFieldData("InBlock", "shcode", code);
                viSubscribingObj.UnadviseRealData();

                dviSubscribingObj.SetFieldData("InBlock", "shcode", code);
                dviSubscribingObj.UnadviseRealData();

                CircuitBreakSubscribeCount--;
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

        private void ViSubscribingObj_ReceiveRealData(string szTrCode)
        {
            LastCommTick = Environment.TickCount;
            CircuitBreakReceived(viSubscribingObj);
        }

        private void DviSubscribingObj_ReceiveRealData(string szTrCode)
        {
            LastCommTick = Environment.TickCount;
            CircuitBreakReceived(dviSubscribingObj);
        }

        private void CircuitBreakReceived(XARealClass subscribingObj)
        {
            try
            {
                var circuitBreak = new CircuitBreak();
                circuitBreak.Id = ObjectId.GenerateNewId();
                circuitBreak.Time = circuitBreak.ReceivedTime = DateTime.Now;

                circuitBreak.Code = subscribingObj.GetFieldData("OutBlock", "shcode");

                var circuitBreakType = Convert.ToInt32(subscribingObj.GetFieldData("OutBlock", "vi_gubun"));
                var time = subscribingObj.GetFieldData("OutBlock", "time");
                var svi_recprice = Convert.ToSingle(subscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                var dvi_recprice = Convert.ToSingle(subscribingObj.GetFieldData("OutBlock", "dvi_recprice"));
                logger.Info($"CircuitBreak debugging, type: {circuitBreakType}, time: {time}, svi_recprice: {svi_recprice}, dvi_recprice: {dvi_recprice}"); // For debugging

                switch (circuitBreakType)
                {
                    case 1:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticInvoke;
                        circuitBreak.InvokeBasisPrice = svi_recprice;
                        break;

                    case 2:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.DynamicInvoke;
                        circuitBreak.InvokeBasisPrice = dvi_recprice;
                        break;

                    case 3:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticAndDynamicInvoke;
                        circuitBreak.InvokeBasisPrice = dvi_recprice; // TODO: dvi_recprice 맞나?
                        break;

                    default: // 0
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Clear;
                        circuitBreak.InvokeBasisPrice = 0;
                        break;
                }

                circuitBreak.InvokePrice = Convert.ToSingle(subscribingObj.GetFieldData("OutBlock", "vi_trgprice"));

                CircuitBreakQueue.Enqueue(circuitBreak);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
