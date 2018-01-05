using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class TaskUtility
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static Task Run(string taskName, CancellationToken cancelToken, Action action)
        {
            var _taskName = taskName;
            var _cancelToken = cancelToken;
            var _action = action;

            return Task.Run(() =>
            {
                _logger.Info($"{_taskName} task started");

                while (true)
                {
                    try
                    {
                        _cancelToken.ThrowIfCancellationRequested();
                        _action?.Invoke();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }

                _logger.Info($"{_taskName} task stopped");
            }, _cancelToken);
        }
    }
}
