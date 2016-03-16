using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MTree.KiwoomPublisher
{
    public partial class MainWindow : Form
    {
        KiwoomPublisher kiwoomPublisher;

        public MainWindow()
        {
            InitializeComponent();

            kiwoomPublisher = new KiwoomPublisher(axKHOpenAPI);
#if false
            Task.Run(() => {
                for (int i = 0; i < 100; i++)
                {
                    System.Threading.Thread.Sleep(100);

                    StockMaster master = kiwoomPublisher.GetStockMaster("000087");
                    if (master.Code != "000087")
                    {
                        Debugger.Break();
                    }   
                }
            });
#endif
#if false
            Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                Dictionary<string, CodeEntity> list = kiwoomPublisher.GetStockCodeList();
                Console.WriteLine($"count:{list.Count}");
                int cnt = 0;
                foreach (KeyValuePair<string, CodeEntity> codeEntity in list)
                {
                    if (codeEntity.Value.Market == MarketType.KOSPI || codeEntity.Value.Market == MarketType.KOSDAQ ||
                                    codeEntity.Value.Market == MarketType.ETF || codeEntity.Value.Market == MarketType.ETN ||
                                    codeEntity.Value.Market == MarketType.FREEBOARD)
                    {
                        if (codeEntity.Value.Code != "108630" && codeEntity.Value.Code != "025850")/* Daishin not support */
                        {
                            StockMaster master = kiwoomPublisher.GetStockMaster(codeEntity.Key);
                            Console.WriteLine($"{master.Code}, {master.PER}");
                            cnt++;
                        }
                    }
                }
                Console.WriteLine($"count:{cnt}");
                sw.Stop();
                Trace.WriteLine(">>>>>>>>>>>>>>> " + (sw.Elapsed));
                Debugger.Break();
            });
#endif
        }
    }
}
