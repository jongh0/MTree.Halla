using DataStructure;
using System;
using System.Collections.Generic;
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

namespace DaishinPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DaishinPublisher_ Publisher { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;


            Publisher = new DaishinPublisher_();
            this.DataContext = Publisher;

            //TestBidding();
            //TestSubscribingSingle();
            //TestCodeListAndSubscribing();
            //TestEtfSubscribing();
        }

        void TestSubscribingSingle()
        {
            bool result = Publisher.SubscribeStock("A005930");
        }

        void TestCodeListAndSubscribing()
        {
            Dictionary<string, CodeEntity> list = Publisher.GetCodeList();
            foreach (KeyValuePair<string, CodeEntity> entity in list)
            {
                if (entity.Value.MarketType == MarketTypes.KOSPI)
                {
                    if (Publisher.IsSubscribable() == true)
                        Publisher.SubscribeStock("A" + entity.Value.Code);
                }
                
            }
        }
        void TestBidding()
        {
            Publisher.SubscribeBidding("A000020");
        }
        void TestEtfSubscribing()
        {
            Publisher.SubscribeStock("A122630");
            Publisher.SubscribeETF("A122630");
        }
    }
}
