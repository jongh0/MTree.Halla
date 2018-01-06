using GalaSoft.MvvmLight.Command;
using Configuration;
using Consumer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DataExtractor
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly string _defaultDir = Path.Combine(Environment.CurrentDirectory, "Extract");
        private string _fileName;
        private string _filePath;

        private string _code;
        public string Code
        {
            get => _code;
            set
            {
                _code = value.Trim();
                NotifyPropertyChanged(nameof(Code));
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }

        private DateTime _startingDate = DateTime.Now;
        public DateTime StartingDate
        {
            get => _startingDate;
            set
            {
                _startingDate = value;

                if (EndingDate < _startingDate)
                    EndingDate = _startingDate;

                NotifyPropertyChanged(nameof(StartingDate));
            }
        }

        private DateTime _endingDate = DateTime.Now;
        public DateTime EndingDate
        {
            get => _endingDate;
            set
            {
                _endingDate = value;

                if (_endingDate < StartingDate)
                    StartingDate = _endingDate;

                NotifyPropertyChanged(nameof(EndingDate));
            }
        }

        private bool _isExtracting = false;
        public bool IsExtracting
        {
            get => _isExtracting;
            set
            {
                _isExtracting = value;
                NotifyPropertyChanged(nameof(IsExtracting));
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }

        private bool _includeTAValues = true;
        public bool IncludeTAValues
        {
            get => _includeTAValues;
            set
            {
                _includeTAValues = value;
                NotifyPropertyChanged(nameof(IncludeTAValues));
            }
        }

        #region Command
        private RelayCommand _startExtractCommand;
        public ICommand StartExtractCommand
        {
            get
            {
                if (_startExtractCommand == null)
                {
                    _startExtractCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        IsExtracting = true;

                        string[] codes = { Code };

                        _fileName = $"{Code}_{StartingDate.ToString(Config.General.DateFormat)}~{EndingDate.ToString(Config.General.DateFormat)}.csv";
                        _filePath = Path.Combine(_defaultDir, _fileName);

                        if (Directory.Exists(_defaultDir) == false)
                            Directory.CreateDirectory(_defaultDir);

                        Extractor.IncludeTAValues = IncludeTAValues;

                        Extractor.StartExtract(_filePath);

                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (Consumer.StartSimulation(codes, targetDate) == true)
                                Extractor.WaitSubscribingDone();
                        }

                        IsExtracting = false;
                    }));
                }

                return _startExtractCommand;
            }
        }

        private bool _canExecuteExtract = true;
        public bool CanExecuteExtract
        {
            get => _canExecuteExtract && Code?.Length >= 6 && IsExtracting == false;
            set
            {
                _canExecuteExtract = value;
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }
        #endregion

        private ISimulation Consumer { get; set; }
        private DataExtractor_ Extractor { get; set; }

        public MainViewModel()
        {
            Consumer = new HistoryConsumer();
            Extractor = new DataExtractor_((ConsumerBase)Consumer);
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
