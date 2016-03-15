﻿using MTree.DataStructure;
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
                System.Threading.Thread.Sleep(10000);
                int startTick = Environment.TickCount;
                foreach (KeyValuePair<string, CodeEntity> pair in kiwoomPublisher.GetStockCodeList())
                {
                    StockMaster master = kiwoomPublisher.GetStockMaster(pair.Key);
                    Console.WriteLine($"{master.Code}, {master.PER}");
                }
                Trace.WriteLine(">>>>>>>>>>>>>>> " + (Environment.TickCount - startTick));
            });
#endif
        }
    }
}
