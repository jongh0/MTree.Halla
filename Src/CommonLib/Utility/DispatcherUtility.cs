using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CommonLib.Utility
{
    public static class DispatcherUtility
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Action을 Dispatcher에서 동기식으로 실행한다.
        /// </summary>
        /// <param name="action"></param>
        public static void InvokeOnDispatcher(Action action)
        {
            if (action == null) return;

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null) return;

            try
            {
                if (dispatcher.CheckAccess() == true)
                    action();
                else
                    dispatcher.Invoke(action);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// Action을 Dispatcher에서 비동기식으로 실행한다.
        /// </summary>
        /// <param name="action"></param>
        public static void BeginInvokeOnDispatcher(Action action)
        {
            if (action == null) return;

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null) return;

            try
            {
                if (dispatcher.CheckAccess() == true)
                    action();
                else
                    dispatcher.BeginInvoke(action);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// Message pumping
        /// WinForm의 Application.DoEvents()와 동일
        /// </summary>
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        public static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }
    }
}
