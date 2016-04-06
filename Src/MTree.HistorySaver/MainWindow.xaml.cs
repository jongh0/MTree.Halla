﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MTree.HistorySaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HistorySaver Consumer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Consumer = new HistorySaver();
            //TestData();

            this.DataContext = Consumer;
        }

        private void TestData()
        {
            Consumer.Counter.StockMasterCount = 100000;

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                Consumer.Counter.Increment(DataStructure.CounterTypes.CircuitBreak);
                Consumer.Counter.Increment(DataStructure.CounterTypes.CircuitBreak);
                Consumer.Counter.Increment(DataStructure.CounterTypes.CircuitBreak);

                Thread.Sleep(1000);
                Consumer.Counter.Increment(DataStructure.CounterTypes.StockConclusion);
                Consumer.Counter.Increment(DataStructure.CounterTypes.StockConclusion);
                Consumer.Counter.Increment(DataStructure.CounterTypes.StockConclusion);
                Consumer.Counter.Increment(DataStructure.CounterTypes.StockConclusion);
            });
        }
    }
}
