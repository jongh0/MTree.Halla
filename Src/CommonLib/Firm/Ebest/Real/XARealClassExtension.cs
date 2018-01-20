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

namespace CommonLib.Firm.Ebest.Real
{
    public static class XARealClassExtension
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Block Class의 내용을 XARealClass Field에 채워준다.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool SetFieldData(this XARealClass query, BlockBase block)
        {
            if (block == null) return false;

            try
            {
                foreach (var property in PropertyUtility.GetProperties(block.GetType()))
                {
                    query.SetFieldData(block.BlockName, property.Name, property.GetValue(block).ToString());
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
                foreach (var property in PropertyUtility.GetProperties(typeof(T)))
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
