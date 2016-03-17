﻿using System;
using MTree.DataStructure;
using System.Collections.Concurrent;

namespace MTree.Consumer
{
    public class Index : IChartable
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static ConcurrentDictionary<string, Index> Indexes { get; set; } = new ConcurrentDictionary<string, Index>();

        public string Code { get; set; }

        public DateTime LastConcludedTime { get; set; }

        public float LastValue { get; set; }

        public Chart GetChart(ChartTypes chartType, DateTime target, TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        public static Index GetIndex(string code)
        {
            try
            {
                if (Indexes.ContainsKey(code) == false)
                {
                    var index = new Index();
                    index.Code = code;

                    Indexes.TryAdd(code, index);
                    return index;
                }

                return Indexes[code];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }
    }
}