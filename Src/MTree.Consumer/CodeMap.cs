﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public interface ICodeMap
    {
        string Name { get; set; }
    }

    public class CodeMapHead : ICodeMap
    {
        public List<ICodeMap> CodeMapList { get; set; }

        public string Name { get; set; }

        public CodeMapHead(string name = "")
        {
            Name = name;
            CodeMapList = new List<ICodeMap>();
        }

        public void Add(ICodeMap codeMap)
        {
            CodeMapList.Add(codeMap);
        }
    }

    public class CodeMapConverter
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static ICodeMap JsonToCodeMap(string headName, string jsonString)
        {
            Dictionary<string, object> deserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString, new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    logger.Error($"Configuration deserialize error, {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true;
                }
            });
           
            ICodeMap child = JsonToCodeMap("Codemap", deserialized);
            return child;
        }

        private static ICodeMap JsonToCodeMap(string headName, Dictionary<string, object> deserialized)
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

                        ret.Add(JsonToCodeMap(key, deserializedChild));
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
