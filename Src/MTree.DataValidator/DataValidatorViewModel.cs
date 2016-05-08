using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataValidator
{
    public class DataValidatorViewModel : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private DataValidator validator = new DataValidator();

        private DateTime _TargetDate = DateTime.Now;
        public DateTime TargetDate
        {
            get
            {
                return _TargetDate;
            }
            set
            {
                _TargetDate = value;
                NotifyPropertyChanged(nameof(TargetDate));
            }
        }

        private string _Code;
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private string _SourceAddress;
        public string SourceAddress
        {
            get
            {
                return _SourceAddress;
            }
            set
            {
                _SourceAddress = value;
                NotifyPropertyChanged(nameof(SourceAddress));
            }
        }

        private string _DestinationAddress;
        public string DestinationAddress
        {
            get
            {
                return _DestinationAddress;
            }
            set
            {
                _DestinationAddress = value;
                NotifyPropertyChanged(nameof(_DestinationAddress));
            }
        }
        
        private RelayCommand _ValidateAllCommand;
        public ICommand ValidateAllCommand
        {
            get
            {
                if (_ValidateAllCommand == null)
                    _ValidateAllCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        ValidateAllExecute();
                    }));

                return _ValidateAllCommand;
            }
        }
        private void ValidateAllExecute()
        {
            if (Config.Validator.UseSimultaneousCompare == true)
            {
                var task = Task.Run(() =>
                {
                    List<Task> tasks = new List<Task>();
                    tasks.Add(Task.Run(() => validator.ValidateCodeList()));
                    tasks.Add(Task.Run(() => validator.ValidateStockConclusionCompare(TargetDate)));
                    tasks.Add(Task.Run(() => validator.ValidateIndexConclusionCompare(TargetDate)));
                    tasks.Add(Task.Run(() => validator.ValidateMasterCompare(TargetDate)));
                    tasks.Add(Task.Run(() => validator.ValidateCircuitBreakCompare(TargetDate)));
                    Task.WaitAll(tasks.ToArray());
                });
                task.Wait();
            }
            else
            {
                var task = Task.Run(() =>
                {
                    validator.ValidateCodeList();
                    validator.ValidateStockConclusionCompare(TargetDate);
                    validator.ValidateIndexConclusionCompare(TargetDate);
                    validator.ValidateMasterCompare(TargetDate);
                    validator.ValidateCircuitBreakCompare(TargetDate);
                });
                task.Wait();
            }
        }

        private RelayCommand _ValidateMasterCommand;
        public ICommand ValidateMasterCommand
        {
            get
            {
                if (_ValidateMasterCommand == null)
                    _ValidateMasterCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        validator.ValidateMasterCompare(TargetDate);
                    }));

                return _ValidateMasterCommand;
            }
        }

        private RelayCommand _ValidateAllStockConclusionCommand;
        public ICommand ValidateAllStockConclusionCommand
        {
            get
            {
                if (_ValidateAllStockConclusionCommand == null)
                    _ValidateAllStockConclusionCommand = new RelayCommand(() => 
                    Task.Run(() => 
                    {
                        validator.ValidateStockConclusionCompare(TargetDate);
                    }));

                return _ValidateAllStockConclusionCommand;
            }
        }

        private RelayCommand _ValidateIndivisualStockConclusionCommand;
        public ICommand ValidateIndivisualStockConclusionCommand
        {
            get
            {
                if (_ValidateIndivisualStockConclusionCommand == null)
                    _ValidateIndivisualStockConclusionCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        validator.ValidateStockConclusionCompare(TargetDate, Code);
                    }));

                return _ValidateIndivisualStockConclusionCommand;
            }
        }

        private RelayCommand _ValidateIndivisualStockConclusionWithDaishinCommand;
        public ICommand ValidateIndivisualStockConclusionWithDaishinCommand
        {
            get
            {
                if (_ValidateIndivisualStockConclusionWithDaishinCommand == null)
                    _ValidateIndivisualStockConclusionWithDaishinCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        validator.ValidateStockConclusionCompareWithDaishin(TargetDate, Code);
                    }));

                return _ValidateIndivisualStockConclusionWithDaishinCommand;
            }
        }

        private RelayCommand _ValidateAllIndexConclusionCommand;
        public ICommand ValidateAllIndexConclusionCommand
        {
            get
            {
                if (_ValidateAllIndexConclusionCommand == null)
                    _ValidateAllIndexConclusionCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        validator.ValidateIndexConclusionCompare(TargetDate);
                    }));

                return _ValidateAllIndexConclusionCommand;
            }
        }
        
        private RelayCommand _ValidateIndivisualIndexConclusionCommand;
        public ICommand ValidateIndivisualIndexConclusionCommand
        {
            get
            {
                if (_ValidateIndivisualIndexConclusionCommand == null)
                    _ValidateIndivisualIndexConclusionCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        validator.ValidateIndexConclusionCompare(TargetDate, Code);
                    }));

                return _ValidateIndivisualIndexConclusionCommand;
            }
        }

        private RelayCommand _ValidateCircuitBreakCommand;
        public ICommand ValidateCircuitBreakCommand
        {
            get
            {
                if (_ValidateCircuitBreakCommand == null)
                    _ValidateCircuitBreakCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        validator.ValidateCircuitBreakCompare(TargetDate);
                    }));

                return _ValidateCircuitBreakCommand;
            }
        }

        public DataValidatorViewModel()
        {
            SourceAddress = Config.Database.ConnectionString;
            DestinationAddress = Config.Database.RemoteConnectionString;

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                logger.Info("Data Validator Started");
                
                if (Environment.GetCommandLineArgs().Count() > 1)
                {
                    if (Environment.GetCommandLineArgs()[1] == ProcessTypes.DataValidatorRegularCheck.ToString())
                    {
                        ValidateAllExecute();
                        Environment.Exit(0);
                    }
                }
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
