using CommonLib.Firm.Ebest.Block;
using CommonLib.Utility;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public abstract class QueryBase<TInBlock> 
        where TInBlock : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<string, QueryLimit> _queryLimitDic = new ConcurrentDictionary<string, QueryLimit>();

        public const int QUERY_TIMEOUT = 10000;

        private bool QueryDone { get; set; }

        private string _resName;
        public string ResName
        {
            get
            {
                if (string.IsNullOrEmpty(_resName) == true)
                {
                    var typeName = typeof(TInBlock).Name;
                    _resName = typeName.Substring(0, typeName.IndexOf("In"));
                }

                return _resName;
            }
        }

        public int Result { get; private set; }

        protected XAQueryClass Query { get; private set; }

        public QueryBase()
        {
            try
            {
                Query = new XAQueryClass();
                Query.ResFileName = $@"\Res\{ResName}.res";
                Query.ReceiveMessage += OnReceiveMessage;
                Query.ReceiveData += OnReceiveData;
                Query.ReceiveChartRealData += OnReceiveChartRealData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public virtual bool ExecuteQuery(TInBlock block)
        {
            try
            {
                if (block.BlockName.Contains(ResName) == false)
                    throw new ArgumentException($"Block not matched, {ResName}, {block.BlockName}");

                _logger.Info($"ExecuteQuery: {block.ToString()}");

                Query.SetFieldData(block);
                WaitQueryInterval();
                Result = Query.Request(false);
                return Result >= 0;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public virtual bool ExecuteQueryAndWait(TInBlock block, int timeout = QUERY_TIMEOUT)
        {
            try
            {
                if (block.BlockName.Contains(ResName) == false)
                    throw new ArgumentException($"Block not matched, {ResName}, {block.BlockName}");

                _logger.Info($"ExecuteQueryAndWait: {block.ToString()}");

                Query.SetFieldData(block);
                WaitQueryInterval();
                Result = Query.Request(false);
                if (Result < 0) return false;

                QueryDone = false;

                while (timeout > 0)
                {
                    timeout -= 10;
                    Thread.Sleep(10);
                    DispatcherUtility.DoEvents();

                    if (QueryDone == true) return true;
                }

                _logger.Error($"{ResName} query time out");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private void WaitQueryInterval()
        {
            if (_queryLimitDic.TryGetValue(ResName, out var limit) == false)
            {
                limit = new QueryLimit();
                limit.QueryName = ResName;
                limit.QueryInterval = (int)(1000 / Query.GetTRCountPerSec(ResName) * 1.1);

                _queryLimitDic.TryAdd(limit.QueryName, limit);
                return;
            }

            limit.WaitInterval();
        }

        protected virtual void OnReceiveChartRealData(string trCode)
        {
        }

        protected virtual void OnReceiveData(string trCode)
        {
            QueryDone = true;
        }

        protected virtual void OnReceiveMessage(bool isSystemError, string messageCode, string message)
        {
            _logger.Log(isSystemError ? LogLevel.Error : LogLevel.Info, $"{nameof(messageCode)}: {messageCode}, {nameof(message)}: {message}");
        }
    }
}
