using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    [DataContract]
    public class PublisherContract
    {
        public static int IdNumbering { get; set; } = 0;

        public int Id { get; set; } = -1;

        [DataMember]
        public ProcessTypes Type { get; set; }

        public IRealTimePublisherCallback Callback { get; set; } = null;

        public bool IsOperating { get; set; } = false;

        public override string ToString()
        {
            return $"{Type}/{Id}/{IsOperating}";
        }

        public static ProcessTypes ConvertToType(string value)
        {
            if (Enum.TryParse<ProcessTypes>(value, out var type) == true)
                return type;
            else
                return ProcessTypes.Unknown;
        }
    }
}
