using MTree.Trader;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace MTree.EbestTrader
{
    public partial class EbestTrader
    {
        private int OrderLockTimeout { get; } = 1000 * 10;

        private object OrderLock { get; } = new object();

        private long CurrDeposit { get; set; } = 0;

        public List<string> GetAccountList()
        {
            var accList = new List<string>();

            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error("Login timeout");
                    return null;
                }
                
                if (sessionObj.IsConnected() == false)
                {
                    logger.Error("Account list query, session not connected");
                    return null;
                }
                
                var accCount = sessionObj.GetAccountListCount();
                for (int i = 0; i < accCount; i++)
                {
                    var acc = sessionObj.GetAccountList(i);
                    accList.Add(acc);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return accList;
        }

        public long GetDeposit(string accNum, string accPw)
        {
            try
            {
                if (sessionObj.IsConnected() == false)
                {
                    logger.Error("Deposit query, session not connected");
                    return 0;
                }

                accDepositObj.SetFieldData("t0424InBlock", "accno", 0, accNum);
                accDepositObj.SetFieldData("t0424InBlock", "passwd", 0, accPw);

                var ret = accDepositObj.Request(false);
                if (ret < 0)
                {
                    logger.Error($"Deposit query error, {GetLastErrorMessage(ret)}");
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
            try
            {
                if (sessionObj.IsConnected() == false)
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
        
        public bool MakeOrder(Order order)
        {
            if (sessionObj.IsConnected() == false)
            {
                logger.Error("Make order, session not connected");
                return false;
            }

            if (Monitor.TryEnter(OrderLock, OrderLockTimeout) == false)
            {
                logger.Error("Making order failed, Can't obtaion lock object");
                return false;
            }

            try
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
                {
                    logger.Info($"Order success, {order.ToString()}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Monitor.Exit(OrderLock);
            }

            logger.Error($"Order fail, {order.ToString()}");
            return false;
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
                // 종목번호. 주식: 종목코드 or A+종목코드(모의투자는 A+종목코드), ELW:J+종목코드
                newOrderObj.SetFieldData(blockName, "IsuNo", 0, 'A' + order.Code);
                // 주문수량
                newOrderObj.SetFieldData(blockName, "OrdQty", 0, order.Quantity.ToString());
                // 매매구분
                if (order.OrderType == OrderTypes.BuyNew)
                    newOrderObj.SetFieldData(blockName, "BnsTpCode", 0, "2");
                else
                    newOrderObj.SetFieldData(blockName, "BnsTpCode", 0, "1");

                // 00@지정가
                // 03@시장가
                // 05@조건부지정가
                // 06@최유리지정가
                // 07@최우선지정가
                // 61@장개시전시간외종가
                // 81@시간외종가
                // 82@시간외단일가
                if (order.PriceType == PriceTypes.LimitPrice)
                {
                    // 주문가
                    newOrderObj.SetFieldData(blockName, "OrdPrc", 0, order.Price.ToString());
                    // 호가유형코드
                    newOrderObj.SetFieldData(blockName, "OrdprcPtnCode", 0, "00");
                }
                else
                {
                    // 주문가
                    newOrderObj.SetFieldData(blockName, "OrdPrc", 0, "0");
                    // 호가유형코드
                    newOrderObj.SetFieldData(blockName, "OrdprcPtnCode", 0, "03");
                }
                // 신용거래코드
                newOrderObj.SetFieldData(blockName, "MgntrnCode", 0, "000");
                // 대출일
                newOrderObj.SetFieldData(blockName, "LoanDt", 0, "");
                // 주문조건구분
                newOrderObj.SetFieldData(blockName, "OrdCndiTpCode", 0, "0");

                var ret = newOrderObj.Request(false);
                if (ret < 0)
                {
                    logger.Error($"New order error, {order.ToString()}, {GetLastErrorMessage(ret)}");
                    return false;
                }

                logger.Info($"New order success, {order.ToString()}");
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
                modifyOrderObj.SetFieldData(blockName, "OrdCndiTpCode", 0, "0");
                // 주문가
                modifyOrderObj.SetFieldData(blockName, "OrdPrc", 0, order.Price.ToString());

                var ret = modifyOrderObj.Request(false);
                if (ret < 0)
                {
                    logger.Error($"Modify order error , {order.ToString()}, {GetLastErrorMessage(ret)}");
                    return false;
                }

                logger.Info($"Modify order success, {order.ToString()}");
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

                var ret = cancelOrderObj.Request(false);
                if (ret < 0)
                {
                    logger.Error($"Cancel order error, {order.ToString()}, {GetLastErrorMessage(ret)}");
                    return false;
                }

                logger.Info($"Cancel order success, {order.ToString()}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }
        
        private void AccDepositObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");

                CurrDeposit = long.Parse(accDepositObj.GetFieldData("t0424OutBlock", "sunamt", 0));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                WaitDepositEvent.Set();
            }
        }

        private void NewOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void CancelOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ModifyOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private OrderResult GetOrderResult(XARealClass orderObj, OrderResultTypes resultType)
        {
            OrderResult result = new OrderResult();
            result.ResultType = OrderResultTypes.Unknown;

            try
            {
                #region Order result field
                StringBuilder sb = new StringBuilder();

                // 계좌번호
                var accno = orderObj.GetFieldData("OutBlock", "accno");
                sb.AppendLine($"{nameof(accno)}: {accno}");

                // 종목번호
                var Isuno = orderObj.GetFieldData("OutBlock", "Isuno");
                sb.AppendLine($"{nameof(Isuno)}: {Isuno}");

                // 단축종목번호
                var shtcode = orderObj.GetFieldData("OutBlock", "shtcode");
                sb.AppendLine($"{nameof(shtcode)}: {shtcode}");

                // 종목명
                var hname = orderObj.GetFieldData("OutBlock", "hname");
                sb.AppendLine($"{nameof(hname)}: {hname}");

                // 주문조건
                var hogagb = orderObj.GetFieldData("OutBlock", "hogagb");
                sb.AppendLine($"{nameof(hogagb)}: {hogagb}");

                // 주문번호
                var ordno = orderObj.GetFieldData("OutBlock", "ordno");
                sb.AppendLine($"{nameof(ordno)}: {ordno}");

                // 원주문번호
                var orgordno = orderObj.GetFieldData("OutBlock", "orgordno");
                sb.AppendLine($"{nameof(orgordno)}: {orgordno}");

                // 주문구분
                var ordgb = orderObj.GetFieldData("OutBlock", "ordgb");
                sb.AppendLine($"{nameof(ordgb)}: {ordgb}");

                // 주문수량
                var ordqty = orderObj.GetFieldData("OutBlock", "ordqty");
                sb.AppendLine($"{nameof(ordqty)}: {ordqty}");

                // 주문가격
                var ordprice = orderObj.GetFieldData("OutBlock", "ordprice");
                sb.AppendLine($"{nameof(ordprice)}: {ordprice}");

                // 체결수량
                var execqty = orderObj.GetFieldData("OutBlock", "execqty");
                sb.AppendLine($"{nameof(execqty)}: {execqty}");

                // 체결가격
                var execprc = orderObj.GetFieldData("OutBlock", "execprc");
                sb.AppendLine($"{nameof(execprc)}: {execprc}");

                // 모주문번호
                var prntordno = orderObj.GetFieldData("OutBlock", "prntordno");
                sb.AppendLine($"{nameof(prntordno)}: {prntordno}");

                // 원주문미체결수량
                var orgordundrqty = orderObj.GetFieldData("OutBlock", "orgordundrqty");
                sb.AppendLine($"{nameof(orgordundrqty)}: {orgordundrqty}");

                // 원주문정정수량
                var orgordmdfyqty = orderObj.GetFieldData("OutBlock", "orgordmdfyqty");
                sb.AppendLine($"{nameof(orgordmdfyqty)}: {orgordmdfyqty}");

                // 원주문취소수량
                var ordordcancelqty = orderObj.GetFieldData("OutBlock", "ordordcancelqty");
                sb.AppendLine($"{nameof(ordordcancelqty)}: {ordordcancelqty}");

                logger.Info($"Order result field\n{sb.ToString()}"); 
                #endregion

                result.AccountNumber = accno;
                result.OrderNumber = ordno;
                result.Code = (resultType == OrderResultTypes.Submitted) ? shtcode : Isuno;
                result.OrderedQuantity = ConvertUtility.ToInt32(ordqty);
                result.OrderedPrice = ConvertUtility.ToInt32(ordprice);
                result.ConcludedQuantity = ConvertUtility.ToInt32(execqty);
                result.ConcludedPrice = ConvertUtility.ToInt32(execprc);

                result.ResultType = resultType;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        private void OrderSubmittedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC0")
            {
                logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC0");
                return;
            }

            var result = GetOrderResult(orderSubmittedObj, OrderResultTypes.Submitted);
            NotifyOrderResult(result);
        }

        private void OrderConcludedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC1")
            {
                logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC1");
                return;
            }

            var result = GetOrderResult(orderConcludedObj, OrderResultTypes.Concluded);
            NotifyOrderResult(result);
        }

        private void OrderModifiedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC2")
            {
                logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC2");
                return;
            }

            var result = GetOrderResult(orderModifiedObj, OrderResultTypes.Modified);
            NotifyOrderResult(result);
        }

        private void OrderCanceledObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC3")
            {
                logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC3");
                return;
            }

            var result = GetOrderResult(orderConcludedObj, OrderResultTypes.Concluded);
            NotifyOrderResult(result);
        }

        private void OrderRejectedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC4")
            {
                logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC4");
                return;
            }

            var result = GetOrderResult(orderRejectedObj, OrderResultTypes.Rejected);
            NotifyOrderResult(result);
        }

        private void NotifyOrderResult(OrderResult result)
        {
            logger.Info($"NotifyOrderResult, {result}");

            if (result.ResultType == OrderResultTypes.Unknown)
                return;

            foreach (var contract in TraderContracts)
            {
                contract.Value.Callback.NotifyOrderResult(result);
            }
        }
    }
}
