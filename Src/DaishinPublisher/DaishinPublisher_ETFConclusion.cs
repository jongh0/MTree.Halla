using Configuration;
using DataStructure;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Utility;
using FirmLib.Daishin;

namespace DaishinPublisher
{
    public partial class DaishinPublisher_
    {
        private int _ETFSubscribeCount = 0;
        public int ETFSubscribeCount
        {
            get { return _ETFSubscribeCount; }
            set
            {
                _ETFSubscribeCount = value;
                NotifyPropertyChanged(nameof(ETFSubscribeCount));
            }
        }

        public override bool SubscribeETF(string code)
        {
            if (GetSubscribableCount() < 1)
            {
                _logger.Error("Not enough subscribable count");
                return false;
            }

#if SEPERATE_SUBSCRIBE_OBJECT
            var etfCur = DaishinETFCur.GetSubscribeObject(code);
            etfCur.Received += EtfCur_Received;
            var result = etfCur.Subscribe(code);
            if (result == true)
                ETFSubscribeCount += 1;

            return result;
#else
            short status = 1;

            try
            {
                etfCurObj.SetInputValue(0, code);
                etfCurObj.Subscribe();

                while (true)
                {
                    status = etfCurObj.GetDibStatus();
                    if (status != 1) // 1 - 수신대기
                        break;

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                if (status == 0)
                {
                    _logger.Trace($"Subscribe ETFConclusion success, Code: {code}");
                    ETFSubscribeCount++;
                }
                else
                {
                    _logger.Error($"Subscribe ETFConclusion fail, Code: {code}, Status: {status}, Msg: {etfCurObj.GetDibMsg1()}");
                }
            }

            return (status == 0); 
#endif
        }

        public override bool UnsubscribeETF(string code)
        {
#if SEPERATE_SUBSCRIBE_OBJECT
            var result = DaishinETFCur.GetSubscribeObject(code).Unsubscribe();
            if (result == true)
                ETFSubscribeCount -= 1;

            return result;
#else
            short status = 1;

            try
            {
                etfCurObj.SetInputValue(0, code);
                etfCurObj.Unsubscribe();

                while (true)
                {
                    status = etfCurObj.GetDibStatus();
                    if (status != 1) // 1 - 수신대기
                        break;

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                if (status == 0)
                {
                    _logger.Trace($"Unsubscribe ETFConclusion success, Code: {code}");
                    ETFSubscribeCount--;
                }
                else
                {
                    _logger.Error($"Unsubscribe ETFConclusion fail, Code: {code}, Status: {status}, Msg: {etfCurObj.GetDibMsg1()}");
                }
            }

            return (status == 0); 
#endif
        }

#if SEPERATE_SUBSCRIBE_OBJECT
        private void EtfCur_Received(ETFConclusion obj)
        {
            throw new NotImplementedException();
        }
#else
        private void etfCurObj_Received()
        {
            var startTick = Environment.TickCount;

            try
            {
                var now = DateTime.Now;

                var conclusion = new ETFConclusion();
                conclusion.Id = ObjectIdUtility.GenerateNewId(now);
                conclusion.ReceivedTime = now;

                // 0 - (string) 종목코드
                string fullCode = etfCurObj.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 1 - (long) 시각
                long time = Convert.ToInt64(etfCurObj.GetHeaderValue(1));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), 0); // Daishin doesn't provide second & milisecond 

                // 2 - (long) 현재가
                conclusion.Price = Convert.ToSingle(etfCurObj.GetHeaderValue(2));
                if (conclusion.Price <= 0)
                    _logger.Error($"ETF conclusion price error, {conclusion.Price}");

                // 3 - (char)대비부호
                var sign = Convert.ToChar(etfCurObj.GetHeaderValue(3));

                // 4 - (long) 대비
                conclusion.Comparision = Convert.ToDouble(etfCurObj.GetHeaderValue(4));
                if (sign == '-' || sign == '5') // Sign값이 '-'가 아닌 '5'로 들어옴. 대신 API Bug로 추정.
                    conclusion.Comparision *= -1;

                // 5 - (long) 거래량
                conclusion.Amount = Convert.ToInt64(etfCurObj.GetHeaderValue(5));

                // 6 - (long) NAV 지수
                conclusion.NAVIndex = Convert.ToInt64(etfCurObj.GetHeaderValue(6)) / 100;

                // 7 - (char) NAV대비부호
                sign = Convert.ToChar(etfCurObj.GetHeaderValue(7));

                // 8 - (long) NAV 대비
                conclusion.NAVComparision = Convert.ToDouble(etfCurObj.GetHeaderValue(8)) / 100;
                if (sign == '-')
                    conclusion.NAVComparision *= -1;

                // 9 - (char) 추적오차율부호
                sign = Convert.ToChar(etfCurObj.GetHeaderValue(9));

                // 10 - (long)추적오차율
                conclusion.TracingErrorRate = Convert.ToDouble(etfCurObj.GetHeaderValue(10)) / 100;
                if (sign == '-')
                    conclusion.TracingErrorRate *= -1;

                // 11 - (char)괴리율부호
                sign = Convert.ToChar(etfCurObj.GetHeaderValue(11));

                // 12 - (long)괴리율
                conclusion.DisparateRatio = Convert.ToDouble(etfCurObj.GetHeaderValue(12)) / 100;
                if (sign == '-')
                    conclusion.DisparateRatio *= -1;

                // 13 - (char)해당 ETF 지수 대비 부호
                sign = Convert.ToChar(etfCurObj.GetHeaderValue(13));

                // 14 - (long)해당 ETF 지수 대비
                conclusion.ReferenceIndexComparision = Convert.ToDouble(etfCurObj.GetHeaderValue(14)) / 100;
                if (sign == '-')
                    conclusion.ReferenceIndexComparision *= -1;

                // 15 - (long)해당 ETF 지수
                conclusion.ReferenceIndex = Convert.ToSingle(etfCurObj.GetHeaderValue(15)) / 100;
                
                ETFConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                if (Config.General.VerifyEnqueueLatency == true)
                {
                    var latency = Environment.TickCount - startTick;
                    if (latency > 10)
                        _logger.Error($"ETF conclusion latency error, {latency}");
                }
            }
        }
#endif
    }
}
