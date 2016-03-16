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

        public string ConnectionString { get; set; } = "Endpoint=sb://mtreenotificationspace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=RQDGxAhXvvS/Mgg5AYwx5FfecJifbIEadaWu/44LzOo=";

        public string NotificationHubPath { get; set; } = "MTreeNotificationHub";
    }
}
