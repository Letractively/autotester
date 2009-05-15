/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MonitorFrm.cs
*
* Description: This class creates a form to control/retrieve the running 
*              status of UAF. we can start/pause/stop/resume UAF.
*                            
*
* History: 2007/09/04 wan,yu Init version.
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.MSAAUtility;
using Shrinerain.AutoTester.HTMLUtility;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.GUI
{
    public partial class MonitorFrm : Form
    {

        #region Fields

        Regex _scriptReg = new Regex(@"(?<action>\w+) On *{(?<object>.*)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex _nameReg = new Regex(@"name=(?<name>.*?),", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        string _scriptFile = @"Z:\TestScript.txt";
        TextWriter _writer;

        MSAATestApp app;
        MSAATestObjectPool pool;
        MSAAEventDispatcher dispatcher;

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

        public MonitorFrm()
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
                m = m.Replace("\t", " ");
                this.actionBox.AppendText(m + "\n");
                this.actionBox.Focus();
            }
        }

        #endregion

        #region private methods

        private void InitEvent()
        {
            OnStartClick += new ButtonActionDelegate(Playback);
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

        private void Record()
        {
            string url = @"http://www.baidu.com/";
            TestSession test = new HTMLTestSession();
            test.Browser.Start();
            test.Browser.Load(url, true);
            test.Event.OnClick += new TestObjectEventHandler(Recorder_OnClick);

            //app = new MSAATestApp();
            //app.Find(null, "SciCalc");
            //dispatcher = (MSAAEventDispatcher)app.GetEventDispatcher();
            //dispatcher.OnClick += new TestObjectEventHandler(Recorder_OnClick);
        }

        private void Playback()
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

            if (dispatcher != null)
            {
                dispatcher.Stop();
                dispatcher = null;
            }

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void Clear()
        {
            try
            {
                this.actionBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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

        private void recordBtn_Click(object sender, EventArgs e)
        {
            Record();
        }

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

        #region record

        void Recorder_OnClick(TestObject sender, TestEventArgs args)
        {
            AddLog("Click on " + sender.ToString());
        }

        #endregion

    }
}