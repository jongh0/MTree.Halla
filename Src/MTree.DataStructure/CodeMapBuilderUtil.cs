using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
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

        public static Dictionary<string, object> RebuildNode(Dictionary<string, object> deserialized)
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

        public static string ConvertToJsonString(Dictionary<string, object> codeMapHeader)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, codeMapHeader);
            }
            return sb.ToString();
        }
    }
}
