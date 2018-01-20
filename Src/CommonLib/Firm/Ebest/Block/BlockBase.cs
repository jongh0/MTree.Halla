﻿using CommonLib.Attribute;
using CommonLib.Utility;
using Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    public abstract class BlockBase
    {
        private string _blockName;

        [PropertyIgnore]
        public virtual string BlockName
        {
            get
            {
                if (string.IsNullOrEmpty(_blockName) == true)
                    _blockName = GetType().Name;

                return _blockName;
            }
        }

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this, Environment.NewLine);
        }
    }
}
