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
    public abstract class XAQueryBase<T> : IDisposable where T : BlockBase
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
                    _resName = typeName.Substring(0, typeName.IndexOf("In"));
                }

                return _resName;
            }
        }

        protected XAQueryClass Query { get; private set; }
        protected AutoResetEvent QueryWaiter { get; private set; } = new AutoResetEvent(false);
        public int QueryTimeout { get; set; }

        public XAQueryBase()
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

        public int ExecuteQuery(T block)
        {
            try
            {
                if (block.BlockName.Contains(ResName) == false)
                    throw new ArgumentException($"Block not matched, {ResName}, {block.BlockName}");

                Query.SetFieldData(block);
                return Query.Request(false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return 0;
        }

        public int ExecuteQueryAndWait(T block)
        {
            try
            {
                if (block.BlockName.Contains(ResName) == false)
                    throw new ArgumentException($"Block not matched, {ResName}, {block.BlockName}");

                Query.SetFieldData(block);

                var result = Query.Request(false);
                if (result < 0) return result;

                QueryWaiter.WaitOne(QueryTimeout);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return 0;
        }

        protected virtual void OnReceiveChartRealData(string trCode)
        {
        }

        protected virtual void OnReceiveData(string trCode)
        {
            QueryWaiter.Set();
        }

        protected virtual void OnReceiveMessage(bool isSystemError, string messageCode, string message)
        {
            _logger.Log(isSystemError ? LogLevel.Error : LogLevel.Info, $"{nameof(messageCode)}: {messageCode}, {nameof(message)}: {message}");
        }

        #region IDisposable
        private bool disposed = false;

        ~XAQueryBase()
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
                    QueryWaiter.Dispose();
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
