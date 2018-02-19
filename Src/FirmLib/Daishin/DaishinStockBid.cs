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
    public class DaishinStockBid : IDaishinSubscribe
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<string, DaishinStockBid> _subscribeObjDic = new ConcurrentDictionary<string, DaishinStockBid>();

        private readonly int[] _biddingIndexes = { 3, 7, 11, 15, 19, 27, 31, 35, 39, 43 };

        public event Action<BiddingPrice> Received;

        private string _code;
        private StockJpbidClass _dib;

        public DaishinStockBid()
        {
            _dib = new StockJpbidClass();
            _dib.Received += OnReceived;
        }

        public bool Subscribe(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code) == true)
                    return false;

                _code = code;

                _dib.SetInputValue(0, code);
                _dib.Subscribe();

                return WaitResponse();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool Unsubscribe()
        {
            try
            {
                if (string.IsNullOrEmpty(_code) == true)
                    return false;

                _dib.SetInputValue(0, _code);
                _dib.Unsubscribe();

                return WaitResponse();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool WaitResponse()
        {
            int timeout = 5000;

            while (timeout > 0)
            {
                if (_dib.GetDibStatus() != 1) // 1 - 수신대기
                    return true;

                DispatcherUtility.DoEvents(); // 혹시 모르니 Message Pumping

                Thread.Sleep(10);
                timeout -= 10;
            }

            _logger.Error($"Dib response timeout");
            return false;
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

                string fullCode = Convert.ToString(_dib.GetHeaderValue(0));
                biddingPrice.Code = CodeEntity.RemovePrefix(fullCode);

                for (int i = 0; i < _biddingIndexes.Length; i++)
                {
                    int index = _biddingIndexes[i];

                    biddingPrice.Offers.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(_dib.GetHeaderValue(index)),
                        Convert.ToInt64(_dib.GetHeaderValue(index + 2))
                        ));

                    biddingPrice.Bids.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(_dib.GetHeaderValue(index + 1)),
                        Convert.ToInt64(_dib.GetHeaderValue(index + 3))
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
