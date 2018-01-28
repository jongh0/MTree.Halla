using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataStructure
{
    [BsonDiscriminator(RootClass = true)]
    public class DataCounter : INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public DataTypes Type { get; set; }

        private int _chartCount = 0;
        public int ChartCount
        {
            get { return _chartCount; }
            set
            {
                if (_chartCount != value)
                {
                    _chartCount = value;
                    NotifyPropertyChanged(nameof(ChartCount));
                }
            }
        }

        private int _biddingPriceCount = 0;
        public int BiddingPriceCount
        {
            get { return _biddingPriceCount; }
            set
            {
                if (_biddingPriceCount != value)
                {
                    _biddingPriceCount = value;
                    NotifyPropertyChanged(nameof(BiddingPriceCount));
                }
            }
        }

        private int _circuitBreakCount = 0;
        public int CircuitBreakCount
        {
            get { return _circuitBreakCount; }
            set
            {
                if (_circuitBreakCount != value)
                {
                    _circuitBreakCount = value;
                    NotifyPropertyChanged(nameof(CircuitBreakCount));
                }
            }
        }

        private int _stockMasterCount = 0;
        public int StockMasterCount
        {
            get { return _stockMasterCount; }
            set
            {
                if (_stockMasterCount != value)
                {
                    _stockMasterCount = value;
                    NotifyPropertyChanged(nameof(StockMasterCount));
                }
            }
        }

        private int _indexMasterCount = 0;
        public int IndexMasterCount
        {
            get { return _indexMasterCount; }
            set
            {
                if (_indexMasterCount != value)
                {
                    _indexMasterCount = value;
                    NotifyPropertyChanged(nameof(IndexMasterCount));
                }
            }
        }

        private int _stockConclusionCount = 0;
        public int StockConclusionCount
        {
            get { return _stockConclusionCount; }
            set
            {
                if (_stockConclusionCount != value)
                {
                    _stockConclusionCount = value;
                    NotifyPropertyChanged(nameof(StockConclusionCount));
                }
            }
        }

        private int _indexConclusionCount = 0;
        public int IndexConclusionCount
        {
            get { return _indexConclusionCount; }
            set
            {
                if (_indexConclusionCount != value)
                {
                    _indexConclusionCount = value;
                    NotifyPropertyChanged(nameof(IndexConclusionCount));
                }
            }
        }

        private int _etfConclusionCount = 0;
        public int ETFConclusionCount
        {
            get { return _etfConclusionCount; }
            set
            {
                if (_etfConclusionCount != value)
                {
                    _etfConclusionCount = value;
                    NotifyPropertyChanged(nameof(ETFConclusionCount));
                }
            }
        }

        private int _tradeConclusionCount = 0;
        public int TradeConclusionCount
        {
            get { return _tradeConclusionCount; }
            set
            {
                if (_tradeConclusionCount != value)
                {
                    _tradeConclusionCount = value;
                    NotifyPropertyChanged(nameof(TradeConclusionCount));
                }
            }
        }

        public int TotalCount
        {
            get { return ChartCount + StockMasterCount + IndexMasterCount + BiddingPriceCount + CircuitBreakCount + StockConclusionCount + IndexConclusionCount + ETFConclusionCount + TradeConclusionCount; }
        }

        public DataCounter(DataTypes type)
        {
            Type = type;
            Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        public void NotifyPropertyAll()
        {
            NotifyPropertyChanged(nameof(ChartCount));
            NotifyPropertyChanged(nameof(BiddingPriceCount));
            NotifyPropertyChanged(nameof(CircuitBreakCount));
            NotifyPropertyChanged(nameof(StockMasterCount));
            NotifyPropertyChanged(nameof(IndexMasterCount));
            NotifyPropertyChanged(nameof(StockConclusionCount));
            NotifyPropertyChanged(nameof(IndexConclusionCount));
            NotifyPropertyChanged(nameof(ETFConclusionCount));
            NotifyPropertyChanged(nameof(TradeConclusionCount));
            NotifyPropertyChanged(nameof(TotalCount));
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public void Increment(CounterTypes type)
        {
            switch (type)
            {
                case CounterTypes.Chart:
                    Interlocked.Increment(ref _chartCount);
                    break;

                case CounterTypes.BiddingPrice:
                    Interlocked.Increment(ref _biddingPriceCount);
                    break;

                case CounterTypes.CircuitBreak:
                    Interlocked.Increment(ref _circuitBreakCount);
                    break;

                case CounterTypes.StockMaster:
                    Interlocked.Increment(ref _stockMasterCount);
                    break;

                case CounterTypes.IndexMaster:
                    Interlocked.Increment(ref _indexMasterCount);
                    break;

                case CounterTypes.StockConclusion:
                    Interlocked.Increment(ref _stockConclusionCount);
                    break;

                case CounterTypes.IndexConclusion:
                    Interlocked.Increment(ref _indexConclusionCount);
                    break;

                case CounterTypes.ETFConclusion:
                    Interlocked.Increment(ref _etfConclusionCount);
                    break;

                case CounterTypes.TradeConclusion:
                    Interlocked.Increment(ref _tradeConclusionCount);
                    break;
            }
        }

        public void Add(CounterTypes type, int value)
        {
            switch (type)
            {
                case CounterTypes.Chart:
                    Interlocked.Add(ref _chartCount, value);
                    break;

                case CounterTypes.BiddingPrice:
                    Interlocked.Add(ref _biddingPriceCount, value);
                    break;

                case CounterTypes.CircuitBreak:
                    Interlocked.Add(ref _circuitBreakCount, value);
                    break;

                case CounterTypes.StockMaster:
                    Interlocked.Add(ref _stockMasterCount, value);
                    break;

                case CounterTypes.IndexMaster:
                    Interlocked.Add(ref _indexMasterCount, value);
                    break;

                case CounterTypes.StockConclusion:
                    Interlocked.Add(ref _stockConclusionCount, value);
                    break;

                case CounterTypes.IndexConclusion:
                    Interlocked.Add(ref _indexConclusionCount, value);
                    break;

                case CounterTypes.ETFConclusion:
                    Interlocked.Add(ref _etfConclusionCount, value);
                    break;

                case CounterTypes.TradeConclusion:
                    Interlocked.Add(ref _tradeConclusionCount, value);
                    break;
            }
        }

        public void SaveToFile()
        {
            try
            {
                var fileName = $"MTree.{Config.General.DateNow}_{Type.ToString()}.csv";
                var filePath = Path.Combine(Environment.CurrentDirectory, "Logs", Config.General.DateNow, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.WriteLine($"Chart, {ChartCount}");
                    sw.WriteLine($"CircuitBreak, {CircuitBreakCount}");
                    sw.WriteLine($"BiddingPrice, {BiddingPriceCount}");
                    sw.WriteLine($"StockMaster, {StockMasterCount}");
                    sw.WriteLine($"IndexMaster, {IndexMasterCount}");
                    sw.WriteLine($"StockConclusion, {StockConclusionCount}");
                    sw.WriteLine($"IndexConclusion, {IndexConclusionCount}");
                    sw.WriteLine($"ETFConclusion, {ETFConclusionCount}");
                    sw.WriteLine($"TradeConclusion, {TradeConclusionCount}");
                    sw.WriteLine($"Total, {TotalCount}");

                    sw.Flush();
                    fs.Flush(true);
                }

                _logger.Info($"Save {Type.ToString()} done, {fileName}{Environment.NewLine}{ToString()}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            try
            {
                sb.AppendLine($"Chart: {ChartCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"CircuitBreak: {CircuitBreakCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"BiddingPrice: {BiddingPriceCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"StockMaster: {StockMasterCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"IndexMaster: {IndexMasterCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"StockConclusion: {StockConclusionCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"IndexConclusion: {IndexConclusionCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"ETFConclusion: {ETFConclusionCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"TradeConclusion: {TradeConclusionCount.ToString(Config.General.CurrencyFormat)}");
                sb.Append($"Total: {TotalCount.ToString(Config.General.CurrencyFormat)}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return sb.ToString();
        }
    }
}
