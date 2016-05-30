using MTree.DataStructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public class CodeMapBuilder
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public CodeMapBuilder(PublisherContract contract)
        {
            try
            {
                var codeList = contract.Callback.GetCodeList();

                Dictionary<string, object> codeMapHeader = new Dictionary<string, object>();
                
                Dictionary<string, object> marketCodeMapHeader = BuildNode(codeMapHeader, "MaketType");

                foreach (KeyValuePair<string, CodeEntity> codeEntity in codeList)
                {
                    if (codeEntity.Value.MarketType != MarketTypes.INDEX &&
                        codeEntity.Value.MarketType != MarketTypes.ELW)
                    {
                        Dictionary<string, object> newChild = new Dictionary<string, object>();
                        newChild.Add(codeEntity.Value.Code, codeEntity.Value.Name);
                        BuildNode(marketCodeMapHeader, codeEntity.Value.MarketType.ToString(), newChild);
                    }
                }
                
                Dictionary<string, object> theme = BuildNode(codeMapHeader, "Theme");
                BuildNode(theme, "DaishinTheme", contract.Callback.GetThemeList());

                ////////////////////////////////////////////////////////////////////////////////

                using (StreamWriter stream = File.CreateText("CodeMap.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(stream, codeMapHeader);
                }
                
                Dictionary<string, object> rebuilt = RebuildNode(File.ReadAllText("CodeMap.json"));

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private Dictionary<string, object> BuildNode(Dictionary<string, object> parent, string name, object child = null)
        {
            try
            {
                if (child == null)
                {
                    if (parent.ContainsKey(name) == false)
                        parent.Add(name, new Dictionary<string, object>());
                }
                else
                {
                    if (parent.ContainsKey(name) == false)
                        parent.Add(name, child);
                    else
                    {
                        Dictionary<string, object> originDic = (Dictionary<string, object>)parent[name];
                        parent.Remove(name);
                        parent.Add(name, originDic.Concat((Dictionary<string, object>)child).ToDictionary(x => x.Key, x => x.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return (Dictionary<string, object>)parent[name];
        }

        private Dictionary<string, object> RebuildNode(string jsonString)
        {
            Dictionary<string, object> deserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString, new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    logger.Error($"Configuration deserialize error, {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true;
                }
            });

            return RebuildNode(deserialized);
        }

        private Dictionary<string, object> RebuildNode(Dictionary<string, object> deserialized)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            List<string> keyList = new List<string>(deserialized.Keys);
            foreach (string key in keyList)
            {
                try
                {
                    if (deserialized[key].ToString().Contains(':') == false)
                        continue;

                    Dictionary<string, object> deserializedChild = JsonConvert.DeserializeObject<Dictionary<string, object>>(deserialized[key].ToString(), new JsonSerializerSettings
                    {
                        Error = (sender, args) =>
                        {
                            logger.Error($"Configuration deserialize error, {args.ErrorContext.Error.Message}");
                            args.ErrorContext.Handled = true;
                        }
                    });
                    if (deserializedChild == null)
                        continue;

                    Dictionary<string, object> children = RebuildNode(deserializedChild);

                    if (children == null || children.Count() == 0)
                        ret.Add(key, deserializedChild);
                    else
                        ret.Add(key, children);
                    
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            return ret;
        }
    }
}
