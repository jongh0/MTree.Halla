﻿using MTree.Configuration;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MTree.AutoLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Launcher launcher;

        private string _laucherInfo = string.Empty;
        public string LauncherInfo
        {
            get { return _laucherInfo; }
            set { _laucherInfo = value; NotifyPropertyChanged(nameof(LauncherInfo)); }
        }

        public MainWindow()
        {
            InitializeComponent();

            var now = DateTime.Now;
            launcher = new Launcher(ProcessTypes.RealTimeProvider);
            launcher.Time = new DateTime(now.Year, now.Month, now.Day, 7, 10, 0);
            launcher.PropertyChanged += Launcher_PropertyChanged;

            launcher.KillProcesses.Add(ProcessTypes.CybosStarter);
            launcher.KillProcesses.Add(ProcessTypes.DaishinPopupStopper);
            launcher.KillProcesses.Add(ProcessTypes.DaishinSessionManager);
            launcher.KillProcesses.Add(ProcessTypes.Ebest);
            launcher.KillProcesses.Add(ProcessTypes.Kiwoom);
            launcher.KillProcesses.Add(ProcessTypes.Daishin);
            launcher.KillProcesses.Add(ProcessTypes.HistorySaver);
            
            launcher.Start();
        }

        private void Launcher_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var l = sender as Launcher;

            if (e?.PropertyName == nameof(l.Time))
            {
                this.LauncherInfo = $"{l.LaunchProcess} | {l.Time.ToString()}";
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
