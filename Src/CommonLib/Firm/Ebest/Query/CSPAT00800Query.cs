﻿using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public class CSPAT00800Query : QueryBase
    {
        public override string ResName => "CSPAT00800";

        public event Action<CSPAT00800OutBlock1> Block1Received;
        public event Action<CSPAT00800OutBlock2> Block2Received;

        private CSPAT00800OutBlock1 _block1;
        public CSPAT00800OutBlock1 Block1 => _block1;

        private CSPAT00800OutBlock2 _block2;
        public CSPAT00800OutBlock2 Block2 => _block2;

        protected override void OnReceiveData(string trCode)
        {
            if (Block1Received != null && Query.GetFieldData(out _block1) == true)
                Block1Received.Invoke(_block1);

            if (Block2Received != null && Query.GetFieldData(out _block2) == true)
                Block2Received.Invoke(_block2);

            base.OnReceiveData(trCode);
        }
    }
}
