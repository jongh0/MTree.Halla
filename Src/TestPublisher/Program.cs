using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestPublisher
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var publisher = new Publisher();

            Thread.Sleep(1000);
            publisher.StartPublising();

            Console.ReadLine();

            publisher.StopPublishing();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error((Exception)e.ExceptionObject, $"Unhandled exception, IsTerminating: {e.IsTerminating}");
        }
    }
}
