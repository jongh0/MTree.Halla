﻿using MongoDB.Driver;
using MTree.DataStructure;
using MTree.DbProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DbProvider
{
    public class DataLoader
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<T> Load<T>(string code, DateTime startDate, DateTime endDate)
        {
            int startTick = Environment.TickCount;

            try
            {
                startDate = DateTimeUtility.StartDateTime(startDate);
                endDate = DateTimeUtility.EndDateTime(endDate);

                var builder = Builders<T>.Filter;
                var filter = builder.Gte(i => (i as Subscribable).Time, startDate) & builder.Lte(i => (i as Subscribable).Time, endDate);

                return DbAgent.Instance.Find(code, filter).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                var duration = Environment.TickCount - startTick;
                logger.Info($"Load, {nameof(code)}: {code}, {nameof(startDate)}: {startDate}, {nameof(endDate)}: {endDate}, {nameof(duration)}: {duration}");
            }

            return new List<T>();
        }
    }
}