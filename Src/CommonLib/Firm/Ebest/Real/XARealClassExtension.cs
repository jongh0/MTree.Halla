using CommonLib.Attributes;
using CommonLib.Extensions;
using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Real
{
    public static class XARealClassExtension
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _getPropDic = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// XARealClass Field를 읽어서 Block Class를 만들어준다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="real"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool GetFieldData<T>(this XARealClass real, out T block) where T : BlockBase
        {
            block = Activator.CreateInstance<T>();

            try
            {
                var type = typeof(T);

                if (_getPropDic.TryGetValue(type, out var properties) == false)
                {
                    properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(
                        p => Attribute.IsDefined(p, typeof(PropertyIgnoreAttribute)) == false);
                    _getPropDic.TryAdd(type, properties);
                }

                foreach (var property in properties)
                {
                    var data = real.GetFieldData(block.BlockName, property.Name);
                    if (string.IsNullOrEmpty(data) == false)
                        property.SetValueByType(block, data);
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
