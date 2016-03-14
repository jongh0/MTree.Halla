using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class CodeEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public MarketType Market { get; set; } = MarketType.UNKNOWN;

        public override string ToString()
        {
            return $"{Code}/{Name}/{Market}";
        }

        public static string ConvertToDaishinCode(CodeEntity entity)
        {
            switch (entity.Market)
            {
                case MarketType.KOSPI:
                    return "A" + entity.Code;
                default:
                    return entity.Code;
            }
        }
    }
}
