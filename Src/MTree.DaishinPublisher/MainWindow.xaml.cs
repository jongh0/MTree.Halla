using MTree.DataStructure;
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

namespace MTree.DaishinPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DaishinPublisher Publisher { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Publisher = new DaishinPublisher();
            //Publisher.SubscribeWorldStock("ENXH");
            //bool result = Publisher.SubscribeStock("A005930");
			/*
            Dictionary<string, CodeEntity> list = Publisher.GetCodeList();
            for (int i = 0; i < 200; i++)
            {
                bool result = Publisher.SubscribeStock("A" + list.ToArray()[i].Key);
            }
			*/
        }
    }
}
