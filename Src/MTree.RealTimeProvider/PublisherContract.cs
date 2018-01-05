using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    [DataContract]
    public class PublisherContract
    {
        [DataMember]
        public static int IdNumbering { get; set; } = 0;

        [DataMember]
        public int Id { get; set; } = -1;

        [DataMember]
        public ProcessTypes Type { get; set; }

        [DataMember]
        public IRealTimePublisherCallback Callback { get; set; } = null;

        [DataMember]
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
