using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class InvestWarningEntity
    {
        /// <summary>
        /// 지정유무
        /// </summary>
        [BsonElement("IDe")]
        public bool IsDesignated { get; set; }

        /// <summary>
        /// 공시일
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("Disc")]
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
            return $"{IsDesignated}, {Disclosure}, {Start}, {End}";
        }
    }
}
