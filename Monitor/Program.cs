using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace Shrinerain.AutoTester.Monitor
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_SHOWNORMAL = 1;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWNOACTIVATE = 4;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
             Application.EnableVisualStyles();
             Application.SetCompatibleTextRenderingDefault(false);

             Application.Run(new MonitorStarter());
        }
    }
}