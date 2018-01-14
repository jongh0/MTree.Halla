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
        public static bool SetFieldData(this XAQueryClass query, BlockBase block)
        {
            if (block == null) return false;

            try
            {
                var properties = PropertyUtility.GetProperties(block.GetType(), typeof(PropertyIgnoreAttribute));
                foreach (var property in properties)
                {
                    query.SetFieldData(block.BlockName, property.Name, 0, property.GetValue(block).ToString());
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
                var properties = PropertyUtility.GetProperties(typeof(T), typeof(PropertyIgnoreAttribute));
                foreach (var property in properties)
                {
                    var data = query.GetFieldData(block.BlockName, property.Name, index);
                    if (string.IsNullOrEmpty(data) == false && property.CanWrite == true)
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
