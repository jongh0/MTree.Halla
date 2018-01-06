using GalaSoft.MvvmLight.Command;
using Configuration;
using DbProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace DataValidator
{
    public interface IWindowFactory
    {
        DialogResult CreateNewWindow(string uri);
    }

    public class RecoveryPopupWindowFactory : IWindowFactory
    {
        private object _DataContext;
        private RecoveryPopup PopupWindow = new RecoveryPopup();

        public RecoveryPopupWindowFactory(object dataContext)
        {
            _DataContext = dataContext;
        }

        public DialogResult CreateNewWindow(string uri)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PopupWindow = new RecoveryPopup
                {
                    DataContext = _DataContext
                };

                PopupWindow.SetUri(uri);
                PopupWindow.ShowDialog();
            }));

            return PopupWindow.Result;
        }
    }

    public partial class MainViewModel
    {        
        private const string logBasePath = "Logs";
        private const string compareResultPath = "CompareResult";
        private const string codeCompareResultFile = "CodeCompare.html";
        private const string masterCompareResultPath = "Master";
        private const string stockConclusionCompareResultPath = "StockConclusion";
        private const string indexConclusionCompareResultPath = "IndexConclusion";
        private const string circuitbreakCompareResultPath = "CircuitBreak";

        private object popupWindowLockObject = new object();

        private IWindowFactory RecoveryPopupFactory { get; }

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

        private string _Code;
        public string Code
        {
            get { return _Code; }
            set
            {
                _Code = value.Trim();
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private bool _ValidateOnly = true;
        public bool ValidateOnly
        {
            get { return _ValidateOnly; }
            set
            {
                _ValidateOnly = value;
                NotifyPropertyChanged(nameof(ValidateOnly));
            }
        }

        private bool _ApplyForAll = false;
        public bool ApplyForAll
        {
            get { return _ApplyForAll; }
            set
            {
                _ApplyForAll = value;
                NotifyPropertyChanged(nameof(ApplyForAll));
            }
        }

        private bool _FromSourceToDestination = true;
        public bool FromSourceToDestination
        {
            get { return _FromSourceToDestination; }
            set
            {
                _FromSourceToDestination = value;
                
                if (_FromSourceToDestination == true)
                {
                    _recoverer.From = DbAgent.Instance;
                    _recoverer.To = DbAgent.RemoteInstance;
                }
                else
                {
                    _recoverer.From = DbAgent.RemoteInstance;
                    _recoverer.To = DbAgent.Instance;
                }
                //NotifyPropertyChanged(nameof(FromSourceToDestination));
            }
        }

        private RelayCommand _recoverAllCommand;
        public ICommand RecoverAllCommand
        {
            get
            {
                if (_recoverAllCommand == null)
                {
                    _recoverAllCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        DoRecoverAll();
                    }));
                }

                return _recoverAllCommand;
            }
        }

        private void DoRecoverAll()
        {
            DoRecoverMasterAll();
            DoRecoverStockConclusionAll();
            DoRecoverIndexConclusionAll();
            DoRecoverCircuitBreakAll();
        }

        private RelayCommand _recoverMasterAllCommand;
        public ICommand RecoverMasterAllCommand => _recoverMasterAllCommand ?? (_recoverMasterAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverMasterAll())));


        private void DoRecoverMasterAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                int cnt = 0;
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (_validator.ValidateMaster(targetDate, code, true) == false)
                    {
                        _logger.Error($"Master Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                        if (ValidateOnly == false)
                        {
                            DialogResult needRecovery = DialogResult.None;

                            lock (popupWindowLockObject)
                            {
                                if (ApplyForAll == false)
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultPath, code + ".html"));
                                else
                                    needRecovery = DialogResult.OK;
                            }

                            if (needRecovery == DialogResult.OK)
                                _recoverer.RecoverMaster(targetDate, code);
                        }
                    }
                    else
                    {
                        _logger.Info($"Master Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                    }
                });
                ApplyForAll = false;
            }

            _logger.Info("Master Recovery Done.");
        }

        private RelayCommand _recoverMasterCommand;
        public ICommand RecoverMasterCommand
        {
            get
            {
                if (_recoverMasterCommand == null)
                    _recoverMasterCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (_validator.ValidateMaster(targetDate, Code, true) == false)
                            {
                                _logger.Error($"Master Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;
                                    lock (popupWindowLockObject)
                                    {
                                        if (ApplyForAll == false)
                                            needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultPath, Code + ".html"));
                                        else
                                            needRecovery = DialogResult.OK;
                                    }
                                    if (needRecovery == DialogResult.OK)
                                    {
                                        _recoverer.RecoverMaster(targetDate, Code);
                                    }
                                }
                            }
                            else
                            {
                                _logger.Info($"Master Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} success.");
                            }
                        }
                    }));

                return _recoverMasterCommand;
            }
        }

        private RelayCommand _recoverStockConclusionAllCommand;
        public ICommand RecoverStockConclusionAllCommand => _recoverStockConclusionAllCommand ?? (_recoverStockConclusionAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverStockConclusionAll())));


        private void DoRecoverStockConclusionAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                int cnt = 0;
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (_validator.ValidateStockConclusion(targetDate, code, true) == false)
                    {
                        _logger.Error($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail. {Interlocked.Increment(ref cnt)}/{codeList.Count}");

                        if (ValidateOnly == false)
                        {
                            DialogResult needRecovery = DialogResult.None;
                            lock (popupWindowLockObject)
                            {
                                if (ApplyForAll == false)
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                                else
                                    needRecovery = DialogResult.OK;
                            }

                            if (needRecovery == DialogResult.OK)
                                _recoverer.RecoverStockConclusion(targetDate, code);
                        }
                    }
                    else
                    {
                        _logger.Info($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                    }
                });

                ApplyForAll = false;
            }

            _logger.Info("Stock Conclusion Recovery Done.");
        }

        private RelayCommand _recoverStockConclusionCommand;
        public ICommand RecoverStockConclusionCommand
        {
            get
            {
                if (_recoverStockConclusionCommand == null)
                {
                    _recoverStockConclusionCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (_validator.ValidateStockConclusion(targetDate, Code, true) == false)
                            {
                                _logger.Error($"Stock Conclusion  Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;

                                    lock (popupWindowLockObject)
                                    {
                                        needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, Code + ".html"));
                                    }

                                    if (needRecovery == DialogResult.OK)
                                        _recoverer.RecoverStockConclusion(targetDate, Code);
                                }
                            }
                            else
                            {
                                _logger.Info($"Stock Conclusion Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} success.");
                            }
                        }

                        ApplyForAll = false;
                        _logger.Info("Stock Conclusion Recovery Done.");
                    }));
                }

                return _recoverStockConclusionCommand;
            }
        }

        private RelayCommand _recoverIndexConclusionAllCommand;
        public ICommand RecoverIndexConclusionAllCommand => _recoverIndexConclusionAllCommand ?? (_recoverIndexConclusionAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverIndexConclusionAll())));

        private void DoRecoverIndexConclusionAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (_validator.ValidateIndexConclusion(targetDate, code, true) == false)
                    {
                        if (ValidateOnly == false)
                        {
                            DialogResult needRecovery = DialogResult.None;

                            lock (popupWindowLockObject)
                            {
                                if (File.Exists(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html")) == false)
                                    _logger.Error($"{Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html")} is not exist.");

                                if (ApplyForAll == false)
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html"));
                                else
                                    needRecovery = DialogResult.OK;
                            }

                            if (needRecovery == DialogResult.OK)
                                _recoverer.RecoverIndexConclusion(targetDate, code);
                        }
                    }
                });

                ApplyForAll = false;
            }

            _logger.Info("Index Conclusion Recovery Done.");
        }

        private RelayCommand _recoverIndexConclusionCommand;
        public ICommand RecoverIndexConclusionCommand
        {
            get
            {
                if (_recoverIndexConclusionCommand == null)
                {
                    _recoverIndexConclusionCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (_validator.ValidateIndexConclusion(targetDate, Code, true) == false)
                            {
                                _logger.Error($"Index Conclusion Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;

                                    lock (popupWindowLockObject)
                                    {
                                        needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, Code + ".html"));
                                    }

                                    if (needRecovery == DialogResult.OK)
                                        _recoverer.RecoverIndexConclusion(targetDate, Code);
                                }
                            }
                            else
                            {
                                _logger.Info($"Index Conclusion Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} success.");
                            }
                        }
                        _logger.Info("Index Conclusion Recovery Done.");
                    }));
                }

                return _recoverIndexConclusionCommand;
            }
        }
        
        private RelayCommand _recoverCircuitBreakAllCommand;
        public ICommand RecoverCircuitBreakAllCommand => _recoverCircuitBreakAllCommand ?? (_recoverCircuitBreakAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverCircuitBreakAll())));

        private void DoRecoverCircuitBreakAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                int cnt = 0;
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (_validator.ValidateCircuitBreak(targetDate, code, true) == false)
                    {
                        _logger.Error($"Circuit Break Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                        if (ValidateOnly == false)
                        {
                            DialogResult needRecovery = DialogResult.None;

                            lock (popupWindowLockObject)
                            {
                                if (ApplyForAll == false)
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, circuitbreakCompareResultPath, code + ".html"));
                                else
                                    needRecovery = DialogResult.OK;
                            }

                            if (needRecovery == DialogResult.OK)
                                _recoverer.RecoverCircuitBreak(targetDate, code);
                        }
                    }
                    else
                    {
                        _logger.Info($"Circuit Break Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success.{Interlocked.Increment(ref cnt)}/{codeList.Count}");
                    }
                });
                ApplyForAll = false;
            }

            _logger.Info("Circuit Break Recovery Done.");
        }


        private RelayCommand _recoverCircuitBreakCommand;
        public ICommand RecoverCircuitBreakCommand
        {
            get
            {
                if (_recoverCircuitBreakCommand == null)
                {
                    _recoverCircuitBreakCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (_validator.ValidateCircuitBreak(targetDate, Code, true) == false)
                            {
                                _logger.Error($"Circuit Break Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;

                                    lock (popupWindowLockObject)
                                    {
                                        needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, circuitbreakCompareResultPath, Code + ".html"));
                                    }

                                    if (needRecovery == DialogResult.OK)
                                        _recoverer.RecoverCircuitBreak(targetDate, Code);
                                }
                            }
                            else
                            {
                                _logger.Info($"Circuit Break Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} success.");
                            }
                        }
                    }));
                }

                return _recoverCircuitBreakCommand;
            }
        }
    }
}
