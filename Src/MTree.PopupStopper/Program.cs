using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MTree.Utility;

namespace MTree.PopupStopper
{
    class Program
    {
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
                    bool popupClosed = false;

                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();
                        popupClosed |= CheckDibPopup();

                        cancelToken.ThrowIfCancellationRequested();
                        popupClosed |= CheckInvestorInfoPopup();

                        cancelToken.ThrowIfCancellationRequested();
                        popupClosed |= CheckRuntimeError();
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

                    if (popupClosed == false)
                        Thread.Sleep(500);
                }
            }, cancelToken);

            task.Wait();
            logger.Info("Application finished");
        }

        private static bool CheckRuntimeError()
        {
            try
            {
                IntPtr windowH = WindowUtility.FindWindow2("Microsoft Visual C++ Runtime Library", interval: 10, setForeground: false);

                if (windowH != IntPtr.Zero)
                {
                    logger.Trace($"Runtime error popup found");

                    IntPtr buttonH = WindowUtility.FindWindowEx2(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Trace($"Confirm button clicked");
                        WindowUtility.SendMessage2(buttonH, WindowUtility.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        private static bool CheckDibPopup()
        {
            try
            {
                IntPtr windowH = WindowUtility.FindWindow2("CPDIB", interval:10, setForeground : false);
                if (windowH == IntPtr.Zero)
                    windowH = WindowUtility.FindWindow2("CPSYSDIB");

                if (windowH != IntPtr.Zero)
                {
                    logger.Trace($"CPDIB/CPSYSDIB popup found");
                    //SetForegroundWindow(windowH);

                    IntPtr buttonH = WindowUtility.FindWindowEx2(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Trace($"Confirm button clicked");
                        WindowUtility.SendMessage2(buttonH, WindowUtility.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        private static bool CheckInvestorInfoPopup()
        {
            try
            {
                IntPtr windowH = WindowUtility.FindWindow2("투자자정보");

                if (windowH != IntPtr.Zero)
                {
                    logger.Info($"투자자정보 popup found");
                    //SetForegroundWindow(windowH);

                    IntPtr buttonH = WindowUtility.FindWindowEx2(windowH, "Button", "오늘은 더 묻지 않음");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Info($"Checkbox clicked");
                        WindowUtility.SendMessage2(buttonH, WindowUtility.BM_CLICK, 0, 0);
                    }

                    buttonH = WindowUtility.FindWindowEx2(windowH, "Button", "아니오(&N)");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Info($"No button clicked");
                        WindowUtility.SendMessage2(buttonH, WindowUtility.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Info("Cancel key pressed");
            e.Cancel = true;
            cancelSource.Cancel();
        }
    }
}
