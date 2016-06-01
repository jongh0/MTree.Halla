using GalaSoft.MvvmLight.Command;
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
        #endregion

        #region Command
        RelayCommand _ExtractCommand;
        public ICommand ExtractCommand
        {
            get
            {
                if (_ExtractCommand == null)
                    _ExtractCommand = new RelayCommand(() => ExecuteExtract());

                return _ExtractCommand;
            }
        }

        public void ExecuteExtract()
        {
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
