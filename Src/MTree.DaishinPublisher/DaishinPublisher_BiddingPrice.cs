using MongoDB.Bson;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        int[] biddingIndexes = { 3, 7, 11, 15, 19, 27, 31, 35, 39, 43 };

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
            if (GetSubscribableCount() < 1)
            {
                logger.Error("Not enough subscribable count");
                return false;
            }

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
                    logger.Trace($"Subscribe BiddingPrice success, Code: {code}");
                    BiddingSubscribeCount++;
                }
                else
                {
                    logger.Error($"Subscribe BiddingPrice fail, Code: {code}, Status: {status}, Msg: {stockJpbidObj.GetDibMsg1()}");
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
                    logger.Trace($"Unsubscribe BiddingPrice success, Code: {code}");
                    BiddingSubscribeCount--;
                }
                else
                {
                    logger.Error($"Unsubscribe BiddingPrice fail, Code: {code}, Status: {status}, Msg: {stockJpbidObj.GetDibMsg1()}");
                }
            }

            return (status == 0);
        }

        private void stockJpbidObj_Received()
        {
            try
            {
                var biddingPrice = new BiddingPrice();
                biddingPrice.Id = ObjectId.GenerateNewId();
                biddingPrice.Time = biddingPrice.ReceivedTime = DateTime.Now;

                biddingPrice.Bids = new List<BiddingPriceEntity>();
                biddingPrice.Offers = new List<BiddingPriceEntity>();

                string fullCode = Convert.ToString(stockJpbidObj.GetHeaderValue(0));
                biddingPrice.Code = CodeEntity.RemovePrefix(fullCode);

                for (int i = 0; i < biddingIndexes.Length; i++)
                {
                    int index = biddingIndexes[i];

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
