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
        private string _code = string.Empty;
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        private float _price = 0;
        public float Price
        {
            get { return _price; }
            set
            {
                if (_price != value)
                {
                    _price = value;
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

        private long _volume = 0;
        public long Volume
        {
            get { return _volume; }
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    NotifyPropertyChanged(nameof(Volume));
                }
            }
        }

        private float _basisPrice = 0;
        public float BasisPrice
        {
            get { return _basisPrice; }
            set
            {
                if (_basisPrice != value)
                {
                    _basisPrice = value;
                    NotifyPropertyChanged(nameof(BasisPrice));
                }
            }
        }

        private long _previousVolume = 0;
        public long PreviousVolume
        {
            get { return _previousVolume; }
            set
            {
                if (_previousVolume != value)
                {
                    _previousVolume = value;
                    NotifyPropertyChanged(nameof(PreviousVolume));
                }
            }
        }

        private CircuitBreakTypes _circuitBreakType = CircuitBreakTypes.Clear;
        public CircuitBreakTypes CircuitBreakType
        {
            get { return _circuitBreakType; }
            set
            {
                if (_circuitBreakType != value)
                {
                    _circuitBreakType = value;
                    NotifyPropertyChanged(nameof(CircuitBreakType));
                }
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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
