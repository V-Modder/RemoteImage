using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Istance
{
    public sealed class InstanceHandler
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int WS_SHOWNORMAL = 1;

        public InstanceHandler() { }

        public static Process RunningInstance()
        {

            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);

            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("/", "\\") ==
                        process.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        public static void HandleRunningInstance(Process instance)
        {

            if (instance != null)
            {
                ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);
                SetForegroundWindow(instance.MainWindowHandle);
            }
        }
    }
}