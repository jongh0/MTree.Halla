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

        public MarketType Market { get; set; } = MarketType.Unknown;

        public override string ToString()
        {
            return $"{Code}/{Name}/{Market}";
        }

        public static string ConvertToDaishinCode(CodeEntity entity)
        {
            switch (entity.Market)
            {
                case MarketType.ELW:
                    return "J" + entity.Code;
                case MarketType.ETN:
                    return "Q" + entity.Code;
                default:
                    return "A" + entity.Code;
            }
        }

        public static MarketType ConvertToMarketType(string fullCode)
        {
            switch (fullCode[0])
            {
                case 'J':
                    return MarketType.ELW;
                case 'Q':
                    return MarketType.ETN;
                default:
                    return MarketType.KOSPI;
            }
        }
    }
}
