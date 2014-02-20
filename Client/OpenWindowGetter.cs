using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowsAPI
{
    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    /// <summary>Contains a method to get all the open windows.</summary>
    public static class OpenWindowGetter
    {
        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary> 
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        public static IDictionary<IntPtr, WindowInfo> GetOpenWindows()
        {
            IntPtr lShellWindow = GetShellWindow();
            Dictionary<IntPtr, WindowInfo> lWindows = new Dictionary<IntPtr, WindowInfo>();

            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                if (hWnd == lShellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int lLength = GetWindowTextLength(hWnd);
                if (lLength == 0) return true;

                StringBuilder lBuilder = new StringBuilder(lLength);
                GetWindowText(hWnd, lBuilder, lLength + 1);
                Rect recc = new Rect();
                GetWindowRect(hWnd, ref recc);
                lWindows[hWnd] = new WindowInfo(lBuilder.ToString(), recc);
                return true;

            }, 0);

            return lWindows;
        }

        delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
    }

    public class WindowInfo
    {
        public WindowInfo()
        {
            Title = "";
            Recangle = new Rect();
        }

        public WindowInfo(string s, Rect r)
        {
            Title = s;
            Recangle = r;
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private Rect recangle;

        public Rect Recangle
        {
            get { return recangle; }
            set { recangle = value; }
        }
    }
}