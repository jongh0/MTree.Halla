using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Strategy
{
    public enum LogicTypes
    {
        AND,
        OR,
    }

    public class StrategyComposite : IStrategy
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = "StrategyComposite";

        public LogicTypes Logic { get; set; } = LogicTypes.AND;

        private List<IStrategy> StrategyList = new List<IStrategy>();

        public void Add(IStrategy strategy)
        {
            StrategyList.Add(strategy);
        }

        public bool CanBuy()
        {
            bool ret = false;

            try
            {
                foreach (var s in StrategyList)
                {
                    ret = s.CanBuy();

                    if (ret == false && Logic == LogicTypes.AND)
                    {
                        logger.Info($"[{Name}/{Logic}] CanBuy: false");
                        return false;
                    }
                    else if (ret == true && Logic == LogicTypes.OR)
                    {
                        logger.Info($"[{Name}/{Logic}] CanBuy: true");
                        return true;
                    }
                }

                ret = (Logic == LogicTypes.AND);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ret = false;
            }

            logger.Info($"[{Name}/{Logic}] CanBuy: {ret}");
            return ret;
        }

        public bool CanSell()
        {
            bool ret = false;

            try
            {
                foreach (var s in StrategyList)
                {
                    ret = s.CanSell();

                    if (ret == false && Logic == LogicTypes.AND)
                    {
                        logger.Info($"[{Name}/{Logic}] CanSell: false");
                        return false;
                    }
                    else if (ret == true && Logic == LogicTypes.OR)
                    {
                        logger.Info($"[{Name}/{Logic}] CanSell: true");
                        return true;
                    }
                }

                ret = (Logic == LogicTypes.AND);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ret = false;
            }

            logger.Info($"[{Name}/{Logic}] CanSell: {ret}");
            return ret;
        }
    }
}
