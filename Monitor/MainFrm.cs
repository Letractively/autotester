using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shrinerain.AutoTester.GUI
{
    public partial class MainFrm : Form
    {
        #region fields

        private static Monitor _monitor;

        public static Monitor Monitor
        {
            get { return MainFrm._monitor; }
        }


        #endregion

        #region ctor
        public MainFrm()
        {
            InitializeComponent();
           // StartMonitor();
        }
        #endregion

        #region monitor

        private void StartMonitor()
        {
            // this.Hide();

            _monitor = new Monitor();
            Monitor.TopMost = true;
            Monitor.Show();

            RegMonitorEvent();

        }

        private void RegMonitorEvent()
        {
            _monitor.OnStartClick += new Monitor.ButtonActionDelegate(MonitorStart);
            _monitor.OnStopClick += new Monitor.ButtonActionDelegate(MonitorStop);
            _monitor.OnPauseClick += new Monitor.ButtonActionDelegate(MonitorPause);
            _monitor.OnHighlightClick += new Monitor.ButtonActionDelegate(MonitorHighlight);
        }

        private void MonitorStart()
        {

        }

        private void MonitorStop()
        {

        }

        private void MonitorPause()
        {

        }

        private void MonitorHighlight()
        {

        }


        #endregion

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnOpenDriveFile_Click(object sender, EventArgs e)
        {

        }

        private void MainFrm_Load(object sender, EventArgs e)
        {

        }



    }
}