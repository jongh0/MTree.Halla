using CommonLib.Utility;
using DataStructure;
using DSCBO1Lib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FirmLib.Daishin
{
    public class DaishinStockBid : DibBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<string, DaishinStockBid> _subscribeObjDic = new ConcurrentDictionary<string, DaishinStockBid>();

        private readonly int[] _biddingIndexes = { 3, 7, 11, 15, 19, 27, 31, 35, 39, 43 };

        public event Action<BiddingPrice> Received;

        public DaishinStockBid()
        {
            var c = new StockJpbidClass();
            c.Received += OnReceived;
            Dib = c;
        }
        
        private void OnReceived()
        {
            try
            {
                var now = DateTime.Now;

                var biddingPrice = new BiddingPrice();
                biddingPrice.Id = ObjectIdUtility.GenerateNewId(now);
                biddingPrice.Time = now;
                biddingPrice.ReceivedTime = now;

                biddingPrice.Bids = new List<BiddingPriceEntity>();
                biddingPrice.Offers = new List<BiddingPriceEntity>();

                string fullCode = Convert.ToString(Dib.GetHeaderValue(0));
                biddingPrice.Code = CodeEntity.RemovePrefix(fullCode);

                for (int i = 0; i < _biddingIndexes.Length; i++)
                {
                    int index = _biddingIndexes[i];

                    biddingPrice.Offers.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(Dib.GetHeaderValue(index)),
                        Convert.ToInt64(Dib.GetHeaderValue(index + 2))
                        ));

                    biddingPrice.Bids.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(Dib.GetHeaderValue(index + 1)),
                        Convert.ToInt64(Dib.GetHeaderValue(index + 3))
                        ));
                }

                Received?.Invoke(biddingPrice);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public static DaishinStockBid GetSubscribeObject(string code)
        {
            if (_subscribeObjDic.TryGetValue(code, out var obj) == false)
            {
                obj = new DaishinStockBid();
                _subscribeObjDic.TryAdd(code, obj);
            }

            return obj;
        }
    }
}
