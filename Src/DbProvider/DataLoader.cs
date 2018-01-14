using MongoDB.Driver;
using DataStructure;
using DbProvider;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Utility;

namespace DbProvider
{
    public class DataLoader
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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
                _logger.Error(ex);
            }
            finally
            {
                var duration = Environment.TickCount - startTick;
                _logger.Info($"Load, {nameof(code)}: {code}, {nameof(startDate)}: {startDate}, {nameof(endDate)}: {endDate}, {nameof(duration)}: {duration}");
            }

            return new List<T>();
        }
    }
}
