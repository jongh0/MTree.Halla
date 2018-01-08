using MongoDB.Bson;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Surrogates
{
    [ProtoContract]
    public class ObjectIdSurrogate
    {
        [ProtoMember(1)]
        public string Data { get; set; }

        public static implicit operator ObjectIdSurrogate(ObjectId objectId)
        {
            return objectId != null ? new ObjectIdSurrogate { Data = objectId.ToString() } : null;
        }

        public static implicit operator ObjectId(ObjectIdSurrogate surrogate)
        {
            if (ObjectId.TryParse(surrogate.Data, out var objectId) == true)
                return objectId;

            return new ObjectId();
        }
    }
}
