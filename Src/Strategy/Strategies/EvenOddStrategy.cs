using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.Strategies
{
    public class EvenOddStrategy : IStrategy
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = nameof(EvenOddStrategy);

        public bool CanBuy()
        {
            try
            {
                if (DateTime.Now.Day % 2 == 0)
                {
                    _logger.Info($"[{Name}] CanBuy: true");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            _logger.Info($"[{Name}] CanBuy: false");
            return false;
        }

        public bool CanSell()
        {
            try
            {
                if (DateTime.Now.Day % 2 == 1)
                {
                    _logger.Info($"[{Name}] CanSell: true");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            _logger.Info($"[{Name}] CanSell: false");
            return false;
        }
    }
}
