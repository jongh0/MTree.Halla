using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class ServiceUtility
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool IsServiceRunning(string serviceName)
        {
            ServiceController controller = new ServiceController(serviceName);
            return controller.Status == ServiceControllerStatus.Running;
        }

        public static bool StartService(string serviceName, int waitSec = 60)
        {
            try
            {
                _logger.Info($"Starting {serviceName} Service");

                ServiceController controller = new ServiceController(serviceName);
                controller.Start();
                controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(waitSec * 1000 * 1000));

                _logger.Info($"Starting {serviceName} Service done. Service Status: {controller.Status}");

                return controller.Status == ServiceControllerStatus.Running;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public static bool StopService(string serviceName, int waitSec = 60)
        {
            try
            {
                _logger.Info($"Stopping {serviceName} Service");

                ServiceController controller = new ServiceController(serviceName);
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(waitSec * 1000 * 1000));

                _logger.Info($"Stopping {serviceName} Service done. Service Status: {controller.Status}");

                return controller.Status == ServiceControllerStatus.Stopped;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
    }
}
