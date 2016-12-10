using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.Consumer;
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
        private readonly string defaultDir = Path.Combine(Environment.CurrentDirectory, "Extract");
        private string fileName;
        private string filePath;

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

        private bool _IsExtracting = false;
        public bool IsExtracting
        {
            get { return _IsExtracting; }
            set
            {
                _IsExtracting = value;
                NotifyPropertyChanged(nameof(IsExtracting));
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }

        private bool _IncludeTAValues = true;
        public bool IncludeTAValues
        {
            get { return _IncludeTAValues; }
            set
            {
                _IncludeTAValues = value;
                NotifyPropertyChanged(nameof(IncludeTAValues));
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
                        IsExtracting = true;

                        string[] codes = { Code };

                        fileName = $"{Code}_{StartingDate.ToString(Config.General.DateFormat)}~{EndingDate.ToString(Config.General.DateFormat)}.csv";
                        filePath = Path.Combine(defaultDir, fileName);

                        if (Directory.Exists(defaultDir) == false)
                            Directory.CreateDirectory(defaultDir);

                        extractor.IncludeTAValues = IncludeTAValues;

                        extractor.StartExtract(filePath);

                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (consumer.StartSimulation(codes, targetDate) == true)
                                extractor.WaitSubscribingDone();
                        }

                        IsExtracting = false;
                    }));

                return _StartExtractCommand;
            }
        }

        private bool _CanExecuteExtract = true;
        public bool CanExecuteExtract
        {
            get
            {
                return _CanExecuteExtract && Code?.Length >= 6 && IsExtracting == false;
            }
            set
            {
                _CanExecuteExtract = value;
                NotifyPropertyChanged(nameof(CanExecuteExtract));
            }
        }
        #endregion

        private ISimulation consumer { get; set; }
        private DataExtractor extractor { get; set; }
        public MainViewModel()
        {
            consumer = new HistoryConsumer();
            extractor = new DataExtractor((ConsumerBase)consumer);
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
