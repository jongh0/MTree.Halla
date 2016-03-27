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
        private string _code;
        public string Code
        {
            get { return _code; }
            set { _code = value;  NotifyPropertyChanged(nameof(Code)); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(nameof(Name)); }
        }

        private float _price;
        public float Price
        {
            get { return _price; }
            set { _price = value; NotifyPropertyChanged(nameof(Price)); }
        }

        private long _volume;
        public long Volume
        {
            get { return _volume; }
            set { _volume = value; NotifyPropertyChanged(nameof(Volume)); }
        }

        private float _previousClosedPrice;
        public float PreviousClosedPrice
        {
            get { return _previousClosedPrice; }
            set { _previousClosedPrice = value; NotifyPropertyChanged(nameof(PreviousClosedPrice)); }
        }

        private long _previousVolume;
        public long PreviousVolume
        {
            get { return _previousVolume; }
            set { _previousVolume = value; NotifyPropertyChanged(nameof(PreviousVolume)); }
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
