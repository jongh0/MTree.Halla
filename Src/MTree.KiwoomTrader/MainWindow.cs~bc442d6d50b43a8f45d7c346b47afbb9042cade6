using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTree.KiwoomTrader
{
    public partial class MainWindow : Form
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        ServiceHost Host { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var instance = new KiwoomTrader();
            Host = new ServiceHost(instance);
            Host.Opened += Host_Opened;
            Host.Closed += Host_Closed;
            Host.Faulted += Host_Faulted;
            Host.Open();
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            logger.Error("Host faulted");
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            logger.Info("Host closed");
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            logger.Info("Host opened");

            Task.Run(() =>
            {
            });
        }
    }
}
