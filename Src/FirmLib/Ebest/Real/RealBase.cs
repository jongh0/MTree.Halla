using FirmLib.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace FirmLib.Ebest.Real
{
    public abstract class RealBase<TOutBlock> 
        where TOutBlock : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public event Action<TOutBlock> OutBlockReceived;

        private string _resName;
        public string ResName
        {
            get
            {
                if (string.IsNullOrEmpty(_resName) == true)
                {
                    var typeName = typeof(TOutBlock).Name;
                    _resName = typeName.Substring(0, typeName.IndexOf("Out"));
                }

                return _resName;
            }
        }

        protected XARealClass Real { get; private set; }

        public RealBase()
        {
            try
            {
                Real = new XARealClass();
                Real.ResFileName = $@"\Res\{ResName}.res";
                Real.ReceiveRealData += OnReceiveRealData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void OnReceiveRealData(string trCode)
        {
            if (OutBlockReceived != null && Real.GetFieldData(out TOutBlock block) == true)
            {
                _logger.Info($"OnReceiveRealData\n{block.ToString()}");
                OutBlockReceived.Invoke(block);
            }
        }
    }
}
