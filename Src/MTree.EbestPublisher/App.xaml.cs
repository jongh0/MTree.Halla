using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MTree.EbestPublisher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //EbestPublisher publisher = new EbestPublisher();
            //publisher.Login();
            //Task.Run(() => {
            //    System.Threading.Thread.Sleep(1000);
            //    DataStructure.StockMaster master = new DataStructure.StockMaster();
            //    publisher.GetQuote("000020", ref master);
            //});
        }
    }
}
