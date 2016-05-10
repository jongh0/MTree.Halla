using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataValidator
{
    public class RecoveryViewModel: INotifyPropertyChanged
    {
        public DataValidator Validator { get; set; } = new DataValidator();

        private DateTime _TargetDate = DateTime.Now;
        public DateTime TargetDate
        {
            get
            {
                return _TargetDate;
            }
            set
            {
                _TargetDate = value;
                NotifyPropertyChanged(nameof(TargetDate));
            }
        }

        private string _Code;
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private bool _ShowDifferent = true;
        public bool ShowDifferent
        {
            get
            {
                return _ShowDifferent;
            }
            set
            {
                _ShowDifferent = value;
                NotifyPropertyChanged(nameof(ShowDifferent));
            }
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
