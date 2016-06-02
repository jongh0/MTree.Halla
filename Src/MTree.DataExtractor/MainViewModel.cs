using GalaSoft.MvvmLight.Command;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataExtractor
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private DataLoader Loader = new DataLoader();
        private DataExtractor Extractor = new DataExtractor();

        #region Property
        private ExtractTypes _ExtractType = ExtractTypes.Stock;
        public ExtractTypes ExtractType
        {
            get { return _ExtractType; }
            set
            {
                _ExtractType = value;
                NotifyPropertyChanged(nameof(ExtractType));
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
                    _ExtractCommand = new RelayCommand(() => Task.Run(() => ExecuteExtract()), () => CanExecuteExtract);

                return _ExtractCommand;
            }
        }

        private bool _CanExecuteExtract = true;
        public bool CanExecuteExtract
        {
            get { return _CanExecuteExtract & Code.Length == 6; }
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
                if (ExtractType == ExtractTypes.Stock)
                {
                    var conclusionList = Loader.LoadRange<StockConclusion>(Code, StartDate, EndDate);
                    var masterList = Loader.LoadRange<StockMaster>(Code, StartDate, EndDate);
                }
                else
                {
                    var conclusionList = Loader.LoadRange<IndexConclusion>(Code, StartDate, EndDate);
                    var masterList = Loader.LoadRange<IndexMaster>(Code, StartDate, EndDate);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
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
