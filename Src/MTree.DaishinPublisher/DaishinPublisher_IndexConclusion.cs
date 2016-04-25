﻿using MongoDB.Bson;
using MTree.Configuration;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        private int _IndexSubscribeCount = 0;
        public int IndexSubscribeCount
        {
            get { return _IndexSubscribeCount; }
            set
            {
                _IndexSubscribeCount = value;
                NotifyPropertyChanged(nameof(IndexSubscribeCount));
            }
        }

        private Dictionary<string, long> PrevIndexVolume { get; set; } = new Dictionary<string, long>();
        private Dictionary<string, long> PrevIndexMarketCapitalization { get; set; } = new Dictionary<string, long>();

        public override bool SubscribeIndex(string code)
        {
            if (GetSubscribableCount() < 1)
            {
                logger.Error("Not enough subscribable count");
                return false;
            }

            short status = 1;

            try
            {
                indexCurObj.SetInputValue(0, code);
                indexCurObj.Subscribe();

                while (true)
                {
                    status = indexCurObj.GetDibStatus();
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
                    logger.Trace($"Subscribe IndexConclusion success, Code: {code}");
                    IndexSubscribeCount++;
                }
                else
                {
                    logger.Error($"Subscribe IndexConclusion fail, Code: {code}, Status: {status}, Msg: {indexCurObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        public override bool UnsubscribeIndex(string code)
        {
            short status = 1;

            try
            {
                indexCurObj.SetInputValue(0, code);
                indexCurObj.Unsubscribe();

                while (true)
                {
                    status = indexCurObj.GetDibStatus();
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
                    logger.Trace($"Unsubscribe IndexConclusion success, Code: {code}");
                    IndexSubscribeCount--;
                }
                else
                {
                    logger.Error($"Unsubscribe IndexConclusion fail, Code: {code}, Status: {status}, Msg: {indexCurObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        private void indexCurObj_Received()
        {
            try
            {
                var now = DateTime.Now;

                var conclusion = new IndexConclusion();
                conclusion.Id = ObjectId.GenerateNewId();
                conclusion.ReceivedTime = now;

                // 0 - (string) 종목 코드
                string fullCode = indexCurObj.GetHeaderValue(0).ToString();
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

                // 9 - (long) 누적거래량
                conclusion.Amount = Convert.ToInt64(indexCurObj.GetHeaderValue(9));
                if (PrevIndexVolume.ContainsKey(conclusion.Code))
                {
                    long newReceived = conclusion.Amount;
                    conclusion.Amount = conclusion.Amount - PrevIndexVolume[conclusion.Code];
                    PrevIndexVolume[conclusion.Code] = newReceived;
                }
                else
                {
                    PrevIndexVolume.Add(conclusion.Code, conclusion.Amount);
                }

                // 10 - (long) 누적거래대금
                conclusion.MarketCapitalization = Convert.ToInt64(indexCurObj.GetHeaderValue(10));
                if (PrevIndexMarketCapitalization.ContainsKey(conclusion.Code))
                {
                    long newReceived = conclusion.MarketCapitalization;
                    conclusion.MarketCapitalization = conclusion.MarketCapitalization - PrevIndexMarketCapitalization[conclusion.Code];
                    PrevIndexMarketCapitalization[conclusion.Code] = newReceived;
                }
                else
                {
                    PrevIndexMarketCapitalization.Add(conclusion.Code, conclusion.MarketCapitalization);
                }

                // 13 - (long) 현재가
                conclusion.Price = Convert.ToSingle(indexCurObj.GetHeaderValue(13)) / 100;
                if (conclusion.Price <= 0)
                    logger.Error($"Index conclusion price error, Price: {conclusion.Price}");

                // 18 - (long) 시간 (초)
                long time = Convert.ToInt64(indexCurObj.GetHeaderValue(18));
                try
                {
                    conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100); // Daishin doesn't provide milisecond 
                }
                catch (ArgumentOutOfRangeException)
                {
                    conclusion.Time = now;
                    logger.Warn($"Index conclusion time error, time: {time}, code: {conclusion.Code}");
                }

                // 20 - (char) 장 구분 플래그
                char marketTimeType = Convert.ToChar(indexCurObj.GetHeaderValue(20));
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
                        logger.Error($"Index conclusion market time type error, {marketTimeType}");
                        break;
                }

                IndexConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
