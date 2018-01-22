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
                
                if (_session.IsConnected() == false)
                {
                    _logger.Error("Account list query, session not connected");
                    return null;
                }
                
                var accCount = _session.GetAccountListCount();
                for (int i = 0; i < accCount; i++)
                {
                    var acc = _session.GetAccountList(i);
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
                if (_session.IsConnected() == false)
                {
                    _logger.Error("Deposit query, session not connected");
                    return 0;
                }

                var query = new EbestQuery<t0424InBlock, t0424OutBlock, t0424OutBlock1>();
                if (query.ExecuteQueryAndWait(new t0424InBlock { accno = accNum, passwd = accPw}) == false)
                {
                    _logger.Error($"Deposit query error, {GetLastErrorMessage(query.Result)}");
                    return 0;
                }

                return CurrDeposit = query.OutBlock.sunamt;
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
                if (_session.IsConnected() == false)
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
            if (_session.IsConnected() == false)
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
                var block = new CSPAT00600InBlock1();
                order.CopyTo(block);

                var query = new EbestQuery<CSPAT00600InBlock1, CSPAT00600OutBlock1, CSPAT00600OutBlock2>();
                if (query.ExecuteQuery(block) == false)
                {
                    _logger.Error($"New order error, {order.ToString()}, {GetLastErrorMessage(query.Result)}");
                    return false;
                }

                return true;
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

                var query = new EbestQuery<CSPAT00700InBlock1, CSPAT00700OutBlock1, CSPAT00700OutBlock2>();
                if (query.ExecuteQuery(block) == false)
                {
                    _logger.Error($"Modify order error, {order.ToString()}, {GetLastErrorMessage(query.Result)}");
                    return false;
                }

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

                var query = new EbestQuery<CSPAT00800InBlock1, CSPAT00800OutBlock1, CSPAT00800OutBlock2>();
                if (query.ExecuteQuery(block) == false)
                {
                    _logger.Error($"Cancel order error, {order.ToString()}, {GetLastErrorMessage(query.Result)}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
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

            foreach (var contract in TraderContracts.Values)
            {
                contract.Callback.NotifyOrderResult(result);
            }
        }
    }
}
