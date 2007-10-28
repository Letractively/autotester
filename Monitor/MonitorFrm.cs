using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shrinerain.AutoTester.Monitor
{
    public partial class Monitor : Form
    {

        public Monitor()
        {
            InitializeComponent();
        }


        #region private methods

        private void Start()
        {

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