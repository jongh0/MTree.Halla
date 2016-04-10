using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class KiwoomSessionUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void HandleSession()
        {
            int retry = 10;
            while (retry-- > 0)
            {
                IntPtr windowH = WindowUtility.FindWindow2("번개 Login", retryCount: 100);
                if (windowH == IntPtr.Zero)
                    continue;

                IntPtr handle = WindowUtility.GetWindow2(windowH, WindowUtility.GW_CHILD);
                if (handle == IntPtr.Zero)
                    continue;

                var handleList = new Dictionary<int, IntPtr>();

                int index = 0;
                while (handle != IntPtr.Zero)
                {
                    handleList.Add(index++, handle);
                    handle = WindowUtility.GetWindow2(handle, WindowUtility.GW_HWNDNEXT);
                }

                foreach (var pair in handleList)
                {
                    logger.Info($"Kiwoom login, {pair.Key}: {pair.Value.ToString("X")}");
                }

                // User password
                var userPwH = handleList[1];
                var userPw = Config.Kiwoom.UserPw;
                if (string.IsNullOrWhiteSpace(userPw) == false)
                {
                    foreach (var c in userPw.ToCharArray())
                    {
                        WindowUtility.SendMessage2(userPwH, WindowUtility.WM_CHAR, c, 0);
                    }
                }
                else
                {
                    logger.Error("User password empty");
                }

                // Certification password
                var certPwH = handleList[2];
                string certPw = Config.Kiwoom.CertPw;
                if (string.IsNullOrWhiteSpace(certPw) == false)
                {
                    foreach (var c in certPw.ToCharArray())
                    {
                        WindowUtility.SendMessage2(certPwH, WindowUtility.WM_CHAR, c, 0);
                    }
                }
                else
                {
                    logger.Error("Certification password empty");
                }

                // Simulate mode
                var simulH = handleList[11];
                WindowUtility.SendMessage2(simulH, WindowUtility.BM_SETCHECK, 1, 0);
                // TODO : 체크박스 처리가 잘 안될 때가 있음

                // Login button
                var loginH = handleList[3];
                //if (loginH != IntPtr.Zero)
                //{
                //    WindowUtility.SendMessage2(loginH, WindowUtility.BM_CLICK, 0, 0);
                //    logger.Info("Login button clicked");
                //}

                break;
            }
        }
    }
}
