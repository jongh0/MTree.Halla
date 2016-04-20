using MongoDB.Bson;
using MTree.Configuration;
using MTree.DataStructure;
using System;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        private long stockPrevTime = 0;
        private int stockMillisecond = 0;
        private long stockOutPrevTime = 0;
        private int stockOutMillisecond = 0;

        private int _StockSubscribeCount = 0;
        public int StockSubscribeCount
        {
            get { return _StockSubscribeCount; }
            set
            {
                _StockSubscribeCount = value;
                NotifyPropertyChanged(nameof(StockSubscribeCount));
            }
        }

        public override bool SubscribeStock(string code)
        {
            if (GetSubscribableCount() < 2)
            {
                logger.Error("Not enough subscribable count");
                return false;
            }

            short status = 1;

            try
            {
                stockOutCurObj.SetInputValue(0, code);
                stockOutCurObj.Subscribe();

                while (true)
                {
                    status = stockOutCurObj.GetDibStatus();
                    if (status != 1) // 1 - 수신대기
                        break;

                    Thread.Sleep(10);
                }

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
                {
                    logger.Info($"Subscribe stock, Code: {code}");
                    StockSubscribeCount += 2;
                }
                else
                {
                    logger.Error($"Subscribe stock error, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        public override bool UnsubscribeStock(string code)
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

                stockOutCurObj.SetInputValue(0, code);
                stockOutCurObj.Unsubscribe();

                while (true)
                {
                    status = stockOutCurObj.GetDibStatus();
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
                {
                    logger.Trace($"Unsubscribe stock, Code: {code}");
                    StockSubscribeCount -= 2;
                }
                else
                {
                    logger.Error($"Unsubscribe stock error, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        private void stockOutCurObj_Received()
        {
            try
            {
#if false
                logger.Info($"Code: {stockOutCurObj.GetHeaderValue(0).ToString()}");
                logger.Info($"Time: {stockOutCurObj.GetHeaderValue(1).ToString()}");
                logger.Info($"Current: {stockOutCurObj.GetHeaderValue(5).ToString()}");
                logger.Info($"Concluded: {stockOutCurObj.GetHeaderValue(6).ToString()}");
                logger.Info($"Accum Amount: {stockOutCurObj.GetHeaderValue(7).ToString()}");
                logger.Info($"Sell/Buy: {stockOutCurObj.GetHeaderValue(9).ToString()}");
                logger.Info($"Amount: {stockOutCurObj.GetHeaderValue(10).ToString()}");
                logger.Info($"Flag: {stockOutCurObj.GetHeaderValue(11).ToString()}");
                logger.Info($"Capital: {stockOutCurObj.GetHeaderValue(12).ToString()}");
#endif
                var now = DateTime.Now;
                var conclusion = new StockConclusion();
                conclusion.Id = ObjectId.GenerateNewId();

                // 0 - (string) 종목 코드
                string fullCode = stockOutCurObj.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 1 - (long) 시간 (초)
                if (Config.General.VerifyLatency == true)
                {
                    conclusion.Time = now;
                }
                else
                {
                    long time = Convert.ToInt64(stockOutCurObj.GetHeaderValue(1));
                    conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100); // Daishin doesn't provide milisecond 
                }

                // 5 - (long) 현재가
                conclusion.Price = Convert.ToSingle(stockOutCurObj.GetHeaderValue(5));
                if (conclusion.Price <= 0)
                    logger.Error($"Stock conclusion price error, {conclusion.Price}/{stockOutCurObj.GetHeaderValue(5)}");

                // 9 - (char)체결 상태
                char type = Convert.ToChar(stockOutCurObj.GetHeaderValue(9));
                if (type == '1')
                    conclusion.ConclusionType = ConclusionTypes.Buy;
                else
                    conclusion.ConclusionType = ConclusionTypes.Sell;

                // 10 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(stockOutCurObj.GetHeaderValue(17));
                
                // 장 구분 플래그
                conclusion.MarketTimeType = MarketTimeTypes.BeforeOffTheClock;

                StockConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void stockCurObj_Received()
        {
            try
            {
                var now = DateTime.Now;
                var conclusion = new StockConclusion();
                conclusion.Id = ObjectId.GenerateNewId();

                // 0 - (string) 종목 코드
                string fullCode = stockCurObj.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 18 - (long) 시간 (초)
                long time = Convert.ToInt64(stockCurObj.GetHeaderValue(18));
                if (stockPrevTime != time)
                {
                    stockPrevTime = time;
                    stockMillisecond = 0;
                }
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, stockMillisecond++); // Daishin doesn't provide milisecond 

                // 13 - (long) 현재가
                conclusion.Price = Convert.ToSingle(stockCurObj.GetHeaderValue(13));
                if (conclusion.Price <= 0)
                    logger.Error($"Stock conclusion price error, {conclusion.Price}/{stockCurObj.GetHeaderValue(13)}");

                // 14 - (char)체결 상태
                char type = Convert.ToChar(stockCurObj.GetHeaderValue(14));
                if (type == '1') conclusion.ConclusionType = ConclusionTypes.Buy;
                else conclusion.ConclusionType = ConclusionTypes.Sell;

                // 17 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(stockCurObj.GetHeaderValue(17));
                
                // 20 - (char) 장 구분 플래그
                char marketTime = Convert.ToChar(stockCurObj.GetHeaderValue(20));
                switch (marketTime)
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
                        logger.Error($"Stock conclusion market time type error, {marketTime}");
                        break;
                }

                // 19 - (char) 예상 체결가 구분 플래그
                if (conclusion.MarketTimeType == MarketTimeTypes.Normal)
                {
                    char expected = Convert.ToChar(stockCurObj.GetHeaderValue(19));
                    if (expected == '1')
                    {
                        conclusion.MarketTimeType = MarketTimeTypes.NormalExpect;
                        logger.Trace($"Received expected price for {conclusion.Code}, price:{conclusion.Amount}");
                    }
                }

                StockConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
