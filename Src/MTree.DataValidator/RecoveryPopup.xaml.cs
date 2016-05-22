using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTree.DataValidator
{
    /// <summary>
    /// RecoveryPopup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecoveryPopup : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public DialogResult Result { get; set; }
        
        public RecoveryPopup()
        {
            InitializeComponent();
        }
        
        public void SetUri(string path)
        {
            if (File.Exists(path) == false)
                logger.Error($"{path} is not exist.");

            if (path != null)
                CompareViewer.Navigate(Path.Combine(Environment.CurrentDirectory, path));
        }

        private void Recovery_Click(object sender, RoutedEventArgs e)
        {
            Result = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
