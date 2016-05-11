using MTree.DataStructure;
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
        
        public DbAgent From { get; set; }

        public DbAgent To { get; set; }
        
        public void RecoverMaster(DateTime target, string code)
        {
            To.Insert((new DbCollector(From)).GetMaster(target, code));
        }
        public void RecoverMasters(DateTime target)
        {
            DbCollector collector = new DbCollector(From);
            foreach (string code in collector.GetStockCodeList())
            {
                To.Insert(collector.GetMaster(target, code));
            }
        }
        public void RecoverStockConclusion(DateTime target, string code)
        {
            DbCollector collector = new DbCollector(From);
            List<Subscribable> conclusions = collector.GetStockConclusions(target, code, false);
            foreach(Subscribable conclusion in conclusions)
            {
                To.Insert(conclusion);
            }
        }
        public void RecoverIndexConclusion(DateTime target, string code)
        {
            DbCollector collector = new DbCollector(From);
            List<Subscribable> conclusions = collector.GetIndexConclusions(target, code, false);
            foreach (Subscribable conclusion in conclusions)
            {
                To.Insert(conclusion);
            }
        }
        public void RecoverCircuitBreak(DateTime target, string code)
        {
            DbCollector collector = new DbCollector(From);
            List<Subscribable> cbs = collector.GetCircuitBreaks(target, code);
            foreach (Subscribable cb in cbs)
            {
                To.Insert(cb);
            }
        }
    }
}
