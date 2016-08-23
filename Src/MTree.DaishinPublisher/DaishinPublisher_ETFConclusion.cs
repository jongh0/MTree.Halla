using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        private int _ETFSubscribeCount = 0;
        public int ETFSubscribeCount
        {
            get { return _ETFSubscribeCount; }
            set
            {
                _ETFSubscribeCount = value;
                NotifyPropertyChanged(nameof(ETFSubscribeCount));
            }
        }
    }
}
