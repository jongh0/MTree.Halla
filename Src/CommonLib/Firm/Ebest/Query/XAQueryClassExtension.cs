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
                var type = block.GetType();

                if (_setPropDic.TryGetValue(type, out var properties) == false)
                {
                    properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty).Where(
                        p => Attribute.IsDefined(p, typeof(PropertyIgnoreAttribute)) == false);
                    _setPropDic.TryAdd(type, properties);
                }

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
                var type = typeof(T);

                if (_getPropDic.TryGetValue(type, out var properties) == false)
                {
                    properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(
                        p => Attribute.IsDefined(p, typeof(PropertyIgnoreAttribute)) == false);
                    _getPropDic.TryAdd(type, properties);
                }

                foreach (var property in properties)
                {
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
