using MTree.DataStructure;
using System;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        public override bool SubscribeIndex(string code)
        {
            return false;
        }

        public override bool UnsubscribeIndex(string code)
        {
            return false;
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
                
                // 17 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(stockCurObj.GetHeaderValue(17));

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
