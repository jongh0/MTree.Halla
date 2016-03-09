using CPUTILLib;
using DSCBO1Lib;
using MTree.DataStructure;
using MTree.Publisher;
using MTree.RealTimeProvider;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.DaishinPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DaishinPublisher : BrokerageFirmImplement
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public int QueryableCount
        {
            get { return sessionObj.GetLimitRemainCount(LIMIT_TYPE.LT_SUBSCRIBE); }
        }

        public bool IsQueryable
        {
            get { return QueryableCount > 0; }
        }

        #region Daishin Specific
        private CpCybos sessionObj;
        private StockMst stockMstObj;
        private StockCur stockCurObj;
        #endregion

        public DaishinPublisher() : base()
        {
            try
            {
                sessionObj = new CpCybos();
                sessionObj.OnDisconnect += sessionObj_OnDisconnect;

                stockMstObj = new StockMst();
                stockMstObj.Received += stockMstObj_Received;

                stockCurObj = new StockCur();
                stockCurObj.Received += stockCurObj_Received;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            ProcessName = nameof(DaishinPublisher);
        }

        private void sessionObj_OnDisconnect()
        {
            logger.Info("Disconnected");
        }

        private void stockMstObj_Received()
        {
            StockMasterReceived();
        }

        private void stockCurObj_Received()
        {
            StockConclusionReceived();
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            logger.Info($"Start quoting, Code: {code}");

            if (Monitor.TryEnter(LockObject, 1000 * 10) == false)
            {
                logger.Error($"Quoting failed. Not able to lock object. Code: {code}");
                return false;
            }

            int ret = -1;

            try
            {
                stockMaster.Code = code;
                if (code[0] != 'A')
                    code = "A" + code;

                QuotingStockMaster = stockMaster;

                stockMstObj.SetInputValue(0, code);
                ret = stockMstObj.BlockRequest();

                if (ret == 0)
                {
                    if (WaitQuotingEvent.WaitOne(1000 * 10) == false)
                        ret = -1;
                }
                else
                {
                    logger.Error($"Quoting request failed. Code: {code}, Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(LockObject);
            }

            return (ret == 0);
        }

        private void StockMasterReceived()
        {
            try
            {
                // 0 - (string) 종목 코드
                string code = stockMstObj.GetHeaderValue(0).ToString().Substring(1);

                if (QuotingStockMaster == null)
                {
                    logger.Error($"Current quoting master is not assigned. Received Code: {code}");
                    return;
                }

                if (QuotingStockMaster.Code != code)
                {
                    logger.Warn($"{QuotingStockMaster.Code} != {code}");
                    return;
                }

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
                logger.Error(ex);
            }
            finally
            {
                WaitQuotingEvent.Set();
            }
        }

        public bool SubscribeStock(string code)
        {
            int status = 1;

            try
            {
                stockCurObj.SetInputValue(0, "A" + code); // Add Prefix
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
                string msg = stockCurObj.GetDibMsg1();
                logger.Log(status == 0 ? LogLevel.Info : LogLevel.Error, $"Code: {code}, Status: {status}, Msg: {msg}");
            }

            return (status == 0);
        }

        public bool UnsubscribeStock(string code)
        {
            int status = 1;

            try
            {
                stockCurObj.SetInputValue(0, "A" + code); // Add Prefix
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
                string msg = stockCurObj.GetDibMsg1();
                logger.Log(status == 0 ? LogLevel.Info : LogLevel.Error, $"Code: {code}, Status: {status}, Msg: {msg}");
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
                string code = stockCurObj.GetHeaderValue(0).ToString();
                if (code.Length != 0)
                    conclusion.Code = code.Substring(1); // Remove prefix

                // 3 - (long) 시간
                // 18 - (long) 시간 (초)
                ///long time = Convert.ToInt64(stockCurObj.GetHeaderValue(3));
                long sec = Convert.ToInt64(stockCurObj.GetHeaderValue(18));
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(sec / 10000), (int)((sec / 100) % 100), (int)sec % 100, now.Millisecond); // Daishin doesn't provide milisecond 

                // 13 - (long) 현재가
                conclusion.Price = (float)Convert.ToDouble(stockCurObj.GetHeaderValue(13));

                // 14 - (char)체결 상태
                char type = Convert.ToChar(stockCurObj.GetHeaderValue(14));
                if (type == '1') conclusion.ConclusionType = ConclusionType.Buy;
                else if (type == '2') conclusion.ConclusionType = ConclusionType.Sell;

                // 17 - (long) 순간체결수량
                conclusion.Amount = Convert.ToInt64(stockCurObj.GetHeaderValue(17));

                // 20 - (char) 장 구분 플래그
                char typeTime = Convert.ToChar(stockCurObj.GetHeaderValue(20));
                if (typeTime == '1') conclusion.MarketTimeType = MarketTimeType.BeforeExpect;
                else if (typeTime == '2') conclusion.MarketTimeType = MarketTimeType.Normal;
                else if (typeTime == '3') conclusion.MarketTimeType = MarketTimeType.BeforeOffTheClock;
                else if (typeTime == '4') conclusion.MarketTimeType = MarketTimeType.AfterOffTheClock;
                else if (typeTime == '5') conclusion.MarketTimeType = MarketTimeType.AfterOffTheClock;

                StockConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override List<string> GetStockCodeList()
        {
            var codeList = new List<string>();

            try
            {

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codeList;
        }
    }
}
