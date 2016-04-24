using MongoDB.Bson;
using MTree.DataStructure;
using System;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        public override StockMaster GetStockMaster(string code)
        {
            var stockMaster = new StockMaster();
            stockMaster.Id = ObjectId.GenerateNewId();
            stockMaster.Code = code;

            if (GetQuote(code, ref stockMaster) == true)
                stockMaster.Code = CodeEntity.RemovePrefix(code);
            else
                stockMaster.Code = string.Empty;

            return stockMaster;
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if (sessionObj.IsConnect != 1)
            {
                logger.Error("Quoting failed, session not connected");
                return false;
            }

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
                    if (QuotingStockMaster.Code != string.Empty)
                    {
                        logger.Info($"Quoting done, Code: {code}");
                        return true;
                    }

                    logger.Error($"Quoting fail, Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail, Code: {code}, result: {ret}");
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

        private void stockMstObj_Received()
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
                QuotingStockMaster.PreviousClosingPrice = (int)stockMstObj.GetHeaderValue(10);

                // 11 - (long) 현재가
                //currentQuotingkMaster.LastSale = (int)currentPriceQueryObj.GetHeaderValue(11);  

                // 26 - (short) 결산월     
                QuotingStockMaster.SettlementMonth = Convert.ToInt32(stockMstObj.GetHeaderValue(26));

                // 27 - (long) basis price (기준가)
                QuotingStockMaster.BasisPrice = (int)stockMstObj.GetHeaderValue(27);

                // 31 - (decimal) 상장주식수 (단주)
                QuotingStockMaster.ShareVolume = Convert.ToInt64(stockMstObj.GetHeaderValue(31));

                // 32 - (long) 상장자본금
                QuotingStockMaster.ListedCapital = Convert.ToInt64(stockMstObj.GetHeaderValue(32)) * 1000000;

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
            }
            catch (Exception ex)
            {
                QuotingStockMaster.Code = string.Empty;
                logger.Error(ex);
            }
        }
    }
}
