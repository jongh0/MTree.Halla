using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [Serializable]
    public class InvestWarning
    {
        /// <summary>
        /// 지정유무
        /// </summary>
        public bool IsDesignated { get; set; }

        /// <summary>
        /// 공시일
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Disclosure { get; set; }

        /// <summary>
        /// 지정일
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Start { get; set; }

        /// <summary>
        /// 해지일
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime End { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"IsDesignated: {IsDesignated}");
            sb.AppendLine($"Disclosure: {Disclosure.ToShortDateString()}");
            sb.AppendLine($"Start: {Start.ToShortDateString()}");
            sb.AppendLine($"End: {End.ToShortDateString()}");


            return sb.ToString();
        }
    }
}
