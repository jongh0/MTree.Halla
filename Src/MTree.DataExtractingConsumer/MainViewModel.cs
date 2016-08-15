using GalaSoft.MvvmLight.Command;
using MTree.Consumer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataExtractingConsumer
{
    public class MainViewModel : INotifyPropertyChanged
    {

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

        #region Command
        private RelayCommand _StartExtractCommand;
        public ICommand StartExtractCommand
        {
            get
            {
                if (_StartExtractCommand == null)
                    _StartExtractCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        string[] codes = { Code };
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            extractor.StartSimulation(codes, targetDate);
                        }
                    }));

                return _StartExtractCommand;
            }
        }
        #endregion

        private ISimulation extractor { get; set; }
        public MainViewModel()
        {
            extractor = new HistoryConsumer();
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
