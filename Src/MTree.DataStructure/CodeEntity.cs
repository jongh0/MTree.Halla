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

        public MarketTypes MarketType { get; set; } = MarketTypes.Unknown;

        public override string ToString()
        {
            return $"{Code}/{Name}/{MarketType}";
        }

        public static string ConvertToDaishinCode(CodeEntity entity)
        {
            switch (entity.MarketType)
            {
                case MarketTypes.ELW:
                    return "J" + entity.Code;
                case MarketTypes.ETN:
                    return "Q" + entity.Code;
                default:
                    return "A" + entity.Code;
            }
        }

        public static MarketTypes ConvertToMarketType(string fullCode)
        {
            switch (fullCode[0])
            {
                case 'J':
                    return MarketTypes.ELW;
                case 'Q':
                    return MarketTypes.ETN;
                default:
                    return MarketTypes.KOSPI;
            }
        }

        public static string RemovePrefix(string code)
        {
            if (code.Length > 6)
                return code.Substring(1);
            else
                return code;
        }
    }
}
