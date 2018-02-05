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
        /// <typeparam name="TInBlock"></typeparam>
        /// <param name="query"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool SetFieldData<TInBlock>(this XARealClass query, TInBlock block) where TInBlock : BlockBase
        {
            if (block == null) return false;

            try
            {
                foreach (var property in PropertyUtility.GetProperties(typeof(TInBlock), typeof(IgnorePropertyAttribute)))
                {
                    var value = property.GetValue(block);
                    if (value == null) continue;

                    var str = value.ToString();
                    if (string.IsNullOrEmpty(str) == true) continue;

                    query.SetFieldData(block.BlockName, property.Name, str);
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
        /// <typeparam name="TOutBlock"></typeparam>
        /// <param name="real"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static bool GetFieldData<TOutBlock>(this XARealClass real, out TOutBlock block) where TOutBlock : BlockBase
        {
            block = Activator.CreateInstance<TOutBlock>();

            try
            {
                foreach (var property in PropertyUtility.GetProperties(typeof(TOutBlock), typeof(IgnorePropertyAttribute)))
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
