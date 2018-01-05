using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    public class CodeEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public MarketTypes MarketType { get; set; } = MarketTypes.Unknown;

        public override string ToString()
        {
            return $"{Code}/{Name}/{MarketType}";
        }

        public static string ConvertToDaishinCode(CodeEntity codeEntity)
        {
            switch (codeEntity.MarketType)
            {
                case MarketTypes.INDEX: return "U" + codeEntity.Code;
                case MarketTypes.ELW:   return "J" + codeEntity.Code;
                case MarketTypes.ETN:   return "Q" + codeEntity.Code;
                default:                return "A" + codeEntity.Code;
            }
        }

        public static string RemovePrefix(string fullCode)
        {
            if (fullCode?.Length == 7)
                return fullCode.Substring(1);
            else if (fullCode?.Length == 4)
                return fullCode.Substring(1);
            else
                return fullCode;
        }
    }
}
