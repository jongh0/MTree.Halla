using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTree.EbestPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EbestPublisher Publisher { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Publisher = new EbestPublisher();

#if false
            Task.Run(() =>
            {
                Publisher.Waitlogin();

                DataStructure.StockMaster master = new DataStructure.StockMaster();
                int startTick = Environment.TickCount;
                Publisher.GetQuote("000020", ref master);
                Trace.WriteLine(">>>>>>>>>>>>>>> " + (Environment.TickCount - startTick));
                Debugger.Break();
            });
#endif
        }
    }
}
