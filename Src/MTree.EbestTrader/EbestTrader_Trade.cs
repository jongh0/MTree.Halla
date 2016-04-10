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
        private OrderResult CurrOrderResult { get; set; }

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

        public bool MakeOrder(Order order)
        {
            bool ret = false;

            switch (order.OrderType)
            {
                case OrderTypes.BuyNew:
                case OrderTypes.SellNew:
                    ret = MakeNewOrder(order);
                    break;

                case OrderTypes.BuyModify:
                case OrderTypes.SellModify:
                    ret = MakeModifyOrder(order);
                    break;

                case OrderTypes.BuyCancel:
                case OrderTypes.SellCancel:
                    ret = MakeCancelOrder(order);
                    break;
            }

            if (ret == true)
                logger.Info($"Order success, {order.ToString()}");
            else
                logger.Error($"Order fail, {order.ToString()}");

            return ret;
        }

        private bool MakeNewOrder(Order order)
        {
            try
            {
                var blockName = "CSPAT00600InBlock1";

                // 계좌번호
                newOrderObj.SetFieldData(blockName, "AcntNo", 0, order.AccountNumber);
                // 계좌비밀번호
                newOrderObj.SetFieldData(blockName, "InptPwd", 0, order.AccountPassword);
                // 종목번호
                newOrderObj.SetFieldData(blockName, "IsuNo", 0, order.Code);
                // 주문수량
                newOrderObj.SetFieldData(blockName, "OrdQty", 0, order.Quantity.ToString());
                // 주문가
                newOrderObj.SetFieldData(blockName, "OrdPrc", 0, order.Price.ToString());
                // 매매구분
                newOrderObj.SetFieldData(blockName, "BnsTpCode", 0, order.OrderType.ToString());
                // 호가유형코드
                if (order.PriceType == PriceTypes.LimitPrice)
                    newOrderObj.SetFieldData(blockName, "OrdprcPtnCode", 0, "00");
                else
                    newOrderObj.SetFieldData(blockName, "OrdprcPtnCode", 0, "03");
                // 신용거래코드
                //normalOrderObj.SetFieldData(blockName, "MgntrnCode", 0, "");
                // 대출일
                //normalOrderObj.SetFieldData(blockName, "LoanDt", 0, "");
                // 주문조건구분
                //normalOrderObj.SetFieldData(blockName, "OrdCndiTpCode", 0, "");

                if (newOrderObj.Request(false) < 0)
                {
                    logger.Error($"New order error, {GetLastErrorMessage()}");
                    return false;
                }

                if (WaitOrderEvent.WaitOne(WaitTimeout) == false)
                {
                    logger.Error($"New order timeout");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        private bool MakeModifyOrder(Order order)
        {
            try
            {
                var blockName = "CSPAT00700InBlock1";

                // 원주문번호
                modifyOrderObj.SetFieldData(blockName, "OrgOrdNo", 0, order.OriginOrderNumber);
                // 계좌번호
                modifyOrderObj.SetFieldData(blockName, "AcntNo", 0, order.AccountNumber);
                // 계좌비밀번호
                modifyOrderObj.SetFieldData(blockName, "InptPwd", 0, order.AccountPassword);
                // 종목번호
                modifyOrderObj.SetFieldData(blockName, "IsuNo", 0, order.Code);
                // 주문수량
                modifyOrderObj.SetFieldData(blockName, "OrdQty", 0, order.Quantity.ToString());
                // 호가유형코드
                if (order.PriceType == PriceTypes.LimitPrice)
                    modifyOrderObj.SetFieldData(blockName, "OrdprcPtnCode", 0, "00");
                else
                    modifyOrderObj.SetFieldData(blockName, "OrdprcPtnCode", 0, "03");
                // 주문조건구분
                //modifyOrderObj.SetFieldData(blockName, "OrdCndiTpCode", 0, "");
                // 주문가
                modifyOrderObj.SetFieldData(blockName, "OrdPrc", 0, order.Price.ToString());

                if (modifyOrderObj.Request(false) < 0)
                {
                    logger.Error($"Modify order error, {GetLastErrorMessage()}");
                    return false;
                }

                if (WaitOrderEvent.WaitOne(WaitTimeout) == false)
                {
                    logger.Error($"Modify order timeout");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        private bool MakeCancelOrder(Order order)
        {
            try
            {
                var blockName = "CSPAT00800InBlock1";

                // 원주문번호
                cancelOrderObj.SetFieldData(blockName, "OrgOrdNo", 0, order.OriginOrderNumber);
                // 계좌번호
                cancelOrderObj.SetFieldData(blockName, "AcntNo", 0, order.AccountNumber);
                // 계좌비밀번호
                cancelOrderObj.SetFieldData(blockName, "InptPwd", 0, order.AccountPassword);
                // 종목번호
                cancelOrderObj.SetFieldData(blockName, "IsuNo", 0, order.Code);
                // 주문수량
                cancelOrderObj.SetFieldData(blockName, "OrdQty", 0, order.Quantity.ToString());

                if (cancelOrderObj.Request(false) < 0)
                {
                    logger.Error($"Cancel order error, {GetLastErrorMessage()}");
                    return false;
                }

                if (WaitOrderEvent.WaitOne(WaitTimeout) == false)
                {
                    logger.Error($"Cancel order timeout");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }
    }
}
