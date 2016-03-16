using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Configuration
{
    public class PushConfiguration
    {
        public static readonly string FileName = "Config.Push.json";

        public string GcmSenderId { get; set; } = "mtree-1251";

        public string GcmAuthToken { get; set; } = "AIzaSyAXEZu7i1m60ZHKgMM0uKkfVw6xkYiUDPg";

        public List<string> RegistrationIds { get; set; } = new List<string>() { "" };
    }
}
