using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataExtractor
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private DataLoader dataLoader = new DataLoader();
        private DataExtractor dataExtractor = new DataExtractor();

        private static readonly string defaultTitle = "DataExtractor";

        private readonly string defaultDir = Path.Combine(Environment.CurrentDirectory, "Extract");
        private string fileName;
        private string filePath;

        #region Property
        private string _TitleStr = defaultTitle;
        public string TitleStr
        {
            get { return _TitleStr; }
            set
            {
                _TitleStr = value;
                NotifyPropertyChanged(nameof(TitleStr));
            }
        }

        private ExtractTypes _ExtractType = ExtractTypes.Stock;
        public ExtractTypes ExtractType
        {
            get { return _ExtractType; }
            set
            {
                _ExtractType = value;
                NotifyPropertyChanged(nameof(ExtractType));
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }

        private string _Code = string.Empty;
        public string Code
        {
            get { return _Code; }
            set
            {
                _Code = value.Trim();
                NotifyPropertyChanged(nameof(Code));
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }

        private DateTime _StartDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return _StartDate; }
            set
            {
                _StartDate = value;

                if (EndDate < _StartDate)
                    EndDate = _StartDate;

                NotifyPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime _EndDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return _EndDate; }
            set
            {
                _EndDate = value;

                if (_EndDate < StartDate)
                    StartDate = _EndDate;

                NotifyPropertyChanged(nameof(EndDate));
            }
        }
        #endregion

        #region Command
        RelayCommand _ExtractCommand;
        public ICommand ExtractCommand
        {
            get
            {
                if (_ExtractCommand == null)
                    _ExtractCommand = new RelayCommand(() => Task.Run(() => ExecuteExtract()));

                return _ExtractCommand;
            }
        }

        private bool _CanExecuteExtract = true;
        public bool CanExecuteExtract
        {
            get
            {
                if (ExtractType == ExtractTypes.Stock)
                    return _CanExecuteExtract && Code?.Length >= 6;
                else
                    return _CanExecuteExtract && Code?.Length >= 3;
            }
            set
            {
                _CanExecuteExtract = value;
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }

        public void ExecuteExtract()
        {
            CanExecuteExtract = false;

            try
            {
                fileName = $"{ExtractType.ToString()}_{Code}_{StartDate.ToString(Config.General.DateFormat)}~{EndDate.ToString(Config.General.DateFormat)}.csv";
                filePath = Path.Combine(defaultDir, fileName);

                if (Directory.Exists(defaultDir) == false)
                    Directory.CreateDirectory(defaultDir);

                if (ExtractType == ExtractTypes.Stock)
                {
                    TitleStr = $"{defaultTitle} - Loading";
                    var conclusionList = dataLoader.Load<StockConclusion>(Code, StartDate, EndDate);
                    var masterList = dataLoader.Load<StockMaster>(Code, StartDate, EndDate);

                    TitleStr = $"{defaultTitle} - Extracting";
                    dataExtractor.Extract(conclusionList, masterList, filePath);
                }
                else
                {
                    TitleStr = $"{defaultTitle} - Loading";
                    var conclusionList = dataLoader.Load<IndexConclusion>(Code, StartDate, EndDate);
                    var masterList = dataLoader.Load<IndexMaster>(Code, StartDate, EndDate);

                    TitleStr = $"{defaultTitle} - Extracting";
                    dataExtractor.Extract(conclusionList, masterList, filePath);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                TitleStr = defaultTitle;
                CanExecuteExtract = true;
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
