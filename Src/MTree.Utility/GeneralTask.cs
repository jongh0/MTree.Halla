using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class GeneralTask
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static Task Run(string taskName, CancellationToken cancelToken, Action action)
        {
            var _taskName = taskName;
            var _cancelToken = cancelToken;
            var _action = action;

            return Task.Run(() =>
            {
                logger.Info($"{_taskName} task started");

                while (true)
                {
                    try
                    {
                        _cancelToken.ThrowIfCancellationRequested();
                        _action();
                    }
                    catch (OperationCanceledException)
                    {
                        logger.Info($"{_taskName} task canceled");
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                logger.Info($"{_taskName} task stopped");
            }, _cancelToken);
        }
    }
}
