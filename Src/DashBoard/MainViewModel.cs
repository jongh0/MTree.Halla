using GalaSoft.MvvmLight.Command;
using Consumer;
using DataStructure;
using DbProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dashboard
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private ConsumerBase consumer { get; set; }

        private Dashboard_ dashboard;
        public Dashboard_ Dashboard
        {
            get
            {
                return dashboard;
            }
            set
            {
                dashboard = value;
                NotifyPropertyChanged(nameof(Dashboard));
            }
        }
        
        public Visibility SimulControlVisibility
        {
            get
            {
                if (consumer is HistoryConsumer)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
        }

        private DateTime _StartingDate = DateTime.Now;
        public DateTime StartingDate
        {
            get { return _StartingDate; }
            set
            {
                _StartingDate = value;

                if (EndingDate < _StartingDate)
                    EndingDate = _StartingDate;

                NotifyPropertyChanged(nameof(StartingDate));
            }
        }

        private DateTime _EndingDate = DateTime.Now;
        public DateTime EndingDate
        {
            get { return _EndingDate; }
            set
            {
                _EndingDate = value;

                if (_EndingDate < StartingDate)
                    StartingDate = _EndingDate;

                NotifyPropertyChanged(nameof(EndingDate));
            }
        }

        #region Command
        private RelayCommand _startSimulationCommand;
        public ICommand StartSimulationCommand
        {
            get
            {
                if (_startSimulationCommand == null)
                {
                    _startSimulationCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        DataLoader loader = new DataLoader();
                        CodeMapDbObject result = loader.Load<CodeMapDbObject>("CodeMap", StartingDate, EndingDate)[0];
                        ICodeMap codemap = CodeMapConverter.JsonStringToCodeMap(result.Code, result.CodeMap);

                        if (consumer is ISimulation)
                        {
                            string[] codes = DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s).ToArray();
                            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                            {
                                ((ISimulation)consumer).StartSimulation(codes, targetDate);
                            }
                        }
                    }));
                }

                return _startSimulationCommand;
            }
        }

        private RelayCommand<DashboardItem> _doubleClickCommand;
        public ICommand DoubleClickCommand => _doubleClickCommand ?? (_doubleClickCommand = new RelayCommand<DashboardItem>((param) => ExecuteDoubleClick(param)));

        public void ExecuteDoubleClick(DashboardItem item)
        {
            Process.Start("https://search.naver.com/search.naver?where=nexearch&query=" + item.Name);
        }
        #endregion

        public MainViewModel()
        {
            if (Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "/Simul")
                consumer = new HistoryConsumer();
            else
                consumer = new RealTimeConsumer();

            Dashboard = new Dashboard_(consumer);

            //TestData();
        }

        private void TestData()
        {
            dashboard.StockItems.Add("000020", new DashboardItem() { Code = "000020", Name = "삼양1", Price = 2000, Volume = 60, BasisPrice = 2000, PreviousVolume = 50 });
            dashboard.StockItems.Add("000030", new DashboardItem() { Code = "000030", Name = "삼양2", Price = (float)102.20, Volume = 243, BasisPrice = (float)101.20, PreviousVolume = 176750 });
            dashboard.StockItems.Add("000040", new DashboardItem() { Code = "000040", Name = "삼양3", Price = 55, Volume = 5435, BasisPrice = 50, PreviousVolume = 7576, CircuitBreakType = DataStructure.CircuitBreakTypes.StaticInvoke });
            dashboard.StockItems.Add("000050", new DashboardItem() { Code = "000050", Name = "삼양4", Price = 243400, Volume = 423, BasisPrice = 244400, PreviousVolume = 10656 });
            dashboard.StockItems.Add("000060", new DashboardItem() { Code = "000060", Name = "삼양5", Price = 12200, Volume = 510, BasisPrice = 11200, PreviousVolume = 234, CircuitBreakType = DataStructure.CircuitBreakTypes.DynamicInvoke });

            dashboard.IndexItems.Add("000040", new DashboardItem() { Code = "000040", Name = "삼양3", Price = 50, Volume = 5435, BasisPrice = 46, PreviousVolume = 7576 });
            dashboard.IndexItems.Add("000050", new DashboardItem() { Code = "000050", Name = "삼양4", Price = 243400, Volume = 423, BasisPrice = 242400, PreviousVolume = 10656 });
            dashboard.IndexItems.Add("000060", new DashboardItem() { Code = "000060", Name = "삼양5", Price = 12200, Volume = 510, BasisPrice = 12300, PreviousVolume = 234 });

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                dashboard.StockItems["000020"].Price = 2100;
                dashboard.StockItems["000020"].Price = 2110;
                dashboard.StockItems["000020"].Price = 2111;
                dashboard.StockItems["000020"].Price = 2112;
                dashboard.StockItems["000020"].Price = 2113;

                Thread.Sleep(1000);
                dashboard.StockItems["000030"].Price = (float)102.21;

                Thread.Sleep(1000);
                dashboard.StockItems["000060"].Price = 12201;

                Thread.Sleep(1000);
                dashboard.StockItems["000020"].Price = 2101;

                Thread.Sleep(1000);
                dashboard.StockItems["000030"].Price = (float)102.22;

                Thread.Sleep(1000);
                dashboard.StockItems["000060"].Price = 12202;

                Thread.Sleep(1000);
                dashboard.StockItems["000020"].Price = 2103;

                Thread.Sleep(1000);
                dashboard.StockItems["000030"].Price = (float)102.23;

                Thread.Sleep(1000);
                dashboard.StockItems["000060"].Price = 12203;
            });
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
