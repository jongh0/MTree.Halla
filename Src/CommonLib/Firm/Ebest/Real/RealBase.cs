using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Real
{
    public abstract class RealBase<T> where T : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private string _resName;
        public string ResName
        {
            get
            {
                if (string.IsNullOrEmpty(_resName) == true)
                {
                    var typeName = typeof(T).Name;
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
        }

        public void AdviseRealData()
        {
            try
            {
                Real?.AdviseRealData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void UnadviseRealData()
        {
            try
            {
                Real?.UnadviseRealData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
