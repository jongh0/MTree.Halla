using System;
using MTree.DataStructure;

namespace MTree.EbestPublisher
{
    public partial class EbestPublisher
    {
        public override bool SubscribeIndex(string code)
        {
            if (WaitLogin() == false)
            {
                logger.Error("Not login state");
                return false;
            }

            try
            {
                indexSubscribingObj.SetFieldData("InBlock", "upcode", code);
                indexSubscribingObj.AdviseRealData();

                subscribeCount++;
                logger.Info($"Subscribe index success, Code: {code}, subscribeCount: {subscribeCount}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Subscribe index fail, Code: {code}");
            return false;
        }

        public override bool UnsubscribeIndex(string code)
        {
            if (WaitLogin() == false)
            {
                logger.Error("Not login state");
                return false;
            }

            try
            {
                indexSubscribingObj.SetFieldData("InBlock", "upcode", code);
                indexSubscribingObj.UnadviseRealData();

                subscribeCount--;
                logger.Info($"Unsubscribe index success, Code: {code}, subscribeCount: {subscribeCount}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Unsubscribe index fail, Code: {code}");
            return false;
        }

        private void IndexConclusionReceived(string szTrCode)
        {
            try
            {
                var now = DateTime.Now;
                IndexConclusion conclusion = new IndexConclusion();

                string temp = indexSubscribingObj.GetFieldData("OutBlock", "upcode");
                conclusion.Code = temp;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "time");
                uint time;
                if (uint.TryParse(temp, out time) == true)
                    conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, now.Millisecond);
                else
                    conclusion.Time = now;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "jisu");
                double index = 0;
                if (double.TryParse(temp, out index) == false)
                    logger.Error($"Index conclusion index error, {temp}");
                conclusion.Price = (float)index;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "volume");
                long volume = 0;
                if (long.TryParse(temp, out volume) == false)
                    logger.Error($"Index conclusion index error, {temp}");
                conclusion.Amount = volume * 1000;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "value");
                long value;
                if (long.TryParse(temp, out value) == false)
                    logger.Error($"Index conclusion value error, {temp}");
                conclusion.MarketCapitalization = value;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "upjo");
                int upperLimitCount;
                if (int.TryParse(temp, out upperLimitCount) == false)
                    logger.Error($"Index conclusion upper limit count error, {temp}");
                conclusion.UpperLimitedItemCount = upperLimitCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "highjo");
                int increasingCount;
                if (int.TryParse(temp, out increasingCount) == false)
                    logger.Error($"Index conclusion increasing count error, {temp}");
                conclusion.IncreasingItemCount = increasingCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "unchgjo");
                int steadyCount;
                if (int.TryParse(temp, out steadyCount) == false)
                    logger.Error($"Index conclusion steady count error, {temp}");
                conclusion.SteadyItemCount = steadyCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "lowjo");
                int decreasingCount;
                if (int.TryParse(temp, out decreasingCount) == false)
                    logger.Error($"Index conclusion decreasing count error, {temp}");
                conclusion.DecreasingItemCount = decreasingCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "downjo");
                int lowerLimitedCount;
                if (int.TryParse(temp, out lowerLimitedCount) == false)
                    logger.Error($"Index conclusion lower limited count error, {temp}");
                conclusion.LowerLimitedItemCount = lowerLimitedCount;

                if (PrevIndexConclusions.ContainsKey(conclusion.Code) == false)
                    PrevIndexConclusions.TryAdd(conclusion.Code, new IndexConclusion());

                if (PrevIndexConclusions[conclusion.Code].MarketCapitalization == conclusion.MarketCapitalization &&
                    PrevIndexConclusions[conclusion.Code].Amount == conclusion.Amount)
                    return;

                long newReceived;
                newReceived = conclusion.MarketCapitalization;
                conclusion.MarketCapitalization = conclusion.MarketCapitalization - PrevIndexConclusions[conclusion.Code].MarketCapitalization;
                PrevIndexConclusions[conclusion.Code].MarketCapitalization = newReceived;

                newReceived = conclusion.Amount;
                conclusion.Amount = conclusion.Amount - PrevIndexConclusions[conclusion.Code].Amount;
                PrevIndexConclusions[conclusion.Code].Amount = newReceived;

                IndexConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
