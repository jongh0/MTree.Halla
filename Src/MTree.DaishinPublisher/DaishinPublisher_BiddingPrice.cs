﻿using MongoDB.Bson;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        private int _BiddingSubscribeCount = 0;
        public int BiddingSubscribeCount
        {
            get { return _BiddingSubscribeCount; }
            set
            {
                _BiddingSubscribeCount = value;
                NotifyPropertyChanged(nameof(BiddingSubscribeCount));
            }
        }

        public override bool SubscribeBidding(string code)
        {
            short status = 1;

            try
            {
                stockJpbidObj.SetInputValue(0, code);
                stockJpbidObj.Subscribe();

                while (true)
                {
                    status = stockJpbidObj.GetDibStatus();
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
                    logger.Info($"Subscribe bidding, Code: {code}");
                    BiddingSubscribeCount++;
                }
                else
                {
                    logger.Error($"Subscribe bidding error, Code: {code}, Status: {status}, Msg: {stockJpbidObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        public override bool UnsubscribeBidding(string code)
        {
            short status = 1;

            try
            {
                stockJpbidObj.SetInputValue(0, code);
                stockJpbidObj.Unsubscribe();

                while (true)
                {
                    status = stockJpbidObj.GetDibStatus();
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
                    logger.Trace($"Unsubscribe bidding, Code: {code}");
                    BiddingSubscribeCount--;
                }
                else
                {
                    logger.Error($"Unsubscribe bidding error, Code: {code}, Status: {status}, Msg: {stockJpbidObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        private void BiddingPriceReceived()
        {
            try
            {
                var now = DateTime.Now;

                var biddingPrice = new BiddingPrice();
                biddingPrice.Id = ObjectId.GenerateNewId();
                biddingPrice.Bids = new List<BiddingPriceEntity>();
                biddingPrice.Offers = new List<BiddingPriceEntity>();

                string fullCode = Convert.ToString(stockJpbidObj.GetHeaderValue(0));
                biddingPrice.Code = CodeEntity.RemovePrefix(fullCode);

                long time = Convert.ToInt64(stockJpbidObj.GetHeaderValue(1));
                biddingPrice.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 100), (int)(time % 100), now.Second, now.Millisecond); // Daishin doesn't provide second 

                int[] indexes = { 3, 7, 11, 15, 19, 27, 31, 35, 39, 43 };

                for (int i = 0; i < indexes.Length; i++)
                {
                    int index = indexes[i];

                    biddingPrice.Offers.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(stockJpbidObj.GetHeaderValue(index)),
                        Convert.ToInt64(stockJpbidObj.GetHeaderValue(index + 2))
                        ));

                    biddingPrice.Bids.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(stockJpbidObj.GetHeaderValue(index + 1)),
                        Convert.ToInt64(stockJpbidObj.GetHeaderValue(index + 3))
                        ));
                }

                BiddingPriceQueue.Enqueue(biddingPrice);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}
