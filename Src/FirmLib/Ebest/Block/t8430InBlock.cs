﻿using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Block
{
    /// <summary>
    /// 주식종목조회
    /// </summary>
    public class t8430InBlock : BlockBase
    {
        /// <summary>
        /// 구분(0:전체1:코스피2:코스닥) [1]
        /// </summary>
        public string gubun { get; set; }
    }
}
