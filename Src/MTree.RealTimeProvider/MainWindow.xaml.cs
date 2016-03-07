using System.Windows;
using System.ServiceModel;

namespace MTree.RealTimeProvider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServiceHost RealTimeHost { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            RealTimeHost = new ServiceHost(typeof(RealTimeProvider));
            RealTimeHost.Open();
        }
    }
}
