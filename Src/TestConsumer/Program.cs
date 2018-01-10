using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsumer
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var consumer = new TestConsumer();

            Thread.Sleep(1000);
            consumer.StartConsume();

            Console.ReadLine();
            consumer.StopConsume();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error((Exception)e.ExceptionObject, $"Unhandled exception, IsTerminating: {e.IsTerminating}");
        }
    }
}
