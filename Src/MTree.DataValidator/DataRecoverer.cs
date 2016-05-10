using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataValidator
{
    public class DataRecoverer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private DataValidator _Validator;
        public DataValidator Validator
        {
            get
            {
                return _Validator;
            }
            set
            {
                _Validator = value;
            }
        }
        
        public DataRecoverer()
        {
        }

        public void RecoverMaster(DateTime target, string code, IDataCollector from, IDataCollector to)
        {
            if (Validator.ValidateMaster(target, code, false) == true)
            {
                logger.Info($"Master of {code} is same");
                return;
            }
        }
    }
}
