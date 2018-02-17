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
    public class DaishinStockCur : DibBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<string, DaishinStockCur> _subscribeObjDic = new ConcurrentDictionary<string, DaishinStockCur>();

        protected override IDib Dib { get; set; }

        public event Action<StockConclusion> Received;

        public DaishinStockCur()
        {
            var c = new StockCurClass();
            c.Received += OnReceived;
            Dib = c;
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
                string fullCode = Dib.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 13 - (long) 현재가
                conclusion.Price = Convert.ToSingle(Dib.GetHeaderValue(13));
                if (conclusion.Price <= 0)
                    _logger.Error($"Stock conclusion price error, {conclusion.Price}");

                // 14 - (char)체결 상태
                char conclusionType = Convert.ToChar(Dib.GetHeaderValue(14));
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

                // 17 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(Dib.GetHeaderValue(17));

                // 18 - (long) 시간 (초)
                long time = Convert.ToInt64(Dib.GetHeaderValue(18));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100); // Daishin doesn't provide milisecond 

                // 20 - (char) 장 구분 플래그
                char marketTimeType = Convert.ToChar(Dib.GetHeaderValue(20));
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
                        _logger.Error($"Stock conclusion market time type error, {marketTimeType}");
                        break;
                }

                // 19 - (char) 예상 체결가 구분 플래그
                if (conclusion.MarketTimeType == MarketTimeTypes.Normal)
                {
                    char expected = Convert.ToChar(Dib.GetHeaderValue(19));
                    if (expected == '1')
                    {
                        conclusion.MarketTimeType = MarketTimeTypes.NormalExpect;
                        _logger.Trace($"Received expected price for {conclusion.Code}, price:{conclusion.Amount}");
                    }
                }

                Received?.Invoke(conclusion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static DaishinStockCur GetSubscribeObject(string code)
        {
            if (_subscribeObjDic.TryGetValue(code, out var obj) == false)
            {
                obj = new DaishinStockCur();
                _subscribeObjDic.TryAdd(code, obj);
            }

            return obj;
        }
    }
}
