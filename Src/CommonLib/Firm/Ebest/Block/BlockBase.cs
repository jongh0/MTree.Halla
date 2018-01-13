using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    public abstract class BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public abstract string BlockName { get; }

        private const string _stringSeperator = ", "; // ToString()으로 로그 볼 때 편하라고..

        public override string ToString()
        {
            List<string> strList = new List<string>();
            strList.Add(BlockName);

            try
            {
                foreach (var property in GetType().GetProperties())
                {
                    object value = property.GetValue(this);

                    if (value is DateTime dateTime)
                        strList.Add($"{property.Name}: {dateTime.ToString(Config.General.DateTimeFormat)}");
                    else
                        strList.Add($"{property.Name}: {value}");
                }

                return string.Join(_stringSeperator, strList.ToArray());
            }
            catch
            {
            }
            finally
            {
                strList.Clear();
            }

            return string.Empty;
        }
    }
}
