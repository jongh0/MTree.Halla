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

namespace EbestResPropertyGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder sb = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var str = textBox.Text;

            var lines = str.Split(';');

            foreach (var l in lines)
            {
                var values = l.Split(',');
                AddProperty(values);
            }

            textBox.Text = sb.ToString();
        }

        /// <summary>
        /// 계좌번호
        /// </summary>
        //public string AcntNo { get; set; }


        private void AddProperty(string[] values)
        {
            if (values.Length != 5) return;
            sb.AppendLine($"/// <summary>");
            sb.AppendLine($"/// {values[0].Trim()}");
            sb.AppendLine($"/// </summary>");
            sb.AppendLine($"public {values[3].Trim()} {values[1].Trim()} {{ get; set; }}\n");
        }
    }
}
