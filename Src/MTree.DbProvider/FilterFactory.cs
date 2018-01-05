using MongoDB.Driver;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DbProvider
{
    public class FilterFactory
    {
        private static readonly object _lockObject = new object();
        private static volatile FilterFactory _Instance;
        public static FilterFactory Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_Instance == null)
                            _Instance = new FilterFactory();
                    }
                }

                return _Instance;
            }
        }
        private FilterFactory()
        {
        }

        public FilterDefinition<IndexMaster> BuildIndexMasterFilter(DateTime targetDate)
        {
            var builder = Builders<IndexMaster>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }

        public FilterDefinition<StockMaster> BuildStockMasterFilter(DateTime targetDate)
        {
            var builder = Builders<StockMaster>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }

        public FilterDefinition<StockConclusion> BuildStockConclusionFilter(DateTime targetDate, bool normalOnly = false)
        {
            var builder = Builders<StockConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));
            if (normalOnly == true)
                filter = filter & builder.Eq(i => i.MarketTimeType, MarketTimeTypes.Normal);

            return filter;
        }

        public FilterDefinition<IndexConclusion> BuildIndexConclusionFilter(DateTime targetDate, bool normalOnly = false)
        {
            var builder = Builders<IndexConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));
            if (normalOnly == true)
                filter = filter & builder.Eq(i => i.MarketTimeType, MarketTimeTypes.Normal);

            return filter;
        }
        public FilterDefinition<CircuitBreak> BuildCircuitBreakFilter(DateTime targetDate, bool normalOnly = false)
        {
            var builder = Builders<CircuitBreak>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }

    }
}
