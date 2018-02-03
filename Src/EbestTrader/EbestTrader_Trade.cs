using Trader;
using CommonLib;
using System;
using System.Collections.Generic;
using CommonLib.Firm.Ebest.Block;
using CommonLib.Firm.Ebest.Query;
using Trader.Account;

namespace EbestTrader
{
    public partial class EbestTrader_
    {
        public List<AccountInformation> GetAccountInformations()
        {
            var accountInfos = new List<AccountInformation>();

            try
            {
                if (_session.WaitLogin() == false)
                {
                    _logger.Error("Not login state");
                    return accountInfos;
                }

                var accountNums = GetAccountNumbers();
                if (accountNums == null) return accountInfos;

                foreach (var accountNum in accountNums)
                {
                    var info = GetAccountInfo(accountNum, _loginInfo.AccountPassword);
                    if (info != null)
                    {
                        info.AccountNumber = accountNum;
                        accountInfos.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return accountInfos;
        }

        private AccountInformation GetAccountInfo(string accountNum, string accountPw)
        {
            try
            {
                if (_session.WaitLogin() == false)
                {
                    _logger.Error("Not login state");
                    return null;
                }

                _session.LastCommTick = Environment.TickCount;

#if true
                var query = new EbestQuery<t0424InBlock>();
                if (query.ExecuteQueryAndWait(new t0424InBlock { accno = accountNum, passwd = accountPw }) == false)
                {
                    _logger.Error($"Deposit query error, {GetLastErrorMessage(query.Result)}");
                    return null;
                }

                var info = AutoMapper.Mapper.Map<AccountInformation>(query.GetOutBlock<t0424OutBlock>());

                var blocks = query.GetAllOutBlocks<t0424OutBlock1>();
                foreach (var block in blocks)
                {
                    var stock = AutoMapper.Mapper.Map<HoldingStock>(block);
                    info.HoldingStocks.Add(stock);
                }

                return info; 
#else
                var query = new EbestQuery<CSPAQ12300InBlock1>();
                if (query.ExecuteQueryAndWait(new CSPAQ12300InBlock1 { AcntNo = accountNum, Pwd = accountPw }) == false)
                {
                    _logger.Error($"Deposit query error, {GetLastErrorMessage(query.Result)}");
                    return null;
                }

                var info = AutoMapper.Mapper.Map<AccountInformation>(query.GetOutBlock<t0424OutBlock>());
#endif
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public List<string> GetAccountNumbers()
        {
            var accountNums = new List<string>();

            try
            {
                if (_session.WaitLogin() == false)
                {
                    _logger.Error("Not login state");
                    return accountNums;
                }

                var accCount = _session.GetAccountListCount();
                for (int i = 0; i < accCount; i++)
                {
                    var acc = _session.GetAccountList(i);
                    accountNums.Add(acc);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return accountNums;
        }
        
        public bool MakeOrder(Order order)
        {
            try
            {
                if (_session.WaitLogin() == false)
                {
                    _logger.Error("Not login state");
                    return false;
                }

                bool ret = false;

                if (string.IsNullOrEmpty(order.AccountPassword) == true)
                    order.AccountPassword = _loginInfo.AccountPassword;

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

            _logger.Error($"Order fail, {order.ToString()}");
            return false;
        }

        private bool MakeNewOrder(Order order)
        {
            try
            {
                var block = AutoMapper.Mapper.Map<CSPAT00600InBlock1>(order);

                var query = new EbestQuery<CSPAT00600InBlock1>();
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
                var block = AutoMapper.Mapper.Map<CSPAT00700InBlock1>(order);

                var query = new EbestQuery<CSPAT00700InBlock1>();
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
                var block = AutoMapper.Mapper.Map<CSPAT00800InBlock1>(order);

                var query = new EbestQuery<CSPAT00800InBlock1>();
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
            _session.LastCommTick = Environment.TickCount;

            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderCanceledReal_OutBlockReceived(SC3OutBlock block)
        {
            _session.LastCommTick = Environment.TickCount;

            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderModifiedReal_OutBlockReceived(SC2OutBlock block)
        {
            _session.LastCommTick = Environment.TickCount;

            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderConclusionReal_OutBlockReceived(SC1OutBlock block)
        {
            _session.LastCommTick = Environment.TickCount;

            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderSubmitReal_OutBlockReceived(SC0OutBlock block)
        {
            _session.LastCommTick = Environment.TickCount;

            var result = AutoMapper.Mapper.Map<OrderResult>(block);
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
