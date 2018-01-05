using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommonLib;

namespace PopupStopper
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static CancellationTokenSource _cancelSource = new CancellationTokenSource();
        private static CancellationToken _cancelToken = _cancelSource.Token;

        static void Main(string[] args)
        {
            _logger.Info("Popup stopper started");

            Console.CancelKeyPress += Console_CancelKeyPress;

            var task = Task.Run(() =>
            {
                while (true)
                {
                    bool popupClosed = false;

                    try
                    {
                        _cancelToken.ThrowIfCancellationRequested();

                        popupClosed |= CheckDibPopup();
                        popupClosed |= CheckInvestorInfoPopup();
                        popupClosed |= CheckRuntimeErrorPopup();
                        //popupClosed |= ChecknProtectPopup();
                        popupClosed |= CheckRegularCheckupPopup();
                        popupClosed |= CheckNoticePopup();
                        popupClosed |= CheckCertiExpiredPopup();
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.Trace("Operation canceled");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }

                    if (popupClosed == false)
                        Thread.Sleep(100);
                }
            }, _cancelToken);

            task.Wait();
            _logger.Info("Popup stopper finished");
        }

        private static bool ChecknProtectPopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("nProtect Netizen v5.5", interval: 10, setForeground: false);

                if (windowH != IntPtr.Zero)
                {
                    _logger.Trace($"nProtect popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"Confirm button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private static bool CheckRuntimeErrorPopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("Microsoft Visual C++ Runtime Library", interval: 10, setForeground: false);

                if (windowH != IntPtr.Zero)
                {
                    _logger.Trace($"Runtime error popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"Confirm button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private static bool CheckDibPopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("CPDIB", interval:10, setForeground : false);
                if (windowH == IntPtr.Zero)
                    windowH = WindowsAPI.findWindow("CPSYSDIB");

                if (windowH != IntPtr.Zero)
                {
                    _logger.Trace($"CPDIB/CPSYSDIB popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"Confirm button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private static bool CheckInvestorInfoPopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("투자자정보");

                if (windowH != IntPtr.Zero)
                {
                    _logger.Trace($"투자자정보 popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "오늘은 더 묻지 않음");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"Checkbox clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);
                    }

                    buttonH = WindowsAPI.findWindowEx(windowH, "Button", "아니오(&N)");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"No button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private static bool CheckRegularCheckupPopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("정기 점검", interval: 10, setForeground: false);

                if (windowH != IntPtr.Zero)
                {
                    _logger.Trace($"Regular check-up popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"Confirm button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private static bool CheckNoticePopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("CybosPlus 공지사항", interval: 10, setForeground: false);

                if (windowH != IntPtr.Zero)
                {
                    _logger.Trace($"Notice popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"Confirm button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private static bool CheckCertiExpiredPopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("인증서 만료공지", interval: 10, setForeground: false);

                if (windowH != IntPtr.Zero)
                {
                    _logger.Trace($"Certi expired popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Trace($"Confirm button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _logger.Info("Cancel key pressed");
            e.Cancel = true;
            _cancelSource.Cancel();
        }
    }
}
