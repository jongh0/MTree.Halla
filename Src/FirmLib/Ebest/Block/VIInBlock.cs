﻿using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Block
{
    /// <summary>
    /// VI발동해제
    /// </summary>
    public class VIInBlock : BlockBase
    {
        [IgnoreProperty]
        public override string BlockName => "InBlock";

        /// <summary>
        /// 단축코드(KEY) [6]
        /// </summary>
        public string shcode { get; set; }
    }
}
