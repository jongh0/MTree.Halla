using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy
{
    public interface IStrategy
    {
        string Name { get; set; }

        bool CanSell();

        bool CanBuy();
    }
}
