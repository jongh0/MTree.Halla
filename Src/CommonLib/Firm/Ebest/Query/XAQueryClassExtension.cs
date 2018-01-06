using CommonLib.Attributes;
using CommonLib.Extensions;
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
                var blockName = type.Name;

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
                {
                    //if (Attribute.IsDefined(property, typeof(PropertyIgnoreAttribute)) == true) continue;

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

        /// <summary>
        /// XAQueryClass Field를 읽어서 Block Class를 만들어준다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool GetFieldData<T>(this XAQueryClass query, out T block) where T : BlockBase
        {
            block = Activator.CreateInstance<T>();

            var type = typeof(T);
            var blockName = type.Name;

            try
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    //if (Attribute.IsDefined(property, typeof(PropertyIgnoreAttribute)) == true) continue;

                    var data = query.GetFieldData(blockName, property.Name, 0);
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
