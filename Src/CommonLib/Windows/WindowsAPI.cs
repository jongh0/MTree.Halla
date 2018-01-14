using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace CommonLib.Windows
{
    public class WindowsAPI
    {
        #region Dll Import
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string strClassName, string strWindowName);  //1. 찾고자하는 클래스이름, 2.캡션값

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string Ipsz1, string Ipsz2);    //1.바로위의 부모값을 주고 2. 0이나 null 3,4.클래스명과 캡션명을 

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowCaption(IntPtr hwnd, StringBuilder lpString, int maxCount);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public static readonly int GW_HWNDFIRST = 0;
        public static readonly int GW_HWNDLAST = 1;
        public static readonly int GW_HWNDNEXT = 2;
        public static readonly int GW_HWNDPREV = 3;
        public static readonly int GW_OWNER = 4;
        public static readonly int GW_CHILD = 5;

        public static readonly uint WM_SETTEXT = 0x000C;
        public static readonly uint WM_KEYDOWN = 0x0100;
        public static readonly uint WM_KEYUP = 0x0101;
        public static readonly uint WM_CHAR = 0x0102;

        public static readonly int VK_TAB = 0x09;
        public static readonly int VK_CANCEL = 0x03;
        public static readonly int VK_ENTER = 0x0D;
        public static readonly int VK_UP = 0x26;
        public static readonly int VK_DOWN = 0x28;
        public static readonly int VK_RIGHT = 0x27;
        public static readonly int VK_BACKSPACE = 0x08;

        public static readonly uint BM_CLICK = 0X00F5;
        public static readonly uint BM_SETCHECK = 0x00F1;
        #endregion

        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static IntPtr getWindow(IntPtr hWnd, int uCmd, int retryCount = 0, int interval = 100)
        {
            while (retryCount-- >= 0)
            {
                IntPtr handle = GetWindow(hWnd, uCmd);
                if (handle != IntPtr.Zero)
                {
                    _logger.Trace($"getWindow found, {hWnd.ToString("X")}/{uCmd}/{handle.ToString("X")}");
                    return handle;
                }

                if (interval > 0)
                    Thread.Sleep(interval);
            }

            return IntPtr.Zero;
        }

        public static IntPtr sendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam)
        {
            _logger.Trace($"sendMessage, {hWnd.ToString("X")}/{Msg}/{wParam}/{lParam}");
            return SendMessage(hWnd, Msg, wParam, lParam);
        }

        public static IntPtr postMessage(IntPtr hWnd, uint Msg, int wParam, int lParam)
        {
            _logger.Trace($"postMessage, {hWnd.ToString("X")}/{Msg}/{wParam}/{lParam}");
            return PostMessage(hWnd, Msg, wParam, lParam);
        }

        public static IntPtr findWindow(string windowName, int retryCount = 0, int interval = 100, bool setForeground = true)
        {
            try
            {
                while (retryCount-- >= 0)
                {
                    IntPtr handle = FindWindow(null, windowName);
                    if (handle != IntPtr.Zero)
                    {
                        _logger.Trace($"findWindow found, {windowName}/{handle.ToString("X")}");

                        if (setForeground)
                        {
                            SetForegroundWindow(handle);
                            Thread.Sleep(100);
                        }

                        return handle;
                    }

                    if (interval > 0)
                        Thread.Sleep(interval);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return IntPtr.Zero;
        }

        public static IntPtr findWindowEx(IntPtr window, string className, string caption, int retryCount = 0, int interval = 100)
        {
            try
            {
                while (retryCount-- >= 0)
                {
                    IntPtr handle = FindWindowEx(window, IntPtr.Zero, className, caption);
                    if (handle != IntPtr.Zero)
                    {
                        _logger.Trace($"findWindowEx found, {window.ToString("X")}/{className}/{caption}/{handle.ToString("X")}");
                        return handle;
                    }

                    if (interval > 0)
                        Thread.Sleep(interval);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return IntPtr.Zero;
        }

        public static bool isWindow(IntPtr window)
        {
            return IsWindow(window);
        }

        public static string getWindowCaption(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowCaption(hwnd, sb, 256);
            return sb.ToString();
        }

        public static bool setForegroundWindow(IntPtr handle)
        {
            return SetForegroundWindow(handle);
        }
    }
}
