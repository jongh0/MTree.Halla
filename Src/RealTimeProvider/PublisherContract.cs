using CommonLib;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    [DataContract]
    [ProtoContract]
    public class PublisherContract
    {
        [IgnoreDataMember]
        [ProtoIgnore]
        public static int IdNumbering { get; set; } = 0;

        [IgnoreDataMember]
        [ProtoIgnore]
        public int Id { get; set; } = -1;

        [DataMember]
        [ProtoMember(1)]
        public ProcessTypes Type { get; set; }

        [IgnoreDataMember]
        [ProtoIgnore]
        public IRealTimePublisherCallback Callback { get; set; } = null;

        [IgnoreDataMember]
        [ProtoIgnore]
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
