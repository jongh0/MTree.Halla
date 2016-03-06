using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Provider
{
    public class BaseProvider
    {
        protected object lockObject;

        public BaseProvider()
        {
            lockObject = new object();
        }
    }
}
