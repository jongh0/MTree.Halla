using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataValidator
{
    public class ValidatorViewModel : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataValidator Validator { get; set; } = new DataValidator();

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

        private string _CodeForDaishinValidate;
        public string CodeForDaishinValidate
        {
            get
            {
                return _CodeForDaishinValidate;
            }
            set
            {
                _CodeForDaishinValidate = value;
                NotifyPropertyChanged(nameof(CodeForDaishinValidate));
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

        public void ValidateAllExecute()
        {
            if (Config.Validator.UseSimultaneousCompare == true)
            {
                var task = Task.Run(() =>
                {
                    if (Validator != null)
                    {
                        List<Task> tasks = new List<Task>();

                        tasks.Add(Task.Run(() => Validator.ValidateCodeList()));
                        tasks.Add(Task.Run(() => Validator.ValidateStockConclusions(TargetDate)));
                        tasks.Add(Task.Run(() => Validator.ValidateIndexConclusion(TargetDate)));
                        tasks.Add(Task.Run(() => Validator.ValidateMasters(TargetDate)));
                        tasks.Add(Task.Run(() => Validator.ValidateCircuitBreak(TargetDate)));
                        Task.WaitAll(tasks.ToArray());
                    }
                    else
                    {
                        logger.Error("Validator is not assigned");
                    }
                });
                task.Wait();
            }
            else
            {
                var task = Task.Run(() =>
                {
                    if (Validator != null)
                    {
                        Validator.ValidateCodeList();
                        Validator.ValidateStockConclusions(TargetDate);
                        Validator.ValidateIndexConclusion(TargetDate);
                        Validator.ValidateMasters(TargetDate);
                        Validator.ValidateCircuitBreak(TargetDate);
                    }
                    else
                    {
                        logger.Error("Validator is not assigned");
                    }
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
                        if (Validator != null)
                        {
                            Validator.ValidateMasters(TargetDate);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
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
                        if (Validator != null)
                        {
                            Validator.ValidateStockConclusions(TargetDate);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
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
                        if (Validator != null)
                        {
                            Validator.ValidateStockConclusion(TargetDate, Code);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _ValidateIndivisualStockConclusionCommand;
            }
        }

        private RelayCommand _ValidateSourceConclusionWithDaishinCommand;
        public ICommand ValidateSourceConclusionWithDaishinCommand
        {
            get
            {
                if (_ValidateSourceConclusionWithDaishinCommand == null)
                    _ValidateSourceConclusionWithDaishinCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        if (Validator != null)
                        {
                            Validator.ValidateSourceConclusionWithDaishin(CodeForDaishinValidate);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _ValidateSourceConclusionWithDaishinCommand;
            }
        }

        private RelayCommand _ValidateDestinationConclusionWithDaishinCommand;
        public ICommand ValidateDestinationConclusionWithDaishinCommand
        {
            get
            {
                if (_ValidateDestinationConclusionWithDaishinCommand == null)
                    _ValidateDestinationConclusionWithDaishinCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        if (Validator != null)
                        {
                            Validator.ValidateDestinationConclusionWithDaishin(CodeForDaishinValidate);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _ValidateDestinationConclusionWithDaishinCommand;
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
                        if (Validator != null)
                        {
                            Validator.ValidateIndexConclusion(TargetDate);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
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
                        if (Validator != null)
                        {
                            Validator.ValidateIndexConclusion(TargetDate, Code);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
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
                        if (Validator != null)
                        {
                            Validator.ValidateCircuitBreak(TargetDate);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _ValidateCircuitBreakCommand;
            }
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
