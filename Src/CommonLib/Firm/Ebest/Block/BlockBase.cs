using CommonLib.Attribute;
using CommonLib.Utility;
using Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    public abstract class BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _propDic = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        public abstract string BlockName { get; }

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this);
        }
    }
}
