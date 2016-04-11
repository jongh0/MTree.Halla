#define NOT_USE_QUEUE

using System;
using MTree.DataStructure;
using MongoDB.Bson;
using System.ServiceModel;

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
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");

                var circuitBreak = new CircuitBreak();
                circuitBreak.Id = ObjectId.GenerateNewId();
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

#if NOT_USE_QUEUE
                if (ServiceClient.State == CommunicationState.Opened)
                    ServiceClient.PublishCircuitBreak(circuitBreak);
#else
                CircuitBreakQueue.Enqueue(circuitBreak); 
#endif
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void DviSubscribingObj_ReceiveRealData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");

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

#if NOT_USE_QUEUE
                if (ServiceClient.State == CommunicationState.Opened)
                    ServiceClient.PublishCircuitBreak(circuitBreak);
#else
                CircuitBreakQueue.Enqueue(circuitBreak); 
#endif
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
