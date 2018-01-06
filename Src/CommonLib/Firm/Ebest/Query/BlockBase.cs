﻿using CommonLib.Attributes;
using CommonLib.Extensions;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public abstract class BlockBase
    {
        public override string ToString()
        {
            List<string> strList = new List<string>();

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

                return string.Join(", ", strList.ToArray());
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
