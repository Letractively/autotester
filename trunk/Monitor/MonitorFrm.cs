using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shrinerain.AutoTester.Monitor
{
    public partial class Monitor : Form
    {

        #region Fields
        //add message to rich text box.
        private delegate void AddLogDelegate(string m);
        #endregion

        public Monitor()
        {
            InitializeComponent();
        }



        #region public methods

        public void AddLog(string m)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddLogDelegate(AddLog), m);
            }
            else
            {
                this.actionBox.Text += (m + "\n");
            }
        }

        #endregion

        #region private methods

        private void Start()
        {
            for (int i = 0; i < 100; i++)
            {
                AddLog("test a test a test!!!!!!!!!!!!!!!!!!!!!!!!.");
                Thread.Sleep(500);
            }
        }

        private void Pause()
        {

        }

        private void Stop()
        {

        }

        private void HighLight()
        {

        }

        private void Copy()
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(this.actionBox.Text, true);
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void Clear()
        {
            try
            {
                this.actionBox.Clear();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        #endregion

        #region event

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(Start));
            t.IsBackground = true;
            t.Start();
            // Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {

        }

        private void btnHighlight_Click(object sender, EventArgs e)
        {

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        #endregion
    }
}