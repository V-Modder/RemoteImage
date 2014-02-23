using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Istance;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutexObj = null;
            bool mutexCreated = false;
            mutexObj = new Mutex(true, "RemoteImage_Client", out mutexCreated);
            Process proc = InstanceHandler.RunningInstance();

            if (proc != null || !mutexCreated)
            {
                //ErrorMessages.Show_OnlyOneProgInstance();
                InstanceHandler.HandleRunningInstance(proc);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ClientDlg());
            }
        }
    }
}
