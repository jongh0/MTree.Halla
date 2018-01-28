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
using Trader.Account;
using Configuration;

namespace EbestTrader
{
    public partial class EbestTrader_
    {
        public List<AccountInformation> GetAccountInformations()
        {
            var accountInfos = new List<AccountInformation>();

            try
            {
                if (LoginInfo.State != LoginStates.Login)
                {
                    _logger.Error("Not login state");
                    return accountInfos;
                }

                var accountNums = GetAccountNumbers();
                if (accountNums == null) return accountInfos;

                foreach (var accountNum in accountNums)
                {
                    var info = GetAccountInfo(accountNum, LoginInfo.AccountPw);
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
                var query = new EbestQuery<t0424InBlock, t0424OutBlock, t0424OutBlock1>();
                if (query.ExecuteQueryAndWait(new t0424InBlock { accno = accountNum, passwd = accountPw }) == false)
                {
                    _logger.Error($"Deposit query error, {GetLastErrorMessage(query.Result)}");
                    return null;
                }

                var info = AutoMapper.Mapper.Map<AccountInformation>(query.OutBlock);

                var blocks = query.GetAllOutBlock1();
                foreach (var block in blocks)
                {
                    var stock = AutoMapper.Mapper.Map<HoldingStock>(block);
                    info.HoldingStocks.Add(stock);
                }

                return info;
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
                if (LoginInfo.State != LoginStates.Login)
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
                if (LoginInfo.State != LoginStates.Login)
                {
                    _logger.Error("Not login state");
                    return false;
                }

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

            _logger.Error($"Order fail, {order.ToString()}");
            return false;
        }

        private bool MakeNewOrder(Order order)
        {
            try
            {
                var block = AutoMapper.Mapper.Map<CSPAT00600InBlock1>(order);

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
                var block = AutoMapper.Mapper.Map<CSPAT00700InBlock1>(order);

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
                var block = AutoMapper.Mapper.Map<CSPAT00800InBlock1>(order);

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
            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderCanceledReal_OutBlockReceived(SC3OutBlock block)
        {
            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderModifiedReal_OutBlockReceived(SC2OutBlock block)
        {
            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderConclusionReal_OutBlockReceived(SC1OutBlock block)
        {
            var result = AutoMapper.Mapper.Map<OrderResult>(block);
            NotifyOrderResult(result);
        }

        private void OrderSubmitReal_OutBlockReceived(SC0OutBlock block)
        {
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
