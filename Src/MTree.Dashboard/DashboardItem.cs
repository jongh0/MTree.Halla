using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MTree.Dashboard
{
    public class DashboardItem : INotifyPropertyChanged
    {
        private DateTime _Time = DateTime.MinValue;
        public DateTime Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                NotifyPropertyChanged(nameof(Time));
            }
        }

        private string _Code = string.Empty;
        public string Code
        {
            get { return _Code; }
            set
            {
                _Code = value;
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        private float _Price = 0;
        public float Price
        {
            get { return _Price; }
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                    NotifyPropertyChanged(nameof(Price));
                    NotifyPropertyChanged(nameof(PriceColor));
                    NotifyPropertyChanged(nameof(PricePercent));
                }
            }
        }

        public float PricePercent
        {
            get
            {
                if (Price == 0 || BasisPrice == 0)
                    return 0;

                return (Price - BasisPrice) / BasisPrice;
            }
        }

        public Brush PriceColor
        {
            get
            {
                if (BasisPrice == 0)
                    return Brushes.Black;
                else if (BasisPrice < Price)
                    return Brushes.Red;
                else if (BasisPrice > Price)
                    return Brushes.Blue;
                else
                    return Brushes.Black;
            }
        }

        private long _Volume = 0;
        public long Volume
        {
            get { return _Volume; }
            set
            {
                if (_Volume != value)
                {
                    _Volume = value;
                    NotifyPropertyChanged(nameof(Volume));
                }
            }
        }

        private float _BasisPrice = 0;
        public float BasisPrice
        {
            get { return _BasisPrice; }
            set
            {
                if (_BasisPrice != value)
                {
                    _BasisPrice = value;
                    NotifyPropertyChanged(nameof(BasisPrice));
                }
            }
        }

        private long _PreviousVolume = 0;
        public long PreviousVolume
        {
            get { return _PreviousVolume; }
            set
            {
                if (_PreviousVolume != value)
                {
                    _PreviousVolume = value;
                    NotifyPropertyChanged(nameof(PreviousVolume));
                }
            }
        }

        private CircuitBreakTypes _CircuitBreakType = CircuitBreakTypes.Clear;
        public CircuitBreakTypes CircuitBreakType
        {
            get { return _CircuitBreakType; }
            set
            {
                if (_CircuitBreakType != value)
                {
                    _CircuitBreakType = value;
                    NotifyPropertyChanged(nameof(CircuitBreakType));
                }
            }
        }

        private MarketTypes _MarketType = MarketTypes.Unknown;
        public MarketTypes MarketType
        {
            get { return _MarketType; }
            set
            {
                _MarketType = value;
                NotifyPropertyChanged(nameof(MarketType));
            }
        }

        public DashboardItem()
        {
        }

        public DashboardItem(string code)
        {
            Code = code;
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
