using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.Strategies
{
    public class SampleStrategy : IStrategy
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = nameof(SampleStrategy);

        public bool CanBuy()
        {
            try
            {

                _logger.Info($"[{Name}] CanBuy: true");
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
                _logger.Info($"[{Name}] CanSell: true");
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
