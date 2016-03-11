using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    [Serializable]
    public class PublishContract
    {
        public static int IdNumbering { get; set; } = 0;

        public int Id { get; set; } = -1;

        public ProcessType Type { get; set; }

        public IRealTimePublisherCallback Callback { get; set; } = null;

        public bool NowOperating { get; set; } = false;

        public override string ToString()
        {
            return $"{Type}/{Id}/{NowOperating}";
        }

        public static ProcessType ConvertToType(string value)
        {
            ProcessType type;
            if (Enum.TryParse(value, out type) == true)
                return type;
            else
                return ProcessType.None;
        }
    }
}
