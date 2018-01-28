using MongoDB.Driver;
using DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProvider
{
    public static class FilterFactory
    {
        public static FilterDefinition<IndexMaster> BuildIndexMasterFilter(DateTime targetDate)
        {
            var builder = Builders<IndexMaster>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }

        public static FilterDefinition<StockMaster> BuildStockMasterFilter(DateTime targetDate)
        {
            var builder = Builders<StockMaster>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }

        public static FilterDefinition<StockConclusion> BuildStockConclusionFilter(DateTime targetDate, bool normalOnly = false)
        {
            var builder = Builders<StockConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));
            if (normalOnly == true)
                filter = filter & builder.Eq(i => i.MarketTimeType, MarketTimeTypes.Normal);

            return filter;
        }

        public static FilterDefinition<IndexConclusion> BuildIndexConclusionFilter(DateTime targetDate, bool normalOnly = false)
        {
            var builder = Builders<IndexConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));
            if (normalOnly == true)
                filter = filter & builder.Eq(i => i.MarketTimeType, MarketTimeTypes.Normal);

            return filter;
        }

        public static FilterDefinition<ETFConclusion> BuildETFConclusionFilter(DateTime targetDate)
        {
            var builder = Builders<ETFConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }

        public static FilterDefinition<TradeConclusion> BuildTradeConclusionFilter(DateTime targetDate)
        {
            var builder = Builders<TradeConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }

        public static FilterDefinition<CircuitBreak> BuildCircuitBreakFilter(DateTime targetDate)
        {
            var builder = Builders<CircuitBreak>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            return filter;
        }
    }
}
