﻿using MongoDB.Bson;
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

        private int _ChartCount = 0;
        public int ChartCount
        {
            get { return _ChartCount; }
            set
            {
                if (_ChartCount != value)
                {
                    _ChartCount = value;
                    NotifyPropertyChanged(nameof(ChartCount));
                }
            }
        }

        private int _BiddingPriceCount = 0;
        public int BiddingPriceCount
        {
            get { return _BiddingPriceCount; }
            set
            {
                if (_BiddingPriceCount != value)
                {
                    _BiddingPriceCount = value;
                    NotifyPropertyChanged(nameof(BiddingPriceCount));
                }
            }
        }

        private int _CircuitBreakCount = 0;
        public int CircuitBreakCount
        {
            get { return _CircuitBreakCount; }
            set
            {
                if (_CircuitBreakCount != value)
                {
                    _CircuitBreakCount = value;
                    NotifyPropertyChanged(nameof(CircuitBreakCount));
                }
            }
        }

        private int _StockMasterCount = 0;
        public int StockMasterCount
        {
            get { return _StockMasterCount; }
            set
            {
                if (_StockMasterCount != value)
                {
                    _StockMasterCount = value;
                    NotifyPropertyChanged(nameof(StockMasterCount));
                }
            }
        }

        private int _IndexMasterCount = 0;
        public int IndexMasterCount
        {
            get { return _IndexMasterCount; }
            set
            {
                if (_IndexMasterCount != value)
                {
                    _IndexMasterCount = value;
                    NotifyPropertyChanged(nameof(IndexMasterCount));
                }
            }
        }

        private int _StockConclusionCount = 0;
        public int StockConclusionCount
        {
            get { return _StockConclusionCount; }
            set
            {
                if (_StockConclusionCount != value)
                {
                    _StockConclusionCount = value;
                    NotifyPropertyChanged(nameof(StockConclusionCount));
                }
            }
        }

        private int _IndexConclusionCount = 0;
        public int IndexConclusionCount
        {
            get { return _IndexConclusionCount; }
            set
            {
                if (_IndexConclusionCount != value)
                {
                    _IndexConclusionCount = value;
                    NotifyPropertyChanged(nameof(IndexConclusionCount));
                }
            }
        }

        private int _ETFConclusionCount = 0;
        public int ETFConclusionCount
        {
            get { return _ETFConclusionCount; }
            set
            {
                if (_ETFConclusionCount != value)
                {
                    _ETFConclusionCount = value;
                    NotifyPropertyChanged(nameof(ETFConclusionCount));
                }
            }
        }

        public int TotalCount
        {
            get { return ChartCount + StockMasterCount + IndexMasterCount + BiddingPriceCount + CircuitBreakCount + StockConclusionCount + IndexConclusionCount + ETFConclusionCount; }
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
                    Interlocked.Increment(ref _ChartCount);
                    break;

                case CounterTypes.BiddingPrice:
                    Interlocked.Increment(ref _BiddingPriceCount);
                    break;

                case CounterTypes.CircuitBreak:
                    Interlocked.Increment(ref _CircuitBreakCount);
                    break;

                case CounterTypes.StockMaster:
                    Interlocked.Increment(ref _StockMasterCount);
                    break;

                case CounterTypes.IndexMaster:
                    Interlocked.Increment(ref _IndexMasterCount);
                    break;

                case CounterTypes.StockConclusion:
                    Interlocked.Increment(ref _StockConclusionCount);
                    break;

                case CounterTypes.IndexConclusion:
                    Interlocked.Increment(ref _IndexConclusionCount);
                    break;

                case CounterTypes.ETFConclusion:
                    Interlocked.Increment(ref _ETFConclusionCount);
                    break;
            }
        }

        public void Add(CounterTypes type, int value)
        {
            switch (type)
            {
                case CounterTypes.Chart:
                    Interlocked.Add(ref _ChartCount, value);
                    break;

                case CounterTypes.BiddingPrice:
                    Interlocked.Add(ref _BiddingPriceCount, value);
                    break;

                case CounterTypes.CircuitBreak:
                    Interlocked.Add(ref _CircuitBreakCount, value);
                    break;

                case CounterTypes.StockMaster:
                    Interlocked.Add(ref _StockMasterCount, value);
                    break;

                case CounterTypes.IndexMaster:
                    Interlocked.Add(ref _IndexMasterCount, value);
                    break;

                case CounterTypes.StockConclusion:
                    Interlocked.Add(ref _StockConclusionCount, value);
                    break;

                case CounterTypes.IndexConclusion:
                    Interlocked.Add(ref _IndexConclusionCount, value);
                    break;

                case CounterTypes.ETFConclusion:
                    Interlocked.Add(ref _ETFConclusionCount, value);
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