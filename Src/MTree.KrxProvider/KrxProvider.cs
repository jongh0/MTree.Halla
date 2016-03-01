using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTree.KrxProvider
{
    public class KrxProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region 관리종목
        public static void Collect040301()
        {
            try
            {
                using (var web = new WebBrowser())
                {
                    web.Navigate("http://marketdata.krx.co.kr/mdi#document=040301");
                    Thread.Sleep(1000 * 10);

                    var table = web.Document.GetElementsByTagName("tbody");
                    if (table.Count > 0)
                    {
                        foreach (HtmlElement tr in table[0].GetElementsByTagName("tr"))
                        {
                            List<string> columns = new List<string>();

                            foreach (HtmlElement td in tr.GetElementsByTagName("td"))
                            {
                                columns.Add(td.InnerText);
                            }
                        }

                        logger.Info("Collect040301 success");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        #endregion

        #region 상호출자제한기업집단 그룹별 현재가
        public static void Collect040306()
        {
            try
            {
                using (var web = new WebBrowser())
                {
                    web.Navigate("http://marketdata.krx.co.kr/mdi#document=040306");
                    Thread.Sleep(1000 * 10);

                    var table = web.Document.GetElementsByTagName("tbody");
                    if (table.Count > 0)
                    {
                        foreach (HtmlElement tr in table[0].GetElementsByTagName("tr"))
                        {
                            List<string> columns = new List<string>();

                            foreach (HtmlElement td in tr.GetElementsByTagName("td"))
                            {
                                columns.Add(td.InnerText);
                            }
                        }

                        logger.Info("Collect040306 success");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        } 
        #endregion
    }
}
