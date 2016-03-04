using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MTree.DaishinPopupStopper
{
    class Program
    {
        #region Dll Import
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);  //1. 찾고자하는 클래스이름, 2.캡션값

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string Ipsz1, string Ipsz2);    //1.바로위의 부모값을 주고 2. 0이나 null 3,4.클래스명과 캡션명을 

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        const int GW_HWNDFIRST = 0;
        const int GW_HWNDLAST = 1;
        const int GW_HWNDNEXT = 2;
        const int GW_HWNDPREV = 3;
        const int GW_OWNER = 4;
        const int GW_CHILD = 5;

        const uint WM_SETTEXT = 0x000C;
        const uint WM_KEYDOWN = 0x0100;
        const uint WM_KEYUP = 0x0101;
        const uint WM_CHAR = 0x0102;

        const int VK_TAB = 0x09;
        const int VK_CANCEL = 0x03;
        const int VK_ENTER = 0x0D;
        const int VK_UP = 0x26;
        const int VK_DOWN = 0x28;
        const int VK_RIGHT = 0x27;
        const int VK_BACKSPACE = 0x08;

        const int BM_CLICK = 0X00F5;
        #endregion

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static CancellationTokenSource cancelSource = new CancellationTokenSource();
        private static CancellationToken cancelToken = cancelSource.Token;

        static void Main(string[] args)
        {
            logger.Info("Application Started");

            Console.CancelKeyPress += Console_CancelKeyPress;

            var task = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();
                        CheckDibPopup();

                        cancelToken.ThrowIfCancellationRequested();
                        CheckInvestorInfoPopup();
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine("Operation canceled");
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                    Thread.Sleep(500);
                }
            }, cancelToken);

            task.Wait();
            logger.Info("Application finished");
        }

        private static void CheckDibPopup()
        {
            try
            {
                IntPtr windowH = FindWindow(null, "CPDIB");
                if (windowH == IntPtr.Zero)
                    windowH = FindWindow(null, "CPSYSDIB");

                if (windowH != IntPtr.Zero)
                {
                    logger.Info($"CPDIB/CPSYSDIB popup found");
                    SetForegroundWindow(windowH);

                    IntPtr buttonH = FindWindowEx(windowH, IntPtr.Zero, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Info($"Confirm button clicked");
                        SendMessage(buttonH, BM_CLICK, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void CheckInvestorInfoPopup()
        {
            try
            {
                IntPtr windowH = FindWindow(null, "투자자정보");

                if (windowH != IntPtr.Zero)
                {
                    logger.Info($"투자자정보 popup found");
                    SetForegroundWindow(windowH);

                    IntPtr buttonH = FindWindowEx(windowH, IntPtr.Zero, "Button", "오늘은 더 묻지 않음");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Info($"Checkbox clicked");
                        SendMessage(buttonH, BM_CLICK, 0, 0);
                    }

                    buttonH = FindWindowEx(windowH, IntPtr.Zero, "Button", "아니오(&N)");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Info($"No button clicked");
                        SendMessage(buttonH, BM_CLICK, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Info("Cancel key pressed");
            e.Cancel = true;
            cancelSource.Cancel();
        }
    }
}
