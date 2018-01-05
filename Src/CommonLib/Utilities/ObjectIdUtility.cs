using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib
{
    public class ObjectIdUtility
    {
        private static int index = 0;

        private static int machine;

        private static short pid;

        static ObjectIdUtility()
        {
            var id = ObjectId.GenerateNewId();
            machine = id.Machine;
            pid = id.Pid;
        }

        public static ObjectId GenerateNewId()
        {
            return GenerateNewId(DateTime.Now);
        }

        public static ObjectId GenerateNewId(DateTime dateTime)
        {
            Interlocked.Add(ref index, 1);
            return new ObjectId(dateTime, machine, pid, index);
        }
    }
}
