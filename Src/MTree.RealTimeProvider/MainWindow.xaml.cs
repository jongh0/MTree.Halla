using System.Windows;
using System.ServiceModel;

namespace MTree.RealTimeProvider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServiceHost host;

        public MainWindow()
        {
            InitializeComponent();

            host = new ServiceHost(typeof(RealTimeProvider));
            host.Open();
        }
    }
}
