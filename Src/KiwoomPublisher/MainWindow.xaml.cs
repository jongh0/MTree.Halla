﻿using AxKHOpenAPILib;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace KiwoomPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        KiwoomPublisher_ Publisher { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AxKHOpenAPI axKHOpenAPI = new AxKHOpenAPI();
                formsHost.Child = axKHOpenAPI;

                Publisher = new KiwoomPublisher_(axKHOpenAPI);
                this.DataContext = Publisher;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
