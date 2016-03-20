using System;
using System.Collections.Generic;
using MTree.DataStructure;
using MTree.Configuration;
using System.Threading;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        private List<Candle> CandleList { get; set; } = new List<Candle>();

        public override List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, ChartTypes chartType)
        {
            if (string.IsNullOrEmpty(code) == true ||
                startDate == null || endDate == null || startDate > endDate)
                return null;

            CandleList.Clear();

            stockChartObj.SetInputValue(0, code);
            stockChartObj.SetInputValue(1, 1); // 1: 기간, 2: 개수
            stockChartObj.SetInputValue(2, endDate.Year * 10000 + endDate.Month * 100 + endDate.Day); // 요청 종료일
            stockChartObj.SetInputValue(3, startDate.Year * 10000 + startDate.Month * 100 + startDate.Day); // 요청 시작일
            stockChartObj.SetInputValue(4, uint.MaxValue);
            stockChartObj.SetInputValue(5, new int[] { 0, 1, 2, 3, 4, 5}); // 요청 필드 (날짜, 시간, 시가, 고가, 저가, 종가)
            stockChartObj.SetInputValue(6, Chart.ConvertToChar(chartType)); // 차트 구분
            stockChartObj.SetInputValue(8, 0); // 0: 갭무보정, 1: 갭보정
            stockChartObj.SetInputValue(9, 1); // 0: 무수정주가, 1: 수정주가
            stockChartObj.SetInputValue(10, 3); // 1: 시간외거래량 모두 포함, 3: 시간외거래량 모두 제외

            stockChartObj.BlockRequest();

            return CandleList;
        }

        private void StockChartReceived()
        {
            try
            {
                string code = stockChartObj.GetHeaderValue(0).ToString(); // 종목코드
                long count = Convert.ToInt64(stockChartObj.GetHeaderValue(3)); // 수신개수

                for (int i = 0; i < count; i++)
                {
                    var candle = new Candle();

                    ulong date = Convert.ToUInt64(stockChartObj.GetDataValue(0, i));
                    long time = Convert.ToInt64(stockChartObj.GetDataValue(1, i));
                    candle.Start = new DateTime((int)(date / 10000), (int)(date % 10000 / 100), (int)(date % 100), (int)(time / 100), (int)(time % 100), 0);
                    candle.Open = Convert.ToSingle(stockChartObj.GetDataValue(2, i));
                    candle.High = Convert.ToSingle(stockChartObj.GetDataValue(3, i));
                    candle.Low = Convert.ToSingle(stockChartObj.GetDataValue(4, i));
                    candle.Close = Convert.ToSingle(stockChartObj.GetDataValue(5, i));

                    CandleList.Add(candle);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
