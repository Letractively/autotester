using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Shrinerain.AutoTester.GUI
{
    public partial class Monitor : Form
    {

        #region Fields
        //add message to rich text box.
        private delegate void AddLogDelegate(string m);

        //delegate for button actions.
        public delegate void ButtonActionDelegate();


        //event for button click action, communicate with other program.
        public event ButtonActionDelegate OnStartClick;
        public event ButtonActionDelegate OnStopClick;
        public event ButtonActionDelegate OnPauseClick;
        public event ButtonActionDelegate OnHighlightClick;

        private bool _started = true;

        private bool _isHightlight = false;

        #endregion

        #region properties

        public Boolean Started
        {
            get { return this._started; }
        }

        public Boolean IsHighlight
        {
            get { return this._isHightlight; }
        }
        #endregion


        #region ctor

        public Monitor()
        {
            InitializeComponent();

            InitButtonStatus();

            InitEvent();
        }

        #endregion




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

        private void InitEvent()
        {
            OnStartClick += new ButtonActionDelegate(Start);
            OnStopClick += new ButtonActionDelegate(Stop);
            OnHighlightClick += new ButtonActionDelegate(HighLight);
            OnPauseClick += new ButtonActionDelegate(Pause);
        }

        private void InitButtonStatus()
        {
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
            this.btnPause.Enabled = true;
        }

        private void Start()
        {
            _started = true;

            ChangeButtonStatus(false, true, true);

            AddLog("Start...");
        }

        private void Pause()
        {
            _started = false;

            ChangeButtonStatus(true, true, false);

            AddLog("Pause.");
        }

        private void Stop()
        {
            _started = false;

            ChangeButtonStatus(true, false, false);

            AddLog("Stop.");
        }

        private void HighLight()
        {
            if (_isHightlight)
            {
                AddLog("Cancle Highlight.");
            }
            else
            {
                AddLog("Use Highlight.");
            }

            _isHightlight = !_isHightlight;


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

        private void ChangeButtonStatus(bool start, bool stop, bool pause)
        {
            this.btnStart.Enabled = start;
            this.btnStop.Enabled = stop;
            this.btnPause.Enabled = pause;
        }

        #endregion

        #region event

        private void btnStart_Click(object sender, EventArgs e)
        {
            OnStartClick();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            OnStopClick();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            OnPauseClick();
        }

        private void btnHighlight_Click(object sender, EventArgs e)
        {
            OnHighlightClick();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        #endregion
    }
}