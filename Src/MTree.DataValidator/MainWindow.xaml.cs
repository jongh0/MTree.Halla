using MongoDB.Driver;
using MTree.DataStructure;
using MTree.DbProvider;
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

namespace MTree.DataValidator
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg == "/R")
                {
                    Task.Run(() =>
                    {
                        DateTime targetDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                        DataValidator validator = new DataValidator();

                        validator.ValidateCodeList();
                        validator.ValidateMasterCompare(targetDate);
                        validator.ValidateStockConclusionCompare(targetDate);
                        validator.ValidateIndexConclusionCompare(targetDate);
                        validator.ValidateCircuitBreakCompare(targetDate);
                        //validator.ValidateStockConclusionCompareWithDaishin(target);
                    });
                }
            }
        }
    }
}
