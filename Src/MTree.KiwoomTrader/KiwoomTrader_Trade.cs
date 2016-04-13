using MTree.Trader;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.KiwoomTrader
{
    public partial class KiwoomTrader
    {
        private OrderResult CurrOrderResult { get; set; }

        public List<string> GetAccountList()
        {
            try
            {
                if (kiwoomObj.GetConnectState() == 0)
                {
                    logger.Error("Account list query, session not connected");
                    return null;
                }

                List<string> accList = new List<string>();
                foreach (string acc in kiwoomObj.GetLoginInfo("ACCNO").Split(';'))
                {
                    if (string.IsNullOrEmpty(acc) == false)
                        accList.Add(acc);
                }

                return accList;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public long GetDeposit(string accNum, string accPw)
        {
            try
            {
                if (kiwoomObj.GetConnectState() == 0)
                {
                    logger.Error("Deposit query, session not connected");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return 0;
        }

        public OrderResult MakeOrder(Order order)
        {
            try
            {
                if (kiwoomObj.GetConnectState() == 0)
                {
                    logger.Error("Make order, session not connected");
                    goto ORDER_FAIL;
                }

                var hoga = (order.PriceType == PriceTypes.LimitPrice) ? "00" : "03";

                var ret = kiwoomObj.SendOrder(
                    "주식주문",
                    KiwoomScreen.GetScreenNum(), 
                    order.AccountNumber, 
                    (int)order.OrderType, 
                    order.Code, 
                    order.Quantity, 
                    order.Price, 
                    hoga, 
                    order.OriginOrderNumber);

                if (ret == 0)
                {
                    logger.Info($"Order success, {order.ToString()}");
                    return CurrOrderResult;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Order fail, {order.ToString()}");
            
            ORDER_FAIL:
            OrderResult failedOrder = new OrderResult();
            failedOrder.OrderNumber = string.Empty;
            failedOrder.Code = order.Code;
            failedOrder.OrderedQuantity = order.Quantity;
            failedOrder.OrderType = order.OrderType;
            failedOrder.OrderedPrice = order.Price;
            failedOrder.ConcludedPrice = 0;
            failedOrder.ConcludedQuantity = 0;
            return failedOrder;
        }

        public List<HoldingStock> GetHoldingList(string accNum)
        {
            try
            {
                if (kiwoomObj.GetConnectState() == 0)
                {
                    logger.Error("Holding list query, session not connected");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        private void OrderResultReceived(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void OrderConclusionReceived(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void AccountDepositReceived(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
