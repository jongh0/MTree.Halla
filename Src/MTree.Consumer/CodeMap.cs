using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public interface ICodeMap
    {
        string Name { get; set; }
    }
    public class CodeMapHeader: ICodeMap
    {
        private List<ICodeMap> codeMapList = new List<ICodeMap>();

        public string Name { get; set; }

        public void Add(ICodeMap codeMap)
        {
            codeMapList.Add(codeMap);
        }
    }
    public class CodeMapElement : ICodeMap
    {
        public string Name { get; set; }
    }
}
