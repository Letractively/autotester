using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shrinerain.AutoTester.Monitor
{
    public partial class MonitorStarter : Form
    {
        #region fields

        private static Monitor _monitor;

        public static Monitor Monitor
        {
            get { return MonitorStarter._monitor; }
        }


        #endregion

        public MonitorStarter()
        {
            InitializeComponent();
            StartMonitor();
        }

        private void StartMonitor()
        {
            this.Hide();

            _monitor = new Monitor();
            Monitor.TopMost = true;
            Monitor.Show();
        }



    }
}