using CommonLib.Firm.Ebest.Block;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public abstract class QueryBase<TInBlock> : IDisposable 
        where TInBlock : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public const int QUERY_TIMEOUT = 15000;

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
        protected AutoResetEvent WaitQuery { get; private set; } = new AutoResetEvent(false);

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

                _logger.Info($"ExecuteQuery\n{block.ToString()}");

                Query.SetFieldData(block);
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

                _logger.Info($"ExecuteQueryAndWait\n{block.ToString()}");

                Query.SetFieldData(block);
                Result = Query.Request(false);
                if (Result < 0) return false;

                if (WaitQuery.WaitOne(timeout) == false)
                {
                    _logger.Error($"{ResName} query time out");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        protected virtual void OnReceiveChartRealData(string trCode)
        {
        }

        protected virtual void OnReceiveData(string trCode)
        {
            WaitQuery.Set();
        }

        protected virtual void OnReceiveMessage(bool isSystemError, string messageCode, string message)
        {
            _logger.Log(isSystemError ? LogLevel.Error : LogLevel.Info, $"{nameof(messageCode)}: {messageCode}, {nameof(message)}: {message}");
        }

        public int GetTRCountPerSec()
        {
            return Query.GetTRCountPerSec(ResName);
        }

        #region IDisposable
        private bool disposed = false;

        ~QueryBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }

                try
                {
                    WaitQuery.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}
