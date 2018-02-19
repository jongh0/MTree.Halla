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
    public class DaishinIndexCur : IDaishinSubscribe
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<string, DaishinIndexCur> _subscribeObjDic = new ConcurrentDictionary<string, DaishinIndexCur>();

        private static ConcurrentDictionary<string, long> _prevIndexVolume = new ConcurrentDictionary<string, long>();
        private static ConcurrentDictionary<string, long> _prevIndexMarketCapitalization = new ConcurrentDictionary<string, long>();

        public event Action<IndexConclusion> Received;

        private string _code;
        private StockCurClass _dib;

        public DaishinIndexCur()
        {
            _dib = new StockCurClass();
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

                var conclusion = new IndexConclusion();
                conclusion.Id = ObjectIdUtility.GenerateNewId(now);
                conclusion.ReceivedTime = now;

                // 0 - (string) 종목 코드
                string fullCode = _dib.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 9 - (long) 누적거래량
                conclusion.Amount = Convert.ToInt64(_dib.GetHeaderValue(9));
                if (_prevIndexVolume.ContainsKey(conclusion.Code))
                {
                    long newReceived = conclusion.Amount;
                    conclusion.Amount = conclusion.Amount - _prevIndexVolume[conclusion.Code];
                    _prevIndexVolume[conclusion.Code] = newReceived;
                }
                else
                {
                    _prevIndexVolume.TryAdd(conclusion.Code, conclusion.Amount);
                }

                // 10 - (long) 누적거래대금
                conclusion.MarketCapitalization = Convert.ToInt64(_dib.GetHeaderValue(10));
                if (_prevIndexMarketCapitalization.ContainsKey(conclusion.Code))
                {
                    long newReceived = conclusion.MarketCapitalization;
                    conclusion.MarketCapitalization = conclusion.MarketCapitalization - _prevIndexMarketCapitalization[conclusion.Code];
                    _prevIndexMarketCapitalization[conclusion.Code] = newReceived;
                }
                else
                {
                    _prevIndexMarketCapitalization.TryAdd(conclusion.Code, conclusion.MarketCapitalization);
                }

                // 13 - (long) 현재가
                conclusion.Price = Convert.ToSingle(_dib.GetHeaderValue(13)) / 100;
                if (conclusion.Price <= 0)
                    _logger.Error($"Index conclusion price error, Price: {conclusion.Price}");

                // 18 - (long) 시간 (초)
                long time = Convert.ToInt64(_dib.GetHeaderValue(18));
                try
                {
                    if (time == 240000)
                    {
                        conclusion.Time = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59); // End of day
                    }
                    else
                    {
                        conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100); // Daishin doesn't provide milisecond 
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    conclusion.Time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0); // ignore second
                    _logger.Warn($"Index conclusion time error, time: {time}, code: {conclusion.Code}");
                }

                // 20 - (char) 장 구분 플래그
                char marketTimeType = Convert.ToChar(_dib.GetHeaderValue(20));
                switch (marketTimeType)
                {
                    case '1':
                        conclusion.MarketTimeType = MarketTimeTypes.BeforeExpect;
                        break;

                    case '2':
                        conclusion.MarketTimeType = MarketTimeTypes.Normal;
                        break;

                    case '3':
                        conclusion.MarketTimeType = MarketTimeTypes.BeforeOffTheClock;
                        break;

                    case '4':
                        conclusion.MarketTimeType = MarketTimeTypes.AfterOffTheClock;
                        break;

                    case '5':
                        conclusion.MarketTimeType = MarketTimeTypes.AfterExpect;
                        break;

                    default:
                        conclusion.MarketTimeType = MarketTimeTypes.Unknown;
                        _logger.Error($"Index conclusion market time type error, {marketTimeType}");
                        break;
                }

                Received?.Invoke(conclusion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static DaishinIndexCur GetSubscribeObject(string code)
        {
            if (_subscribeObjDic.TryGetValue(code, out var obj) == false)
            {
                obj = new DaishinIndexCur();
                _subscribeObjDic.TryAdd(code, obj);
            }

            return obj;
        }
    }
}
