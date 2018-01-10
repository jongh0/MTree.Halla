using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public class TraderContract
    {
        public IRealTimeTraderCallback Callback { get; set; } = null;
    }
}
