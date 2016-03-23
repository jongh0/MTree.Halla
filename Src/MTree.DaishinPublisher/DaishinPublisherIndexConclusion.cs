using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        private Dictionary<String, long> prevIndexVolume;
        private Dictionary<String, long> prevIndexMarketCapitalization;

        public override bool SubscribeIndex(string code)
        {
            if (prevIndexVolume == null)
            {
                prevIndexVolume = new Dictionary<string, long>();
            }
            if (prevIndexMarketCapitalization == null)
            {
                prevIndexMarketCapitalization = new Dictionary<string, long>();
            }

            short status = 1;

            try
            {
                stockCurObj.SetInputValue(0, code);
                stockCurObj.Subscribe();

                while (true)
                {
                    status = stockCurObj.GetDibStatus();
                    if (status != 1) // 1 - 수신대기
                        break;

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if (status == 0)
                    logger.Info($"Subscribe index, Code: {code}");
                else
                    logger.Error($"Subscribe index error, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
            }

            return (status == 0);
        }

        public override bool UnsubscribeIndex(string code)
        {
            short status = 1;

            try
            {
                stockCurObj.SetInputValue(0, code);
                stockCurObj.Unsubscribe();

                while (true)
                {
                    status = stockCurObj.GetDibStatus();
                    if (status != 1) // 1 - 수신대기
                        break;

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if (status == 0)
                    logger.Trace($"Unsubscribe index, Code: {code}");
                else
                    logger.Error($"Unsubscribe index error, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
            }

            return (status == 0);
        }

        private void IndexConclusionReceived()
        {
            try
            {
                var now = DateTime.Now;
                var conclusion = new IndexConclusion();

                // 0 - (string) 종목 코드
                string fullCode = stockCurObj.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 18 - (long) 시간 (초)
                long time = Convert.ToInt64(stockCurObj.GetHeaderValue(18));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, now.Millisecond); // Daishin doesn't provide milisecond 

                // 13 - (long) 현재가
                conclusion.Price = Convert.ToSingle(stockCurObj.GetHeaderValue(13)) / 100;
                if (conclusion.Price <= 0)
                    logger.Error($"Stock conclusion price error, {conclusion.Price}/{stockCurObj.GetHeaderValue(13)}");

                // 9 - (long) 누적거래량
                conclusion.Amount = Convert.ToInt64(stockCurObj.GetHeaderValue(9));
                
                long newReceived;
                newReceived = conclusion.Amount;
                conclusion.Amount = conclusion.Amount - prevIndexVolume[conclusion.Code];
                if (!prevIndexVolume.ContainsKey(conclusion.Code))
                    prevIndexVolume.Add(conclusion.Code, newReceived);
                else
                    prevIndexVolume[conclusion.Code] = newReceived;

                // 10 - (long) 누적거래대금
                conclusion.MarketCapitalization = Convert.ToInt64(stockCurObj.GetHeaderValue(10));
                
                newReceived = conclusion.MarketCapitalization;
                conclusion.MarketCapitalization = conclusion.MarketCapitalization - prevIndexMarketCapitalization[conclusion.Code];
                if (!prevIndexMarketCapitalization.ContainsKey(conclusion.Code))
                    prevIndexMarketCapitalization.Add(conclusion.Code, newReceived);
                else
                    prevIndexMarketCapitalization[conclusion.Code] = newReceived;
                
                // 20 - (char) 장 구분 플래그
                char marketTime = Convert.ToChar(stockCurObj.GetHeaderValue(20));
                if (marketTime == '1') conclusion.MarketTimeType = MarketTimeTypes.BeforeExpect;
                else if (marketTime == '2') conclusion.MarketTimeType = MarketTimeTypes.Normal;
                else if (marketTime == '3') conclusion.MarketTimeType = MarketTimeTypes.BeforeOffTheClock;
                else if (marketTime == '4') conclusion.MarketTimeType = MarketTimeTypes.AfterOffTheClock;
                else if (marketTime == '5') conclusion.MarketTimeType = MarketTimeTypes.AfterOffTheClock;
                else logger.Error($"Stock conclusion market time type error, {stockCurObj.GetHeaderValue(20)}");


                IndexConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
