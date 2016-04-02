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
    public class DataCollector
    {
        private DbAgent DataSource { get; set; } = DbAgent.Instance;

        public DataCollector(DbAgent dataSource = null)
        {
            if (dataSource != null)
            {
                DataSource = dataSource;
            }
        }

        public List<string> GetStockCodeList()
        {
            return DataSource.GetCollectionList(DbTypes.StockMaster);
        }

        public StockMaster GetMaster(string code, DateTime targetDate)
        {
            var builder = Builders<StockMaster>.Filter;
            var filter = builder.Eq(i => i.Time, targetDate);
            return DataSource.Find(code, filter).FirstOrDefault();
        }
    }
}
