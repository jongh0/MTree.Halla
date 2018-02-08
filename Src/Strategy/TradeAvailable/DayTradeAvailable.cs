using DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader;
using Trader.Account;

namespace Strategy.TradeAvailable
{
    public class DayTradeAvailable : ITradeAvailable
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = nameof(DayTradeAvailable);

        private bool _tradeDone = false;

        private DateTime _tradeStartTime;

        public DayTradeAvailable()
        {
            var now = DateTime.Now;
            _tradeStartTime = new DateTime(now.Year, now.Month, now.Day, 9, 30, 0);
        }

        public bool CanBuy(TradeInformation info)
        {
            try
            {
                if (info == null) throw new ArgumentNullException(nameof(info));

                if (info.Subscribable is StockConclusion conclusion)
                {
                    if (conclusion.Time < _tradeStartTime)
                        return false;

                    if (_tradeDone == false)
                    {
                        _tradeDone = true;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool CanSell(TradeInformation info)
        {
            try
            {
                if (info == null) throw new ArgumentNullException(nameof(info));

                if (info.Subscribable is StockConclusion conclusion)
                {
                    if (conclusion.Time < _tradeStartTime)
                        return false;

                    if (_tradeDone == false)
                    {
                        _tradeDone = true;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
    }
}
