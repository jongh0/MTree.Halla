﻿using MongoDB.Bson;
using MTree.Configuration;
using MTree.DataStructure;
using System;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
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
                    logger.Trace($"Subscribe StockConclusion success, Code: {code}");
                    StockSubscribeCount += 2;
                }
                else
                {
                    logger.Error($"Subscribe StockConclusion fail, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
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
                    logger.Trace($"Unsubscribe StockConclusion success, Code: {code}");
                    StockSubscribeCount -= 2;
                }
                else
                {
                    logger.Error($"Unsubscribe StockConclusion fail, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        private void stockOutCurObj_Received()
        {
            try
            {
                var now = DateTime.Now;

                var conclusion = new StockConclusion();
                conclusion.Id = ObjectId.GenerateNewId();
                conclusion.Timestamp = now.Ticks;

                // 0 - (string) 종목 코드
                string fullCode = stockOutCurObj.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 1 - (long) 시각
                long time = Convert.ToInt64(stockOutCurObj.GetHeaderValue(1));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), 0); // Daishin doesn't provide second & milisecond 

                // 5 - (long) 현재가
                conclusion.Price = Convert.ToSingle(stockOutCurObj.GetHeaderValue(5));
                if (conclusion.Price <= 0)
                    logger.Error($"Stock conclusion price error, {conclusion.Price}");

                // 9 - (char)  체결매수매도플래그
                char conclusionType = Convert.ToChar(stockOutCurObj.GetHeaderValue(9));
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
                        logger.Error($"Stock conclusion type error, {conclusionType}");
                        break;
                }

                // 10 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(stockOutCurObj.GetHeaderValue(17));

                // 11 - (char) 장전시간외플래그 ('3'으로 나옴)
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
                conclusion.Timestamp = now.Ticks;

                // 0 - (string) 종목 코드
                string fullCode = stockCurObj.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 13 - (long) 현재가
                conclusion.Price = Convert.ToSingle(stockCurObj.GetHeaderValue(13));
                if (conclusion.Price <= 0)
                    logger.Error($"Stock conclusion price error, {conclusion.Price}");

                // 14 - (char)체결 상태
                char conclusionType = Convert.ToChar(stockCurObj.GetHeaderValue(14));
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
                        logger.Error($"Stock conclusion type error, {conclusionType}");
                        break;
                }

                // 17 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(stockCurObj.GetHeaderValue(17));

                // 18 - (long) 시간 (초)
                long time = Convert.ToInt64(stockCurObj.GetHeaderValue(18));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100); // Daishin doesn't provide milisecond 

                // 20 - (char) 장 구분 플래그
                char marketTimeType = Convert.ToChar(stockCurObj.GetHeaderValue(20));
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
                        logger.Error($"Stock conclusion market time type error, {marketTimeType}");
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
