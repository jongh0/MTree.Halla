using MongoDB.Driver;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataValidator
{
    public class DbCollector : IDataCollector
    {
        private DbAgent DataSource { get; set; } = DbAgent.Instance;

        public DbCollector(DbAgent dataSource = null)
        {
            if (dataSource != null)
                DataSource = dataSource;
        }

        public List<string> GetStockCodeList()
        {
            return DataSource.GetCollectionList(DbTypes.StockMaster);
        }

        public List<string> GetIndexCodeList()
        {
            return DataSource.GetCollectionList(DbTypes.IndexMaster);
        }

        public StockMaster GetMaster(string code, DateTime targetDate)
        {
            var builder = Builders<StockMaster>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));
            return DataSource.Find(code, filter).FirstOrDefault();
        }

        public List<Subscribable> GetIndexConclusions(string code, DateTime targetDate, bool normalOnly = true)
        {
            List<Subscribable> conclusions = new List<Subscribable>();

            var builder = Builders<IndexConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            foreach (Subscribable conclusion in DataSource.Find(code, filter).SortBy(o => o.ReceivedTime).ToList())
            {
                if (((IndexConclusion)conclusion).MarketTimeType == MarketTimeTypes.Normal)
                {
                    conclusions.Add(conclusion);
                }
            }

            return conclusions;
        }

        public List<Subscribable> GetStockConclusions(string code, DateTime targetDate, bool normalOnly = true)
        {
            List<Subscribable> conclusions = new List<Subscribable>();

            var builder = Builders<StockConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));
            
            foreach (Subscribable conclusion in DataSource.Find(code, filter).SortBy(o => o.ReceivedTime).ToList())
            {
                if (conclusion.Time < new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 15, 00, 0))
                {
                    if (normalOnly == true)
                    {
                        if (((StockConclusion)conclusion).MarketTimeType == MarketTimeTypes.Normal)
                        {
                            conclusions.Add(conclusion);
                        }
                    }
                    else
                    {
                        conclusions.Add(conclusion);
                    }
                }
            }
            
            return conclusions;
        }

        public List<Subscribable> GetCircuitBreaks(string code, DateTime targetDate)
        {
            List<Subscribable> cbs = new List<Subscribable>();

            var builder = Builders<CircuitBreak>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            foreach (Subscribable conclusion in DataSource.Find(code, filter).SortBy(o => o.ReceivedTime).ToList())
            {
                cbs.Add(conclusion);
            }

            return cbs;
        }
    }
}
