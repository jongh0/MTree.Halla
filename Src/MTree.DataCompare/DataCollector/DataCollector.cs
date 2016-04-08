using MongoDB.Driver;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataCompare
{
    public class DbCollector : IDataCollector
    {
        private DbAgent DataSource { get; set; } = DbAgent.Instance;

        public DbCollector(DbAgent dataSource = null)
        {
            if (dataSource != null)
                DataSource = dataSource;
        }

        public List<string> GetCodeList()
        {
            return DataSource.GetCollectionList(DbTypes.StockMaster);
        }

        public StockMaster GetMaster(string code, DateTime targetDate)
        {
            var builder = Builders<StockMaster>.Filter;
            var filter = builder.Eq(i => i.Time, targetDate);
            return DataSource.Find(code, filter).FirstOrDefault();
        }

        public List<Subscribable> GetIndexConclusions(string code, DateTime targetDate, bool normalOnly = true)
        {
            List<Subscribable> conclusions = new List<Subscribable>();

            var builder = Builders<IndexConclusion>.Filter;
            var filter = builder.Gte(i => i.Time, targetDate) & builder.Lt(i => i.Time, targetDate.AddDays(1));

            foreach (Subscribable conclusion in DataSource.Find(code, filter).ToList())
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
            
            foreach (Subscribable conclusion in DataSource.Find(code, filter).ToList())
            {
                if (((StockConclusion)conclusion).MarketTimeType == MarketTimeTypes.Normal)
                {
                    conclusions.Add(conclusion);
                }
            }
            
            return conclusions;
        }

    }
}
