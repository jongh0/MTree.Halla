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

        public bool MakeOrder(Order order)
        {
            try
            {
                if (kiwoomObj.GetConnectState() == 0)
                {
                    logger.Error("Make order, session not connected");
                    return false;
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
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Order fail, {order.ToString()}");
            return false;
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
