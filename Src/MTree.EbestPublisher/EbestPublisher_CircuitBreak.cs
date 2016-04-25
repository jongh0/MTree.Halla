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
                var now = DateTime.Now;

                var circuitBreak = new CircuitBreak();
                circuitBreak.Id = ObjectId.GenerateNewId();
                circuitBreak.ReceivedTime = now;

                int time = Convert.ToInt32(subscribingObj.GetFieldData("OutBlock", "time"));
                circuitBreak.Time = new DateTime(now.Year, now.Month, now.Day, time / 10000, time / 100 % 100, time % 100);

                circuitBreak.Code = subscribingObj.GetFieldData("OutBlock", "shcode");

                var circuitBreakType = Convert.ToInt32(subscribingObj.GetFieldData("OutBlock", "vi_gubun"));
                switch (circuitBreakType)
                {
                    case 1:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticInvoke;
                        break;
                    case 2:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.DynamicInvoke;
                        break;
                    case 3:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticAndDynamicInvoke;
                        break;
                    default: // 0
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Clear;
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
