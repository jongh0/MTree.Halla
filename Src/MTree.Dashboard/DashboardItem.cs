using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                }
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

        private float _previousClosedPrice = 0;
        public float PreviousClosedPrice
        {
            get { return _previousClosedPrice; }
            set
            {
                if (_previousClosedPrice != value)
                {
                    _previousClosedPrice = value;
                    NotifyPropertyChanged(nameof(PreviousClosedPrice));
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
