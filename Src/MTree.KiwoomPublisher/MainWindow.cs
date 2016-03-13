using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTree.KiwoomPublisher
{
    public partial class MainWindow : Form
    {
        KiwoomPublisher kiwoomPublisher;
        public MainWindow()
        {
            InitializeComponent();

            kiwoomPublisher = new KiwoomPublisher(axKHOpenAPI);
#if false
            Task.Run(() => {
                System.Threading.Thread.Sleep(1000);
                StockMaster master = new StockMaster();
                kiwoomPublisher.GetQuote("000087", ref master);
            });
#endif
        }
    }
}
