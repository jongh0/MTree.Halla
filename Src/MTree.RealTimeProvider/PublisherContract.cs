using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    [Serializable]
    public class PublisherContract
    {
        public static int IdNumbering { get; set; } = 0;

        public int Id { get; set; } = -1;

        public ProcessTypes Type { get; set; }

        public IRealTimePublisherCallback Callback { get; set; } = null;

        public bool IsOperating { get; set; } = false;

        public override string ToString()
        {
            return $"{Type}/{Id}/{IsOperating}";
        }

        public static ProcessTypes ConvertToType(string value)
        {
            ProcessTypes type;
            if (Enum.TryParse(value, out type) == true)
                return type;
            else
                return ProcessTypes.None;
        }
    }
}
