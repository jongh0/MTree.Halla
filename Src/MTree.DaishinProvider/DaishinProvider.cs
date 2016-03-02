using MTree.DataStructure;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.DaishinProvider
{
    class DaishinProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public int QueryableCount
        {
            get { return stateQueryObj.GetLimitRemainCount(CPUTILLib.LIMIT_TYPE.LT_SUBSCRIBE); }
        }

        public bool IsQueryable
        {
            get { return QueryableCount > 0; }
        }

        private static object lockObject = new object();

        private static volatile DaishinProvider _instance;
        static public DaishinProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                            _instance = new DaishinProvider();
                    }
                }

                return _instance;
            }
        }

        private AutoResetEvent waitQuoting;
        private StockMaster quotingStockMaster;

        #region Daishin Specific
        private CPUTILLib.CpCybos stateQueryObj;
        private DSCBO1Lib.StockMst currentPriceQueryObj;
        private DSCBO1Lib.StockCur conclusionSubscribeObj;
        #endregion

        public DaishinProvider()
        {
            waitQuoting = new AutoResetEvent(false);

            stateQueryObj = new CPUTILLib.CpCybos();
            stateQueryObj.OnDisconnect += StateQueryObj_OnDisconnect;

            currentPriceQueryObj = new DSCBO1Lib.StockMst();
            currentPriceQueryObj.Received += CurrentPriceQueryObj_Received;

            conclusionSubscribeObj = new DSCBO1Lib.StockCur();
            conclusionSubscribeObj.Received += ConclusionSubscribeObj_Received;
        }

        private void StateQueryObj_OnDisconnect()
        {
            logger.Info("Disconnected");
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            logger.Info($"Start quoting, Code: {code}");

            if (Monitor.TryEnter(lockObject, 1000 * 10) == false)
            {
                logger.Error($"Quoting failed, Code: {code}");
                return false;
            }

            int ret = -1;

            try
            {
                stockMaster.Code = code;
                if (code[0] != 'A')
                    code = "A" + code;

                quotingStockMaster = stockMaster;

                currentPriceQueryObj.SetInputValue(0, code);
                ret = currentPriceQueryObj.BlockRequest();

                if (ret == 0)
                {
                    if (waitQuoting.WaitOne(1000 * 10) == false)
                        ret = -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Monitor.Exit(lockObject);
            }

            return (ret == 0);
        }

        private void CurrentPriceQueryObj_Received()
        {
            try
            {
                if (quotingStockMaster == null)
                    return;

                if (quotingStockMaster.Code != currentPriceQueryObj.GetHeaderValue(0).Substring(1))
                {
                    logger.Warn($"{quotingStockMaster.Code} != {currentPriceQueryObj.GetHeaderValue(0).Substring(1)}");
                    return;
                }

                // 0 - (string) 종목 코드
                quotingStockMaster.Name = currentPriceQueryObj.GetHeaderValue(1);
                // 2 - (string) 대신 업종코드
                var temp = currentPriceQueryObj.GetHeaderValue(2);
                // 8 - (long) 상한가
                quotingStockMaster.UpperLimit = (int)currentPriceQueryObj.GetHeaderValue(8);
                // 9- (long) 하한가
                quotingStockMaster.LowerLimit = (int)currentPriceQueryObj.GetHeaderValue(9);
                // 10 - (long) 전일종가
                quotingStockMaster.PreviousClosedPrice = (int)currentPriceQueryObj.GetHeaderValue(10);
                // 11 - (long) 현재가
                //currentQuotingkMaster.LastSale = (int)currentPriceQueryObj.GetHeaderValue(11);     
                // 26 - (short) 결산월     
                quotingStockMaster.SettlementMonth = (int)currentPriceQueryObj.GetHeaderValue(26);
                // 27 - (long) basis price (기준가)
                quotingStockMaster.BasisPrice = (int)currentPriceQueryObj.GetHeaderValue(27);
                // 31 - (decimal) 상장주식수 (단주)
                quotingStockMaster.ShareVolume = (long)currentPriceQueryObj.GetHeaderValue(31);
                // 32 - (long) 상장자본금
                quotingStockMaster.ListedCapital = (long)currentPriceQueryObj.GetHeaderValue(32) * 1000000;
                // 37 - (long) 외국인 한도수량
                quotingStockMaster.ForeigneLimit = (long)currentPriceQueryObj.GetHeaderValue(37);
                // 39 - (decimal) 외국인 주문가능수량
                quotingStockMaster.ForeigneAvailableRemain = (long)currentPriceQueryObj.GetHeaderValue(39);
                // 43 - (short) 매매 수량 단위 
                quotingStockMaster.QuantityUnit = (int)currentPriceQueryObj.GetHeaderValue(43);
                // 46 - (long) 전일 거래량
                quotingStockMaster.PreviousVolume = (long)currentPriceQueryObj.GetHeaderValue(46);
                // 54 - (short) 액면가
                quotingStockMaster.FaceValue = (int)currentPriceQueryObj.GetHeaderValue(54);
                // 69 -(char) 불성실 공시구분
                if ((char)currentPriceQueryObj.GetHeaderValue(69) != '0')
                {
                    InvestWarningEntity unfairAnnouncementState = new InvestWarningEntity();
                    unfairAnnouncementState.Start = DateTime.Now;
                    quotingStockMaster.UnfairAnnouncement = unfairAnnouncementState;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                waitQuoting.Set();
            }
        }

        public bool PriceSubscribe(string code)
        {
            int status = 1;

            try
            {
                conclusionSubscribeObj.SetInputValue(0, "A" + code); // Add Prefix
                conclusionSubscribeObj.Subscribe();

                while (true)
                {
                    status = conclusionSubscribeObj.GetDibStatus();
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
                string msg = conclusionSubscribeObj.GetDibMsg1();
                logger.Log(status == 0 ? LogLevel.Info : LogLevel.Error, $"Code: {code}, Status: {status}, Msg: {msg}");
            }

            return (status == 0);
        }

        public bool PriceUnsubscribe(string code)
        {
            int status = 1;

            try
            {
                conclusionSubscribeObj.SetInputValue(0, "A" + code); // Add Prefix
                conclusionSubscribeObj.Unsubscribe();

                while (true)
                {
                    status = conclusionSubscribeObj.GetDibStatus();
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
                string msg = conclusionSubscribeObj.GetDibMsg1();
                logger.Log(status == 0 ? LogLevel.Info : LogLevel.Error, $"Code: {code}, Status: {status}, Msg: {msg}");
            }

            return (status == 0);
        }

        private void ConclusionSubscribeObj_Received()
        {
            try
            {
                StockConclusion conclusion = new StockConclusion();

                // 0 - (string) 종목 코드
                string code = conclusionSubscribeObj.GetHeaderValue(0);
                if (code.Length != 0)
                    conclusion.Code = code.Substring(1); // Remove prefix

                // 3 - (long) 시간
                // 18 - (long) 시간 (초)
                long time = (long)conclusionSubscribeObj.GetHeaderValue(3);
                long sec = (long)conclusionSubscribeObj.GetHeaderValue(18);

                var now = DateTime.Now;
                conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 100), (int)(time % 100), (int)sec, now.Millisecond); // Daishin doesn't provide milisecond 

                // 13 - (long) 현재가
                conclusion.Price = conclusionSubscribeObj.GetHeaderValue(13);

                // 14 - (char)체결 상태
                char type = (char)conclusionSubscribeObj.GetHeaderValue(14);
                if (type == '1') conclusion.ConclusionType = ConclusionType.Buy;
                else if (type == '2') conclusion.ConclusionType = ConclusionType.Sell;

                // 17 - (long) 순간체결수량
                conclusion.Amount = conclusionSubscribeObj.GetHeaderValue(17);

                // 20 - (char) 장 구분 플래그
                char typeTime = (char)conclusionSubscribeObj.GetHeaderValue(20);
                if (typeTime == '1') conclusion.MarketTimeType = MarketTimeType.BeforeExpect;
                else if (typeTime == '2') conclusion.MarketTimeType = MarketTimeType.Normal;
                else if (typeTime == '3') conclusion.MarketTimeType = MarketTimeType.BeforeOffTheClock;
                else if (typeTime == '4') conclusion.MarketTimeType = MarketTimeType.AfterOffTheClock;
                else if (typeTime == '5') conclusion.MarketTimeType = MarketTimeType.AfterOffTheClock;

                // TODO : Conclusion 처리 추가해야함
                //if (ConclusionUpdated != null)
                //    ConclusionUpdated(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
