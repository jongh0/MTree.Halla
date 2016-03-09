using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public enum PublishType
    {
        None,
        Daishin,
        Ebest,
        Krx,
        Naver,
    }

    [Serializable]
    public class PublishContract
    {
        public static PublishType ConvertToType(string value)
        {
            PublishType type;
            if (Enum.TryParse(value, out type) == true)
                return type;
            else
                return PublishType.None;
        }

        public PublishType Type { get; set; }

        public IRealTimePublisherCallback Callback { get; set; } = null;
    }
}
