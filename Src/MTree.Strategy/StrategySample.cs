using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Strategy
{
    public class StrategySample : IStrategy
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = "StrategySample";

        public bool CanBuy()
        {
            try
            {

                logger.Info($"[{Name}] CanBuy: true");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Info($"[{Name}] CanBuy: false");
            return false;
        }

        public bool CanSell()
        {
            try
            {
                logger.Info($"[{Name}] CanSell: true");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Info($"[{Name}] CanSell: false");
            return false;
        }
    }
}
