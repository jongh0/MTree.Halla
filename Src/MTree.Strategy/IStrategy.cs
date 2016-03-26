using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Strategy
{
    public interface IStrategy
    {
        string Name { get; set; }

        bool CanSell();

        bool CanBuy();
    }
}
