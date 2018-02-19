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
    public class DaishinStockOutCur : IDaishinSubscribe
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<string, DaishinStockOutCur> _subscribeObjDic = new ConcurrentDictionary<string, DaishinStockOutCur>();

        public event Action<StockConclusion> Received;

        private string _code;
        private StockOutCurClass _dib;

        public DaishinStockOutCur()
        {
            _dib = new StockOutCurClass();
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

                var conclusion = new StockConclusion();
                conclusion.Id = ObjectIdUtility.GenerateNewId(now);
                conclusion.ReceivedTime = now;

                // 0 - (string) 종목 코드
                string fullCode = _dib.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 1 - (long) 시각
                long time = Convert.ToInt64(_dib.GetHeaderValue(1));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), 0); // Daishin doesn't provide second & milisecond 

                // 5 - (long) 현재가
                conclusion.Price = Convert.ToSingle(_dib.GetHeaderValue(5));
                if (conclusion.Price <= 0)
                    _logger.Error($"Stock conclusion price error, {conclusion.Price}");

                // 9 - (char)  체결매수매도플래그
                char conclusionType = Convert.ToChar(_dib.GetHeaderValue(9));
                switch (conclusionType)
                {
                    case '1':
                        conclusion.ConclusionType = ConclusionTypes.Buy;
                        break;

                    case '2':
                        conclusion.ConclusionType = ConclusionTypes.Sell;
                        break;

                    default:
                        conclusion.ConclusionType = ConclusionTypes.Unknown;
                        _logger.Error($"Stock conclusion type error, {conclusionType}");
                        break;
                }

                // 10 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(_dib.GetHeaderValue(17));

                // 11 - (char) 장전시간외플래그 ('3'으로 나옴)
                conclusion.MarketTimeType = MarketTimeTypes.BeforeOffTheClock;

                Received?.Invoke(conclusion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static DaishinStockOutCur GetSubscribeObject(string code)
        {
            if (_subscribeObjDic.TryGetValue(code, out var obj) == false)
            {
                obj = new DaishinStockOutCur();
                _subscribeObjDic.TryAdd(code, obj);
            }

            return obj;
        }
    }
}
