using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.EbestTrader
{
    public partial class EbestTrader
    {
        private long CurrDeposit { get; set; } = 0;

        public List<string> GetAccountList()
        {
            try
            {
                if (sessionObj.IsConnected() == false)
                {
                    logger.Error("Session not connected");
                    return null;
                }

                var accList = new List<string>();

                var accCount = sessionObj.GetAccountListCount();
                for (int i = 0; i < accCount; i++)
                {
                    var acc = sessionObj.GetAccountList(i);
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
                accDepositObj.SetFieldData("t0424InBlock", "accno", 0, accNum);
                accDepositObj.SetFieldData("t0424InBlock", "passwd", 0, accPw);

                if (accDepositObj.Request(false) < 0)
                {
                    logger.Error($"Deposit query error, {GetLastErrorMessage()}");
                    return 0;
                }

                if (WaitDepositEvent.WaitOne(WaitTimeout) == false)
                {
                    logger.Error($"Deposit checking timeout");
                    return 0;
                }

                return CurrDeposit;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return 0;
        }

        public List<HoldingStock> GetHoldingList(string accNum)
        {
            throw new NotImplementedException();
        }

        public OrderResult MakeOrder(Order order)
        {
            switch (order.OrderType)
            {
                case OrderTypes.Buy:
                case OrderTypes.Sell:
                    return MakeNormalOrder(order);

                case OrderTypes.BuyModify:
                case OrderTypes.SellModify:
                    return MakeModifyOrder(order);

                case OrderTypes.Cancel:
                    return MakeCancelOrder(order);

                default:
                    return null;
            }
        }

        private OrderResult MakeNormalOrder(Order order)
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

        private OrderResult MakeModifyOrder(Order order)
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

        private OrderResult MakeCancelOrder(Order order)
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
