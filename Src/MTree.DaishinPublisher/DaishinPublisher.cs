using CPUTILLib;
using DSCBO1Lib;
using CPSYSDIBLib;
using MTree.DataStructure;
using MTree.Publisher;
using MTree.RealTimeProvider;
using MTree.Utility;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MTree.DaishinPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DaishinPublisher : BrokerageFirmBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected int LastQuoteTick { get; set; } = Environment.TickCount;

        #region Daishin Specific
        private CpCybos sessionObj;
        private StockMst stockMstObj;
        private StockCur stockCurObj;
        private StockJpbid biddingObj;

        private WorldCur worldCurObj;

        #endregion

        public DaishinPublisher() : base()
        {
            try
            {
                QuoteInterval = 15 * 1000 / 60; // 15초당 60개

                sessionObj = new CpCybos();
                sessionObj.OnDisconnect += sessionObj_OnDisconnect;
                
                stockMstObj = new StockMst();
                stockMstObj.Received += stockMstObj_Received;

                stockCurObj = new StockCur();
                stockCurObj.Received += stockCurObj_Received;

                biddingObj = new StockJpbid();
                biddingObj.Received += biddingObj_Received;

                worldCurObj = new WorldCur();
                worldCurObj.Received += WorldCurObj_Received;

                StartBiddingPriceQueueTask();
                StartStockConclusionQueueTask();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public bool SubscribeWorldStock(string code)
        {
            int status = 1;

            try
            {
                worldCurObj.SetInputValue(0, code);
                worldCurObj.Subscribe();

                while (true)
                {
                    status = worldCurObj.GetDibStatus();
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
                
            }

            return (status == 0);
        }
        private void WorldCurObj_Received()
        {
            string code = worldCurObj.GetHeaderValue(0).ToString();
            float price = Convert.ToSingle(worldCurObj.GetHeaderValue(1));
        }

        private void sessionObj_OnDisconnect()
        {
            logger.Info("Disconnected");
        }

        private void stockMstObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            StockMasterReceived();
        }

        private void stockCurObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            StockConclusionReceived();
        }

        private void biddingObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            BiddingPriceReceived();
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            short ret = -1;

            try
            {
                WaitQuoteInterval();

                logger.Info($"Start quoting, Code: {code}");
                QuotingStockMaster = stockMaster;

                stockMstObj.SetInputValue(0, code);
                ret = stockMstObj.BlockRequest();

                if (ret == 0)
                {
                    if (WaitQuoting() == true)
                    {
                        if (QuotingStockMaster.Code != string.Empty)
                        {
                            logger.Info($"Quoting done. Code: {code}");
                            return true;
                        }

                        logger.Error($"Quoting fail. Code: {code}");
                    }

                    logger.Error($"Quoting timeout. Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail. Code: {code}, result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(QuoteLock);
            }

            return false;
        }

        private void StockMasterReceived()
        {
            try
            {
                if (QuotingStockMaster == null)
                    return;

                // 0 - (string) 종목 코드
                string code = stockMstObj.GetHeaderValue(0).ToString().Substring(1);

                // 1 - (string) 종목 명
                QuotingStockMaster.Name = stockMstObj.GetHeaderValue(1).ToString();

                // 2 - (string) 대신 업종코드
                var daishingCode = stockMstObj.GetHeaderValue(2).ToString();

                // 3 - (string) 그룹코드
                var groupCode = stockMstObj.GetHeaderValue(3).ToString();

                // 5 - (string) 소속구분
                var classification = stockMstObj.GetHeaderValue(5).ToString();

                // 6 - (string) 대형,중형,소형
                var size = stockMstObj.GetHeaderValue(6).ToString();

                // 8 - (long) 상한가
                QuotingStockMaster.UpperLimit = (int)stockMstObj.GetHeaderValue(8);

                // 9- (long) 하한가
                QuotingStockMaster.LowerLimit = (int)stockMstObj.GetHeaderValue(9);

                // 10 - (long) 전일종가
                QuotingStockMaster.PreviousClosedPrice = (int)stockMstObj.GetHeaderValue(10);

                // 11 - (long) 현재가
                //currentQuotingkMaster.LastSale = (int)currentPriceQueryObj.GetHeaderValue(11);  

                // 26 - (short) 결산월     
                QuotingStockMaster.SettlementMonth = Convert.ToInt32(stockMstObj.GetHeaderValue(26));

                // 27 - (long) basis price (기준가)
                QuotingStockMaster.BasisPrice = (int)stockMstObj.GetHeaderValue(27);
                
                // 31 - (decimal) 상장주식수 (단주)
                QuotingStockMaster.ShareVolume = Convert.ToInt64(stockMstObj.GetHeaderValue(31));

                // 32 - (long) 상장자본금
                QuotingStockMaster.ListedCapital = Convert.ToInt64(stockMstObj.GetHeaderValue(32)) * 1000000; // TODO : 단위 안넘치나?

                // 37 - (long) 외국인 한도수량
                QuotingStockMaster.ForeigneLimit = Convert.ToInt64(stockMstObj.GetHeaderValue(37));

                // 39 - (decimal) 외국인 주문가능수량
                QuotingStockMaster.ForeigneAvailableRemain = Convert.ToInt64(stockMstObj.GetHeaderValue(39));
                
                // 43 - (short) 매매 수량 단위 
                QuotingStockMaster.QuantityUnit = (int)stockMstObj.GetHeaderValue(43);

                // 45 - (char) 소속 구분(코드)
                var classificationCode = Convert.ToChar(stockMstObj.GetHeaderValue(45));

                // 46 - (long) 전일 거래량
                QuotingStockMaster.PreviousVolume = Convert.ToInt64(stockMstObj.GetHeaderValue(46));

                // 52 - (string) 벤처 구분. [코스닥 : 일반기업 / 벤처기업] [프리보드 : 일반기업 / 벤처기업 / 테크노파크일반 / 테크노파크벤쳐]
                var venture = (string)stockMstObj.GetHeaderValue(52);

                // 53 - (short) KOSPI200 채용 여부
                var kospi200 = (string)stockMstObj.GetHeaderValue(53);

                // 54 - (short) 액면가
                QuotingStockMaster.FaceValue = (int)stockMstObj.GetHeaderValue(54);

                // 69 -(char) 불성실 공시구분 => KRX로 이동
                //if ((char)stockMstObj.GetHeaderValue(69) != '0')
                //    QuotingStockMaster.UnfairAnnouncement = new Warning() { Start = DateTime.Now };
            }
            catch (Exception ex)
            {
                QuotingStockMaster.Code = string.Empty;
                logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }

        public override bool SubscribeStock(string code)
        {
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
                    logger.Info($"Subscribe stock, Code: {code}");
                else
                    logger.Error($"Subscribe stock error, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
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
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if (status == 0)
                    logger.Trace($"Unsubscribe stock, Code: {code}");
                else
                    logger.Error($"Unsubscribe stock error, Code: {code}, Status: {status}, Msg: {stockCurObj.GetDibMsg1()}");
            }

            return (status == 0);
        }

        private void StockConclusionReceived()
        {
            try
            {
                var now = DateTime.Now;
                StockConclusion conclusion = new StockConclusion();

                // 0 - (string) 종목 코드
                string fullCode = stockCurObj.GetHeaderValue(0).ToString();
                if (fullCode.Length != 0)
                {
                    conclusion.Code = fullCode.Substring(1); // Remove prefix
                    conclusion.MarketType = CodeEntity.ConvertToMarketType(fullCode);
                }
                
                // 18 - (long) 시간 (초)
                long time = Convert.ToInt64(stockCurObj.GetHeaderValue(18));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, now.Millisecond); // Daishin doesn't provide milisecond 

                // 13 - (long) 현재가
                conclusion.Price = Convert.ToSingle(stockCurObj.GetHeaderValue(13));
                if (conclusion.Price <= 0)
                    logger.Error($"Stock conclusion price error, {conclusion.Price}/{stockCurObj.GetHeaderValue(13)}");

                // 14 - (char)체결 상태
                char type = Convert.ToChar(stockCurObj.GetHeaderValue(14));
                if (type == '1')    conclusion.ConclusionType = ConclusionTypes.Buy;
                else                conclusion.ConclusionType = ConclusionTypes.Sell;


                // 17 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(stockCurObj.GetHeaderValue(17));
                if (conclusion.Amount <= 0)
                    logger.Error($"Stock conclusion amount error, {conclusion.Amount}/{stockCurObj.GetHeaderValue(17)}");

                // 20 - (char) 장 구분 플래그
                char typeTime = Convert.ToChar(stockCurObj.GetHeaderValue(20));
                if (typeTime == '1')        conclusion.MarketTimeType = MarketTimeTypes.BeforeExpect;
                else if (typeTime == '2')   conclusion.MarketTimeType = MarketTimeTypes.Normal;
                else if (typeTime == '3')   conclusion.MarketTimeType = MarketTimeTypes.BeforeOffTheClock;
                else if (typeTime == '4')   conclusion.MarketTimeType = MarketTimeTypes.AfterOffTheClock;
                else if (typeTime == '5')   conclusion.MarketTimeType = MarketTimeTypes.AfterOffTheClock;
                else logger.Error($"Stock conclusion typeTime error, {stockCurObj.GetHeaderValue(20)}");

                StockConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override bool SubscribeBidding(string code)
        {
            short status = 1;

            try
            {
                biddingObj.SetInputValue(0, code);
                biddingObj.Subscribe();

                while (true)
                {
                    status = biddingObj.GetDibStatus();
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
                    logger.Info($"Subscribe bidding, Code: {code}");
                else
                    logger.Error($"Subscribe bidding error, Code: {code}, Status: {status}, Msg: {biddingObj.GetDibMsg1()}");
            }

            return (status == 0);
        }

        public override bool UnsubscribeBidding(string code)
        {
            short status = 1;

            try
            {
                biddingObj.SetInputValue(0, code);
                biddingObj.Unsubscribe();

                while (true)
                {
                    status = biddingObj.GetDibStatus();
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
                    logger.Trace($"Unsubscribe bidding, Code: {code}");
                else
                    logger.Error($"Unsubscribe bidding error, Code: {code}, Status: {status}, Msg: {biddingObj.GetDibMsg1()}");
            }

            return (status == 0);
        }

        private void BiddingPriceReceived()
        {
            try
            {
                DateTime now = DateTime.Now;

                BiddingPrice biddingPrice = new BiddingPrice();
                biddingPrice.Bids = new List<BiddingPriceEntity>();
                biddingPrice.Offers = new List<BiddingPriceEntity>();

                string fullCode = Convert.ToString(biddingObj.GetHeaderValue(0));
                long time = Convert.ToInt64(biddingObj.GetHeaderValue(1));

                if (fullCode.Length != 0)
                {
                    biddingPrice.Code = fullCode.Substring(1); // Remove prefix
                    biddingPrice.MarketType = CodeEntity.ConvertToMarketType(fullCode);
                }

                biddingPrice.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 100), (int)(time % 100), now.Second, now.Millisecond); // Daishin doesn't provide second 

                int[] indexes = { 3, 7, 11, 15, 19, 27, 31, 35, 39, 43 };

                for (int i = 0; i < indexes.Length; i++)
                {
                    int index = indexes[i];

                    biddingPrice.Offers.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(biddingObj.GetHeaderValue(index)),
                        Convert.ToInt64(biddingObj.GetHeaderValue(index + 2))
                        ));

                    biddingPrice.Bids.Add(new BiddingPriceEntity(i,
                        Convert.ToSingle(biddingObj.GetHeaderValue(index + 1)),
                        Convert.ToInt64(biddingObj.GetHeaderValue(index + 3))
                        ));
                }

                BiddingPriceQueue.Enqueue(biddingPrice);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        public override Dictionary<string, CodeEntity> GetStockCodeList()
        {
            var codeList = new Dictionary<string, CodeEntity>();

            try
            {
                var codeMgr = new CpCodeMgrClass();

                List<object> objList = new List<object>();
                objList.AddRange((object[])codeMgr.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_KOSPI));
                objList.AddRange((object[])codeMgr.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_KOSDAQ));
                objList.AddRange((object[])codeMgr.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_FREEBOARD));
                objList.AddRange((object[])codeMgr.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_KRX));

                foreach (string fullCode in objList)
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.CodeToName(fullCode);
                    codeEntity.MarketType = CodeEntity.ConvertToMarketType(fullCode);
                    codeList.Add(codeEntity.Code, codeEntity);
                }

                logger.Info($"Stock code list query done, Count: {codeList.Count}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codeList;
        }

        public override StockMaster GetStockMaster(string code)
        {
            var stockMaster = new StockMaster();
            stockMaster.Code = code;

            if (GetQuote(code, ref stockMaster) == true)
                stockMaster.Code = CodeEntity.RemovePrefix(code);
            else
                stockMaster.Code = string.Empty;

            return stockMaster;
        }

        public override bool IsSubscribable()
        {
            return sessionObj.GetLimitRemainCount(LIMIT_TYPE.LT_SUBSCRIBE) > 0;
        }

        protected override void OnCommunicateTimer(object sender, ElapsedEventArgs e)
        {
            // TODO : Keep firm communication code
            logger.Info($"[{GetType().Name}] Keep firm connection");

            base.OnCommunicateTimer(sender, e);
        }
    }
}
