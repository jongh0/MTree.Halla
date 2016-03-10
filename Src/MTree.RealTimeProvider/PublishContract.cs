using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public enum PublisherType
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
        public static PublisherType ConvertToType(string value)
        {
            PublisherType type;
            if (Enum.TryParse(value, out type) == true)
                return type;
            else
                return PublisherType.None;
        }

        public PublisherType Type { get; set; }

        public IRealTimePublisherCallback Callback { get; set; } = null;
    }
}
