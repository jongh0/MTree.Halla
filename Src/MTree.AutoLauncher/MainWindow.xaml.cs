using System.IO;
using System.Reflection;
using System.Windows;

namespace MTree.AutoLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            
            var buildTime = new FileInfo(Assembly.GetEntryAssembly().Location).LastWriteTime;
            Title += $" Built at {buildTime}";
            _logger.Info($"Build Time : {buildTime}");
        }
    }
}
