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
using System.Diagnostics;
using MTree.Configuration;

namespace MTree.DaishinPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DaishinPublisher : BrokerageFirmBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected int LastQuoteTick { get; set; } = Environment.TickCount;

        #region Daishin Specific
        private CpCybosClass sessionObj;
        private StockMstClass stockMstObj;
        private StockCurClass stockCurObj;
        private StockJpbidClass stockJpbidObj;
        private StockChartClass stockChartObj;

        private WorldCurClass worldCurObj;

        #endregion

        public DaishinPublisher() : base()
        {
            try
            {
                QuoteInterval = 15 * 1000 / 60; // 15초당 60개

                sessionObj = new CpCybosClass();
                sessionObj.OnDisconnect += sessionObj_OnDisconnect;

                stockMstObj = new StockMstClass();
                stockMstObj.Received += stockMstObj_Received;

                stockCurObj = new StockCurClass();
                stockCurObj.Received += stockCurObj_Received;

                stockJpbidObj = new StockJpbidClass();
                stockJpbidObj.Received += stockJpbidObj_Received;

                worldCurObj = new WorldCurClass();
                worldCurObj.Received += worldCurObj_Received;

                stockChartObj = new StockChartClass();
                stockChartObj.Received += stockChartObj_Received;

                if (sessionObj.IsConnect != 1)
                {
                    logger.Error("Session not connected");
                    return;
                }

                logger.Info($"Server type: {sessionObj.ServerType}");

                StartBiddingPriceQueueTask();
                StartStockConclusionQueueTask();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

<<<<<<< HEAD
        public override Dictionary<string, CodeEntity> GetStockCodeList()
        {
            var codeList = new Dictionary<string, CodeEntity>();

            try
            {
                var codeMgr = new CpCodeMgrClass();


                #region Index
                foreach (string fullCode in (object[])codeMgr.GetIndustryList())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.GetIndustryName(fullCode);
                    codeEntity.MarketType = MarketTypes.INDEX;

                    codeList.Add(codeEntity.Code, codeEntity);
                }

                foreach (string fullCode in (object[])codeMgr.GetKosdaqIndustry1List())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.GetIndustryName(fullCode);
                    codeEntity.MarketType = MarketTypes.INDEX;

                    codeList.Add(codeEntity.Code, codeEntity);
                }

                foreach (string fullCode in (object[])codeMgr.GetKosdaqIndustry2List())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.GetIndustryName(fullCode);
                    codeEntity.MarketType = MarketTypes.INDEX;

                    codeList.Add(codeEntity.Code, codeEntity);
                } 
                #endregion

                #region KOSPI & ETF & ETN
                foreach (string fullCode in (object[])codeMgr.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_KOSPI))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.CodeToName(fullCode);
                    if (codeMgr.GetStockSectionKind(fullCode) == CPE_KSE_SECTION_KIND.CPC_KSE_SECTION_KIND_ETF)
                    {
                        codeEntity.MarketType = MarketTypes.ETF;
                    }
                    else if (fullCode[0] == 'Q')
                    {
                        codeEntity.MarketType = MarketTypes.ETN;
                    }
                    else
                    {
                        codeEntity.MarketType = MarketTypes.KOSPI;
                    }

                    codeList.Add(codeEntity.Code, codeEntity);
                }
                #endregion

                #region KOSDAQ
                foreach (string fullCode in (object[])codeMgr.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_KOSDAQ))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.KOSDAQ;
                    codeList.Add(codeEntity.Code, codeEntity);
                }
                #endregion

                #region KONEX
                foreach (string fullCode in (object[])codeMgr.GetStockListByMarket((CPE_MARKET_KIND)5))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.KONEX;
                    codeList.Add(codeEntity.Code, codeEntity);
                }
                #endregion

                #region Freeboard
                foreach (string fullCode in (object[])codeMgr.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_FREEBOARD))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.FREEBOARD;
                    codeList.Add(codeEntity.Code, codeEntity);
                }
                #endregion

                #region ELW
                var elwCodeMgr = new CpElwCodeClass();
                int cnt = elwCodeMgr.GetCount();
                for (int i = 0; i < cnt; i++)
                {
                    string fullCode = elwCodeMgr.GetData(0, (short)i).ToString();
                    if (fullCode.Length == 0)
                        continue;

                    var codeEntity = new CodeEntity();
                    codeEntity.Code = fullCode.Substring(1);
                    codeEntity.Name = codeMgr.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.ELW;
                    codeList.Add(codeEntity.Code, codeEntity);
                }
                #endregion

                logger.Info($"Stock code list query done, Count: {codeList.Count}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codeList;
        }

=======
>>>>>>> origin/master
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
<<<<<<< HEAD

        private void WorldCurObj_Received()
=======
        private void worldCurObj_Received()
>>>>>>> origin/master
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

        private void stockJpbidObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            BiddingPriceReceived();
        }

        private void stockChartObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            StockChartReceived();
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

#if true // Daishin Warning Code Sample
                // 66 - (char) 관리구분
                char administrative = Convert.ToChar(stockMstObj.GetHeaderValue(66));
                if (administrative == 'Y')          QuotingStockMaster.Administrative = AdministrativeTypes.Administrative;
                else                                QuotingStockMaster.Administrative = AdministrativeTypes.Normal;

                // 67 - (char)투자경고구분
                char investmentWarning = Convert.ToChar(stockMstObj.GetHeaderValue(67));
                if (investmentWarning == '2')       QuotingStockMaster.InvestmentWarning = InvestmentWarningTypes.Caution;
                else if (investmentWarning == '3')  QuotingStockMaster.InvestmentWarning = InvestmentWarningTypes.Warning;
                else if (investmentWarning == '4')  QuotingStockMaster.InvestmentWarning = InvestmentWarningTypes.ForeRisk;
                else if (investmentWarning == '5')  QuotingStockMaster.InvestmentWarning = InvestmentWarningTypes.Risk;
                else                                QuotingStockMaster.InvestmentWarning = InvestmentWarningTypes.Normal;

                // 68 - (char)거래정지구분
                char tradingHalt = Convert.ToChar(stockMstObj.GetHeaderValue(68));
                if (tradingHalt == 'Y')             QuotingStockMaster.TradingHalt = TradingHaltTypes.TradingHalt;
                else                                QuotingStockMaster.TradingHalt = TradingHaltTypes.Normal;

                // 69 - (char) 불성실 공시구분
                char unfairAnnouncement = Convert.ToChar(stockMstObj.GetHeaderValue(69));
                if (unfairAnnouncement == '1')      QuotingStockMaster.UnfairAnnouncement = UnfairAnnouncementTypes.UnfairAnnouncement;
                else if (unfairAnnouncement == '2') QuotingStockMaster.UnfairAnnouncement = UnfairAnnouncementTypes.UnfairAnnouncement2;
                else                                QuotingStockMaster.UnfairAnnouncement = UnfairAnnouncementTypes.Normal;
#endif
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
                conclusion.Code = CodeEntity.RemovePrefix(fullCode);

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

                // 20 - (char) 장 구분 플래그
                char marketTime = Convert.ToChar(stockCurObj.GetHeaderValue(20));
                if (marketTime == '1')        conclusion.MarketTimeType = MarketTimeTypes.BeforeExpect;
                else if (marketTime == '2')   conclusion.MarketTimeType = MarketTimeTypes.Normal;
                else if (marketTime == '3')   conclusion.MarketTimeType = MarketTimeTypes.BeforeOffTheClock;
                else if (marketTime == '4')   conclusion.MarketTimeType = MarketTimeTypes.AfterOffTheClock;
                else if (marketTime == '5')   conclusion.MarketTimeType = MarketTimeTypes.AfterOffTheClock;
                else logger.Error($"Stock conclusion market time type error, {stockCurObj.GetHeaderValue(20)}");

                StockConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void StockChartReceived()
        {
            try
            {

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
                    logger.Info($"Subscribe bidding, Code: {code}");
                else
                    logger.Error($"Subscribe bidding error, Code: {code}, Status: {status}, Msg: {stockJpbidObj.GetDibMsg1()}");
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
                    logger.Trace($"Unsubscribe bidding, Code: {code}");
                else
                    logger.Error($"Unsubscribe bidding error, Code: {code}, Status: {status}, Msg: {stockJpbidObj.GetDibMsg1()}");
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
            int remainCount = sessionObj.GetLimitRemainCount(LIMIT_TYPE.LT_SUBSCRIBE);
            return remainCount > 0;
        }
    }
}
