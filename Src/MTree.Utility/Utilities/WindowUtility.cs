using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace MTree.Utility
{
    public class WindowUtility
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

        public static int GW_HWNDFIRST = 0;
        public static int GW_HWNDLAST = 1;
        public static int GW_HWNDNEXT = 2;
        public static int GW_HWNDPREV = 3;
        public static int GW_OWNER = 4;
        public static int GW_CHILD = 5;

        public static uint WM_SETTEXT = 0x000C;
        public static uint WM_KEYDOWN = 0x0100;
        public static uint WM_KEYUP = 0x0101;
        public static uint WM_CHAR = 0x0102;

        public static int VK_TAB = 0x09;
        public static int VK_CANCEL = 0x03;
        public static int VK_ENTER = 0x0D;
        public static int VK_UP = 0x26;
        public static int VK_DOWN = 0x28;
        public static int VK_RIGHT = 0x27;
        public static int VK_BACKSPACE = 0x08;

        public static uint BM_CLICK = 0X00F5;
        public static uint BM_SETCHECK = 0x00F1;
        #endregion

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static IntPtr GetWindow2(IntPtr hWnd, int uCmd, int retryCount = 0, int interval = 100)
        {
            while (retryCount-- >= 0)
            {
                IntPtr handle = GetWindow(hWnd, uCmd);
                if (handle != IntPtr.Zero)
                {
                    logger.Info($"GetWindow found, {hWnd.ToString("X")}/{uCmd}/{handle.ToString("X")}");
                    return handle;
                }

                if (interval > 0)
                    Thread.Sleep(interval);
            }

            return IntPtr.Zero;
        }

        public static IntPtr SendMessage2(IntPtr hWnd, uint Msg, int wParam, int lParam)
        {
            logger.Info($"SendMessage, {hWnd.ToString("X")}/{Msg}/{wParam}/{lParam}");
            return SendMessage(hWnd, Msg, wParam, lParam);
        }

        public static IntPtr FindWindow2(string windowName, int retryCount = 0, int interval = 100, bool setForeground = true)
        {
            try
            {
                while (retryCount-- >= 0)
                {
                    IntPtr handle = FindWindow(null, windowName);
                    if (handle != IntPtr.Zero)
                    {
                        logger.Info($"FindWindow found, {windowName}/{handle.ToString("X")}");

                        if (setForeground)
                        {
                            SetForegroundWindow(handle);
                            Thread.Sleep(1000);
                        }

                        return handle;
                    }

                    if (interval > 0)
                        Thread.Sleep(interval);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return IntPtr.Zero;
        }

        public static IntPtr FindWindowEx2(IntPtr window, string className, string caption, int retryCount = 0, int interval = 100)
        {
            try
            {
                while (retryCount-- >= 0)
                {
                    IntPtr handle = FindWindowEx(window, IntPtr.Zero, className, caption);
                    if (handle != IntPtr.Zero)
                    {
                        logger.Info($"FindWindowEx found, {window.ToString("X")}/{className}/{caption}/{handle.ToString("X")}");
                        return handle;
                    }

                    if (interval > 0)
                        Thread.Sleep(interval);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return IntPtr.Zero;
        }
    }
}
