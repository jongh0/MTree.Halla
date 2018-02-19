using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Daishin
{
    public interface IDaishinSubscribe
    {
        bool Subscribe(string code);

        bool Unsubscribe();

        bool WaitResponse();
    }
}
