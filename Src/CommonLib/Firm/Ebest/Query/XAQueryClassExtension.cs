using CommonLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public static class XAQueryClassExtension
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool SetFieldData(this XAQueryClass query, BlockBase block)
        {
            if (block == null) return false;

            try
            {
                var type = block.GetType();
                var blockName = type.Name;

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
                {
                    if (Attribute.IsDefined(property, typeof(ParseIgnoreAttribute)) == true) continue;

                    query.SetFieldData(blockName, property.Name, 0, property.GetValue(block).ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
    }
}
