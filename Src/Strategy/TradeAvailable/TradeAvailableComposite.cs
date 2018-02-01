using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader;
using Trader.Account;

namespace Strategy.TradeAvailable
{
    public class TradeAvailableComposite : ITradeAvailable
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = nameof(TradeAvailableComposite);

        public LogicTypes Logic { get; set; } = LogicTypes.AND;

        private List<ITradeAvailable> _strategyList = new List<ITradeAvailable>();

        public void Add(ITradeAvailable strategy)
        {
            _strategyList.Add(strategy);
        }

        public bool CanBuy(AccountInformation accInfo, Order order)
        {
            bool ret = false;

            try
            {
                foreach (var s in _strategyList)
                {
                    ret = s.CanBuy(accInfo, order);

                    if (ret == false && Logic == LogicTypes.AND)
                    {
                        _logger.Info($"[{Name}/{Logic}] CanBuy: false");
                        return false;
                    }
                    else if (ret == true && Logic == LogicTypes.OR)
                    {
                        _logger.Info($"[{Name}/{Logic}] CanBuy: true");
                        return true;
                    }
                }

                ret = (Logic == LogicTypes.AND);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ret = false;
            }

            _logger.Info($"[{Name}/{Logic}] CanBuy: {ret}");
            return ret;
        }

        public bool CanSell(AccountInformation accInfo, Order order)
        {
            bool ret = false;

            try
            {
                foreach (var s in _strategyList)
                {
                    ret = s.CanSell(accInfo, order);

                    if (ret == false && Logic == LogicTypes.AND)
                    {
                        _logger.Info($"[{Name}/{Logic}] CanSell: false");
                        return false;
                    }
                    else if (ret == true && Logic == LogicTypes.OR)
                    {
                        _logger.Info($"[{Name}/{Logic}] CanSell: true");
                        return true;
                    }
                }

                ret = (Logic == LogicTypes.AND);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ret = false;
            }

            _logger.Info($"[{Name}/{Logic}] CanSell: {ret}");
            return ret;
        }
    }
}
