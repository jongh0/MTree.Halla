using MTree.Configuration;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.KiwoomSessionManager
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                logger.Info($"Application Started. Args:{string.Join(",", args)}");

                if (string.IsNullOrEmpty(Config.Kiwoom.UserId) == true ||
                    string.IsNullOrEmpty(Config.Kiwoom.UserPw) == true ||
                    string.IsNullOrEmpty(Config.Kiwoom.CertPw) == true)
                {
                    logger.Error("Check Kiwoom configuration");
                    return;
                }

                // Find Kiwoom Starter
                var khministarterHandle = WindowsAPI.findWindow("번개 Login", retryCount: 10);
                if (khministarterHandle == IntPtr.Zero)
                {
                    logger.Error("Kiwoom Starter not found");
                    return;
                }

                // UserPw, CertPw 핸들 찾기
                IntPtr idH = WindowsAPI.getWindow(khministarterHandle, WindowsAPI.GW_CHILD);
                IntPtr pwH = WindowsAPI.getWindow(idH, WindowsAPI.GW_HWNDNEXT);
                IntPtr certPwH = WindowsAPI.getWindow(pwH, WindowsAPI.GW_HWNDNEXT);
                IntPtr loginBtnH = WindowsAPI.getWindow(certPwH, WindowsAPI.GW_HWNDNEXT);

                EnterUserPw(pwH);
                EnterCertPw(certPwH);

                // 공인인증서 입력 오류 발생 방지
                WindowsAPI.postMessage(khministarterHandle, WindowsAPI.WM_KEYDOWN, WindowsAPI.VK_ENTER, 0);
                WindowsAPI.postMessage(khministarterHandle, WindowsAPI.WM_KEYDOWN, WindowsAPI.VK_ENTER, 0);
                ClickButton(loginBtnH); 

                // Popup Handling
                while (WindowsAPI.isWindow(khministarterHandle) == true)
                {
                    HandleVersionUpdate();
                    HandleAdditionalPopupFail();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                logger.Info("Application finished");
            }
        }

        private static void EnterUserPw(IntPtr pwH)
        {
            try
            {
                string userPw = Config.Kiwoom.UserPw;
                if (string.IsNullOrWhiteSpace(userPw) == false)
                {
                    foreach (var c in userPw.ToCharArray())
                    {
                        WindowsAPI.sendMessage(pwH, WindowsAPI.WM_CHAR, c, 0);
                    }
                }
                else
                {
                    logger.Error("User password empty");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void EnterCertPw(IntPtr certPwH)
        {
            try
            {
                string certPw = Config.Kiwoom.CertPw;
                if (string.IsNullOrWhiteSpace(certPw) == false)
                {
                    foreach (var c in certPw.ToCharArray())
                    {
                        WindowsAPI.sendMessage(certPwH, WindowsAPI.WM_CHAR, c, 0);
                    }
                }
                else
                {
                    logger.Error("Certification password empty");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void ClickButton(IntPtr loginBtnH)
        {
            try
            {
                if (loginBtnH != IntPtr.Zero)
                {
                    WindowsAPI.postMessage(loginBtnH, WindowsAPI.BM_CLICK, 0, 0);
                    logger.Info("Button clicked");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void HandleVersionUpdate()
        {
            // Version Update
            var verUpdateHandle = WindowsAPI.findWindow("khministarter", retryCount: 10);
            if (verUpdateHandle != IntPtr.Zero)
            {
                logger.Info("Version update popup window found");
                int lauchingProcessId = Convert.ToInt32(Environment.GetCommandLineArgs()[1]);
                if (Process.GetProcessById(lauchingProcessId) != null)
                    ProcessUtility.Kill(lauchingProcessId);

                IntPtr okBtnHandle = WindowsAPI.getWindow(verUpdateHandle, WindowsAPI.GW_CHILD);
                ClickButton(okBtnHandle);

                // Wait for popup closed
                Thread.Sleep(1000);

                while (true)
                {
                    var updateCompleteHandle = WindowsAPI.findWindow("khministarter", retryCount: 10);
                    if (updateCompleteHandle != IntPtr.Zero)
                    {
                        logger.Info("Update complete.");
                        okBtnHandle = WindowsAPI.getWindow(updateCompleteHandle, WindowsAPI.GW_CHILD);
                        ClickButton(okBtnHandle);
                        ProcessUtility.Start(ProcessTypes.KiwoomPublisher);
                        break;
                    }
                }
            }
        }

        private static void HandleAdditionalPopupFail()
        {
            // Server Connection Fail
            var popupHandle = WindowsAPI.findWindow("번개", retryCount: 10);
            if (popupHandle != IntPtr.Zero)
            {
                IntPtr okBtnHandle = WindowsAPI.getWindow(popupHandle, WindowsAPI.GW_CHILD);
                string buttonCaption = WindowsAPI.getWindowCaption(okBtnHandle);
                logger.Info($"{buttonCaption} popup window found");

                ClickButton(okBtnHandle);
                
                // Wait for popup closed
                Thread.Sleep(2000);

                if (buttonCaption.Contains("확인"))
                {
                    logger.Info("Server connection fail popup window found");

                    var failWindowHandle = WindowsAPI.findWindow("khministarter", retryCount: 10);
                    var confirmButtonHandle = WindowsAPI.getWindow(failWindowHandle, WindowsAPI.GW_CHILD);
                    if (confirmButtonHandle != IntPtr.Zero)
                    {
                        ClickButton(confirmButtonHandle);
                    }

                    Thread.Sleep(2000);
                }
            }
        }
    }
}
