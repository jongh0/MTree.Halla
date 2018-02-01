using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructure;

namespace Strategy.TradeDecision
{
    public class EvenOddTradeDecision : ITradeDecision
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = nameof(EvenOddTradeDecision);

        public TradeTypes DecideTradeType(Subscribable subscribable)
        {
            var result = TradeTypes.Hold;

            if (DateTime.Now.Day % 2 == 0)
                result = TradeTypes.Buy;
            else
                result = TradeTypes.Sell;

            _logger.Info($"[{Name}] DecideTradeType: {result}");
            return result;
        }
    }
}
