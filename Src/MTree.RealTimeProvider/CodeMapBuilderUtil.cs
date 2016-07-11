using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public static class CodeMapBuilderUtil
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static Dictionary<string, object> BuildNode(Dictionary<string, object> parent, string name, object child = null)
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
                        parent[name] = ConcatNode((Dictionary<string, object>)parent[name], (Dictionary<string, object>)child);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return (Dictionary<string, object>)parent[name];
        }

        private static Dictionary<string, object> ConcatNode(Dictionary<string, object> a, Dictionary<string, object> b)
        {
            Dictionary<string, object> returnDic = new Dictionary<string, object>();

            returnDic = a;
            foreach (KeyValuePair<string, object> node in b)
            {
                if (returnDic.ContainsKey(node.Key))
                {
                    returnDic[node.Key] = ConcatNode((Dictionary<string, object>)(a[node.Key]), (Dictionary<string, object>)(b[node.Key]));
                }
                else
                {
                    returnDic.Add(node.Key, node.Value);
                }
            }
            return returnDic;
        }

        public static string ConvertToJsonString(Dictionary<string, object> codeMapHead)
        {
            /*
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, codeMapHead);
            }
            return sb.ToString();
            */
            return JsonConvert.SerializeObject(codeMapHead, Formatting.Indented);
        }

        public static Dictionary<string, object> RebuildNode(string jsonString)
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

        private static Dictionary<string, object> RebuildNode(Dictionary<string, object> deserialized)
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
