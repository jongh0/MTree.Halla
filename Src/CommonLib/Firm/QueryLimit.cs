using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Firm
{
    public class QueryLimit
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string QueryName { get; set; }

        public int QueryInterval { get; set; }

        public int LastQueryTick { get; set; } = Environment.TickCount;

        public void WaitInterval()
        {
            if (QueryInterval <= 0) return;

            var remain = QueryInterval - (Environment.TickCount - LastQueryTick);
            if (remain > 0)
                _logger.Info($"WaitInterval, {nameof(QueryName)}: {QueryName} ({remain}/{QueryInterval})");

            while (true)
            {
                remain = QueryInterval - (Environment.TickCount - LastQueryTick);
                if (remain <= 0)
                {
                    LastQueryTick = Environment.TickCount;
                    return;
                }

                Thread.Sleep(10);
            }
        }
    }
}
