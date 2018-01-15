using Trader;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XA_DATASETLib;
using CommonLib.Firm.Ebest;
using CommonLib.Firm.Ebest.Block;
using CommonLib.Firm.Ebest.Query;
using CommonLib.Converter;

namespace EbestTrader
{
    public partial class EbestTrader_
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
                    _logger.Error("Login timeout");
                    return null;
                }
                
                if (sessionObj.IsConnected() == false)
                {
                    _logger.Error("Account list query, session not connected");
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
                _logger.Error(ex);
            }

            return accList;
        }

        public long GetDeposit(string accNum, string accPw)
        {
            try
            {
                if (sessionObj.IsConnected() == false)
                {
                    _logger.Error("Deposit query, session not connected");
                    return 0;
                }

                accDepositObj.SetFieldData("t0424InBlock", "accno", 0, accNum);
                accDepositObj.SetFieldData("t0424InBlock", "passwd", 0, accPw);

                var ret = accDepositObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Deposit query error, {GetLastErrorMessage(ret)}");
                    return 0;
                }

                if (WaitDepositEvent.WaitOne(WaitTimeout) == false)
                {
                    _logger.Error($"Deposit checking timeout");
                    return 0;
                }

                return CurrDeposit;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return 0;
        }

        public List<HoldingStock> GetHoldingList(string accNum)
        {
            try
            {
                if (sessionObj.IsConnected() == false)
                {
                    _logger.Error("Holding list query, session not connected");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }
        
        public bool MakeOrder(Order order)
        {
            if (sessionObj.IsConnected() == false)
            {
                _logger.Error("Make order, session not connected");
                return false;
            }

            if (Monitor.TryEnter(OrderLock, OrderLockTimeout) == false)
            {
                _logger.Error("Making order failed, Can't obtaion lock object");
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
                    _logger.Info($"Order success, {order.ToString()}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                Monitor.Exit(OrderLock);
            }

            _logger.Error($"Order fail, {order.ToString()}");
            return false;
        }

        private bool MakeNewOrder(Order order)
        {
            try
            {
#if true
                var block = new CSPAT00600InBlock1();
                order.CopyTo(block);

                var query = new EbestQuery<CSPAT00600InBlock1, CSPAT00600OutBlock1, CSPAT00600OutBlock2>();
                if (query.ExecuteQuery(block) == false)
                {
                    _logger.Error($"New order error, {order.ToString()}, {GetLastErrorMessage(query.Result)}");
                    return false;
                }

                return true;
#else
                var block = new CSPAT00600InBlock1();
                order.CopyTo(block);

                if (newOrderObj.SetFieldData(block) == false)
                {
                    _logger.Error("New order set field error");
                    return false;
                }

                var ret = newOrderObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"New order error, {order.ToString()}, {GetLastErrorMessage(ret)}");
                    return false;
                }

                _logger.Info($"New order success, {order.ToString()}");
                return true; 
#endif
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private bool MakeModifyOrder(Order order)
        {
            try
            {
                var block = new CSPAT00700InBlock1();
                order.CopyTo(block);

                if (newOrderObj.SetFieldData(block) == false)
                {
                    _logger.Error("Modify order set field error");
                    return false;
                }

                var ret = modifyOrderObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Modify order error , {order.ToString()}, {GetLastErrorMessage(ret)}");
                    return false;
                }

                _logger.Info($"Modify order success, {order.ToString()}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private bool MakeCancelOrder(Order order)
        {
            try
            {
                var block = new CSPAT00800InBlock1();
                order.CopyTo(block);

                if (newOrderObj.SetFieldData(block) == false)
                {
                    _logger.Error("Cancel order set field error");
                    return false;
                }

                var ret = cancelOrderObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Cancel order error, {order.ToString()}, {GetLastErrorMessage(ret)}");
                    return false;
                }

                _logger.Info($"Cancel order success, {order.ToString()}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
        
        private void AccDepositObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                CurrDeposit = long.Parse(accDepositObj.GetFieldData("t0424OutBlock", "sunamt", 0));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                _logger.Trace($"szTrCode: {szTrCode}");

                if (newOrderObj.GetFieldData(out CSPAT00600OutBlock1 block) == false)
                {
                    _logger.Error("NewOrderObj_ReceiveData.GetFieldData.Error");
                    return;
                }

                _logger.Info($"NewOrderObj_ReceiveData.Done, {block.ToString()}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ModifyOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                if (modifyOrderObj.GetFieldData(out CSPAT00700OutBlock1 block) == false)
                {
                    _logger.Error("ModifyOrderObj_ReceiveData.GetFieldData.Error");
                    return;
                }

                _logger.Info($"ModifyOrderObj_ReceiveData.Done, {block.ToString()}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void CancelOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                if (cancelOrderObj.GetFieldData(out CSPAT00800OutBlock1 block) == false)
                {
                    _logger.Error("CancelOrderObj_ReceiveData.GetFieldData.Error");
                    return;
                }

                _logger.Info($"CancelOrderObj_ReceiveData.Done, {block.ToString()}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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

                _logger.Info($"Order result field\n{sb.ToString()}"); 
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
                _logger.Error(ex);
            }

            return result;
        }

        private void OrderSubmittedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC0")
            {
                _logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC0");
                return;
            }

            var result = GetOrderResult(orderSubmittedObj, OrderResultTypes.Submitted);
            NotifyOrderResult(result);
        }

        private void OrderConcludedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC1")
            {
                _logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC1");
                return;
            }

            var result = GetOrderResult(orderConcludedObj, OrderResultTypes.Concluded);
            NotifyOrderResult(result);
        }

        private void OrderModifiedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC2")
            {
                _logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC2");
                return;
            }

            var result = GetOrderResult(orderModifiedObj, OrderResultTypes.Modified);
            NotifyOrderResult(result);
        }

        private void OrderCanceledObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC3")
            {
                _logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC3");
                return;
            }

            var result = GetOrderResult(orderConcludedObj, OrderResultTypes.Concluded);
            NotifyOrderResult(result);
        }

        private void OrderRejectedObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode != "SC4")
            {
                _logger.Error($"Wrong tr code. Received: {szTrCode}. Expected: SC4");
                return;
            }

            var result = GetOrderResult(orderRejectedObj, OrderResultTypes.Rejected);
            NotifyOrderResult(result);
        }

        private void OrderRejectedReal_OutBlockReceived(SC4OutBlock block)
        {
            var result = new OrderResult();
            result.AccountNumber = block.accno;
            result.OrderNumber = block.ordno.ToString();
            result.Code = block.Isuno;
            result.OrderedQuantity = block.ordqty;
            result.OrderedPrice = block.ordprc;
            result.ConcludedQuantity = block.execqty;
            result.ConcludedPrice = block.execprc;
            result.ResultType = OrderResultTypes.Rejected;

            NotifyOrderResult(result);
        }

        private void OrderCanceledReal_OutBlockReceived(SC3OutBlock block)
        {
            var result = new OrderResult();
            result.AccountNumber = block.accno;
            result.OrderNumber = block.ordno.ToString();
            result.Code = block.Isuno;
            result.OrderedQuantity = block.ordqty;
            result.OrderedPrice = block.ordprc;
            result.ConcludedQuantity = block.execqty;
            result.ConcludedPrice = block.execprc;
            result.ResultType = OrderResultTypes.Canceled;

            NotifyOrderResult(result);
        }

        private void OrderModifiedReal_OutBlockReceived(SC2OutBlock block)
        {
            var result = new OrderResult();
            result.AccountNumber = block.accno;
            result.OrderNumber = block.ordno.ToString();
            result.Code = block.Isuno;
            result.OrderedQuantity = block.ordqty;
            result.OrderedPrice = block.ordprc;
            result.ConcludedQuantity = block.execqty;
            result.ConcludedPrice = block.execprc;
            result.ResultType = OrderResultTypes.Modified;

            NotifyOrderResult(result);
        }

        private void OrderConclusionReal_OutBlockReceived(SC1OutBlock block)
        {
            var result = new OrderResult();
            result.AccountNumber = block.accno;
            result.OrderNumber = block.ordno.ToString();
            result.Code = block.Isuno;
            result.OrderedQuantity = block.ordqty;
            result.OrderedPrice = block.ordprc;
            result.ConcludedQuantity = block.execqty;
            result.ConcludedPrice = block.execprc;
            result.ResultType = OrderResultTypes.Concluded;

            NotifyOrderResult(result);
        }

        private void OrderSubmitReal_OutBlockReceived(SC0OutBlock block)
        {
            var result = new OrderResult();
            result.AccountNumber = block.accno;
            result.OrderNumber = block.ordno.ToString();
            result.Code = block.expcode;
            result.OrderedQuantity = block.ordqty;
            result.ResultType = OrderResultTypes.Submitted;

            NotifyOrderResult(result);
        }

        private void NotifyOrderResult(OrderResult result)
        {
            _logger.Info($"NotifyOrderResult, {result}");

            if (result.ResultType == OrderResultTypes.Unknown)
                return;

            foreach (var contract in TraderContracts)
            {
                contract.Value.Callback.NotifyOrderResult(result);
            }
        }
    }
}
