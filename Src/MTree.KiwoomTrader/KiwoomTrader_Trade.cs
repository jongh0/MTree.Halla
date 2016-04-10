using MTree.Trader;
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
                if (WaitLogin() == false)
                {
                    logger.Error("Account list, login error");
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
                var rqName = "MakrOrder";
                var screenNum = "1";
                var accNum = order.AccountNumber;
                var orderType = (int)order.OrderType;
                var code = order.Code;
                var qty = order.Quantity;
                var price = order.Price;
                var hoga = (order.PriceType == PriceTypes.LimitPrice) ? "00" : "03";
                var originOrderNum = order.OriginOrderNumber;

                var ret = kiwoomObj.SendOrder(rqName, screenNum, accNum, orderType, code, qty, price, hoga, originOrderNum);
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
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }
    }
}
