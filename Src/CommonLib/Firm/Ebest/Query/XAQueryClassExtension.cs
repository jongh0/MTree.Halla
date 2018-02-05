using CommonLib.Attribute;
using CommonLib.Extension;
using CommonLib.Firm.Ebest.Block;
using CommonLib.Utility;
using System;
using System.Collections.Concurrent;
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

        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _setPropDic = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _getPropDic = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        /// <summary>
        /// Block Class의 내용을 XAQueryClass Field에 채워준다.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool SetFieldData<T>(this XAQueryClass query, T block) where T : BlockBase
        {
            if (block == null) return false;

            try
            {
                foreach (var property in PropertyUtility.GetProperties(typeof(T), typeof(IgnorePropertyAttribute)))
                {
                    var value = property.GetValue(block);
                    if (value == null) continue;

                    var str = value.ToString();
                    if (string.IsNullOrEmpty(str) == true) continue;

                    query.SetFieldData(block.BlockName, property.Name, 0, str);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// XAQueryClass Field를 읽어서 Block Class를 만들어준다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool GetFieldData<T>(this XAQueryClass query, out T block, int index = 0) where T : BlockBase
        {
            block = Activator.CreateInstance<T>();

            try
            {
                foreach (var property in PropertyUtility.GetProperties(typeof(T), typeof(IgnorePropertyAttribute)))
                {
                    if (property.CanWrite == false) continue;

                    var data = query.GetFieldData(block.BlockName, property.Name, index);
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
