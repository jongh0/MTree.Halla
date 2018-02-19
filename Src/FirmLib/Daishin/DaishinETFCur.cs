using CommonLib.Utility;
using CPSYSDIBLib;
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
    public class DaishinETFCur : IDaishinSubscribe
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<string, DaishinETFCur> _subscribeObjDic = new ConcurrentDictionary<string, DaishinETFCur>();

        public event Action<ETFConclusion> Received;

        private string _code;
        private CpSvrNew7244SClass _dib;

        public DaishinETFCur()
        {
            _dib = new CpSvrNew7244SClass();
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

                var conclusion = new ETFConclusion();
                conclusion.Id = ObjectIdUtility.GenerateNewId(now);
                conclusion.ReceivedTime = now;

                // 0 - (string) 종목코드
                string fullCode = _dib.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 1 - (long) 시각
                long time = Convert.ToInt64(_dib.GetHeaderValue(1));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), 0); // Daishin doesn't provide second & milisecond 

                // 2 - (long) 현재가
                conclusion.Price = Convert.ToSingle(_dib.GetHeaderValue(2));
                if (conclusion.Price <= 0)
                    _logger.Error($"ETF conclusion price error, {conclusion.Price}");

                // 3 - (char)대비부호
                var sign = Convert.ToChar(_dib.GetHeaderValue(3));

                // 4 - (long) 대비
                conclusion.Comparision = Convert.ToDouble(_dib.GetHeaderValue(4));
                if (sign == '-' || sign == '5') // Sign값이 '-'가 아닌 '5'로 들어옴. 대신 API Bug로 추정.
                    conclusion.Comparision *= -1;

                // 5 - (long) 거래량
                conclusion.Amount = Convert.ToInt64(_dib.GetHeaderValue(5));

                // 6 - (long) NAV 지수
                conclusion.NAVIndex = Convert.ToInt64(_dib.GetHeaderValue(6)) / 100;

                // 7 - (char) NAV대비부호
                sign = Convert.ToChar(_dib.GetHeaderValue(7));

                // 8 - (long) NAV 대비
                conclusion.NAVComparision = Convert.ToDouble(_dib.GetHeaderValue(8)) / 100;
                if (sign == '-')
                    conclusion.NAVComparision *= -1;

                // 9 - (char) 추적오차율부호
                sign = Convert.ToChar(_dib.GetHeaderValue(9));

                // 10 - (long)추적오차율
                conclusion.TracingErrorRate = Convert.ToDouble(_dib.GetHeaderValue(10)) / 100;
                if (sign == '-')
                    conclusion.TracingErrorRate *= -1;

                // 11 - (char)괴리율부호
                sign = Convert.ToChar(_dib.GetHeaderValue(11));

                // 12 - (long)괴리율
                conclusion.DisparateRatio = Convert.ToDouble(_dib.GetHeaderValue(12)) / 100;
                if (sign == '-')
                    conclusion.DisparateRatio *= -1;

                // 13 - (char)해당 ETF 지수 대비 부호
                sign = Convert.ToChar(_dib.GetHeaderValue(13));

                // 14 - (long)해당 ETF 지수 대비
                conclusion.ReferenceIndexComparision = Convert.ToDouble(_dib.GetHeaderValue(14)) / 100;
                if (sign == '-')
                    conclusion.ReferenceIndexComparision *= -1;

                // 15 - (long)해당 ETF 지수
                conclusion.ReferenceIndex = Convert.ToSingle(_dib.GetHeaderValue(15)) / 100;

                Received?.Invoke(conclusion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static DaishinETFCur GetSubscribeObject(string code)
        {
            if (_subscribeObjDic.TryGetValue(code, out var obj) == false)
            {
                obj = new DaishinETFCur();
                _subscribeObjDic.TryAdd(code, obj);
            }

            return obj;
        }
    }
}
