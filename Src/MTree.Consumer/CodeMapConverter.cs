using MTree.DataStructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class CodeMapConverter
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static ICodeMap JsonStringToCodeMap(string headName, string jsonString)
        {
            Dictionary<string, object> deserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString, new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    logger.Error($"Configuration deserialize error, {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true;
                }
            });

            ICodeMap child = JsonDicToCodeMap("Codemap", deserialized);
            return child;
        }

        private static ICodeMap JsonDicToCodeMap(string headName, Dictionary<string, object> deserialized)
        {
            CodeMapHead ret = new CodeMapHead(headName);

            List<string> keyList = new List<string>(deserialized.Keys);
            foreach (string key in keyList)
            {
                try
                {
                    if (string.IsNullOrEmpty(deserialized[key].ToString().Trim('{').Trim('}'))) // Entity is empty.
                    {
                        ret.Add(new CodeMapHead(key));
                        continue;
                    }
                    if (deserialized[key].ToString().Contains(':') == false) // For leap node (Stock instance)
                    {
                        Stock s = Stock.GetStock(key);
                        if (string.IsNullOrEmpty(s.Name))
                        {
                            s.Name = deserialized[key].ToString();
                        }
                        else
                        {
                            if (s.Name != deserialized[key].ToString())
                            {
                                // TODO : Select correct stock name
                                logger.Error($"Stock instance has diffrent name, {s.Name} vs. {deserialized[key].ToString()}");
                            }
                        }
                        ret.Add(s);
                    }
                    else // For composite
                    {
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

                        ret.Add(JsonDicToCodeMap(key, deserializedChild));
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            return ret;
        }

        public static string CodeMapToJsonString(ICodeMap codeMapHead, bool indented = false)
        {
            return indented == false ? JsonConvert.SerializeObject(CodeMapToDic(codeMapHead)) :
                JsonConvert.SerializeObject(CodeMapToDic(codeMapHead), Formatting.Indented);
        }

        public static Dictionary<string, object> CodeMapToDic(ICodeMap codeMap)
        {
            if (codeMap is Stock)
                return null;

            Dictionary<string, object> ret = new Dictionary<string, object>();

            foreach (ICodeMap child in ((CodeMapHead)codeMap).CodeMapList)
            {
                if (child is CodeMapHead)
                {
                    ret.Add(((CodeMapHead)child).Name, CodeMapToDic(child));
                }
                else if (child is Stock)
                {
                    ret.Add(((Stock)child).Code, ((Stock)child).Name);
                }
                else
                {
                    // Error
                }
            }

            return ret;
        }

        public static ICodeMap DicToCodeMap(string headName, Dictionary<string, object> dic)
        {
            CodeMapHead ret = new CodeMapHead(headName);

            List<string> keyList = new List<string>(dic.Keys);
            foreach (string key in keyList)
            {
                try
                {
                    if (dic[key] is string) // For leap node (Stock instance)
                    {
                        Stock s = Stock.GetStock(key);
                        if (string.IsNullOrEmpty(s.Name))
                        {
                            s.Name = dic[key].ToString();
                        }
                        else
                        {
                            if (s.Name != dic[key].ToString())
                            {
                                // TODO : Select correct stock name
                                logger.Warn($"Stock instance has diffrent name, {s.Name} vs. {dic[key].ToString()}");
                            }
                        }
                        ret.Add(s);
                    }
                    else // For composite
                    {
                        ret.Add(DicToCodeMap(key, (Dictionary<string, object>)dic[key]));
                    }
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
