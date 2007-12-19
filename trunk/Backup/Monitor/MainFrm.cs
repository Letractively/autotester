using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Framework;

namespace Shrinerain.AutoTester.GUI
{
    public partial class MainFrm : Form
    {
        #region fields

        private string _projectConfigFile;
        private string _driveFile;

        private Thread _testJobThread;

        private static Monitor _monitor;


        #endregion

        #region Properties

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
            RegMonitorEvent();
            _monitor.Show();

            //move the monitor window to 600,0
            Win32API.SetWindowPos(_monitor.Handle, IntPtr.Zero, 600, 0, 260, 150, 0);

            //minsize the main window.
            Win32API.SendMessage(this.Handle, Convert.ToInt32(Win32API.WindowMessages.WM_SYSCOMMAND), Convert.ToInt32(Win32API.WindowMenuMessage.SC_MINIMIZE), 0);

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
            try
            {
                _testJobThread.Resume();
            }
            catch (Exception e)
            {
                AutoTesterErrorMsgBox("Error: Can not start testing: " + e.Message);
            }
        }

        private void MonitorStop()
        {
            try
            {
                _testJobThread.Abort();
            }
            catch
            {

            }

        }

        private void MonitorPause()
        {
            try
            {
                _testJobThread.Suspend();
            }
            catch (Exception e)
            {
                AutoTesterErrorMsgBox("Error: Can not pause testing: " + e.Message);

            }
        }

        private void MonitorHighlight()
        {

        }


        #endregion



        #region private helper

        private void MainFrm_Load(object sender, EventArgs e)
        {

        }

        private void RunFramework()
        {
            if (this._projectConfigFile == null)
            {
                AutoTesterErrorMsgBox("Error: No project config file found.");
            }
            else
            {
                try
                {
                    StartMonitor();
                    _testJobThread = new Thread(new ThreadStart(StartTestJob));
                    _testJobThread.Start();
                }
                catch (Exception e)
                {
                    AutoTesterErrorMsgBox(e.ToString());
                }
            }
        }

        private void StartTestJob()
        {
            TestJob job = new TestJob();
            job.ProjectConfigFile = this._projectConfigFile;
            job.OnNewMsg += new TestJob._newMsgDelegate(_monitor.AddLog);
            job.StartTesting();
        }

        private void SetWindowSize(int width, int height)
        {
            try
            {
                Win32API.SendMessage(this.Handle, Convert.ToInt32(Win32API.WindowMessages.WM_SYSCOMMAND), Convert.ToInt32(Win32API.WindowMenuMessage.SC_RESTORE), 0);
                this.Size = new Size(width, height);
            }
            catch
            {

            }

        }

        private void LoadProjectConfigFile(string file)
        {
            try
            {
                AutoConfig cfg = AutoConfig.GetInstance();
                cfg.ProjectConfigFile = file;
                cfg.ParseConfigFile();

                this._driveFile = cfg.ProjectDriveFile;

                //init textbox
                this.tbProjectName.Text = cfg.ProjectName;
                this.tbScreenPrint.Text = cfg.ScreenPrintDir;
                this.tbDriveFile.Text = cfg.ProjectDriveFile;
                this.tbLogFolder.Text = cfg.LogDir;
                this.tbLogTemplate.Text = cfg.LogTemplate;
                for (int i = 0; i < this.cbProjectDomain.Items.Count; i++)
                {
                    if (this.cbProjectDomain.Items[i].ToString().ToUpper() == cfg.ProjectDomain.ToUpper())
                    {
                        this.cbProjectDomain.SelectedIndex = i;
                        break;
                    }
                }

                if (cfg.IsHighlight)
                {
                    this.cbHighlight.Checked = true;
                }
                else
                {
                    this.cbHighlight.Checked = false;
                }

            }
            catch (Exception e)
            {
                AutoTesterErrorMsgBox("Error: Can not parse config file: " + e.ToString());
            }

        }

        private void ClearProjectSettings()
        {
            this.tbProjectName.Text = "";
            this.tbScreenPrint.Text = "";
            this.tbDriveFile.Text = "";
            this.tbLogFolder.Text = "";
            this.tbLogTemplate.Text = "";
            this.cbProjectDomain.SelectedIndex = 0;
            this.cbHighlight.Checked = false;
        }

        private void AutoTesterErrorMsgBox(string message)
        {
            MessageBox.Show(message, "AutoTester", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string GetProjectConfigStringFromWindow()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            sb.Append(" <Project>\n");
            sb.Append("  <ProjectName>" + this.tbProjectName.Text + "</ProjectName>\n");
            if (String.IsNullOrEmpty(this.cbProjectDomain.SelectedText))
            {
                sb.Append("  <ProjectDomain>HTML</ProjectDomain>\n");
            }
            else
            {
                sb.Append("  <ProjectDomain>" + this.cbProjectDomain.SelectedText + "</ProjectDomain>\n");
            }
            sb.Append("  <DriveFile>" + this.tbDriveFile.Text + "</DriveFile>\n");
            sb.Append("  <Log>" + this.tbLogFolder.Text + "</Log>\n");
            sb.Append("  <LogTemplate>" + this.tbLogTemplate.Text + "</LogTemplate>\n");
            sb.Append("  <ScreenPrint>" + this.tbScreenPrint.Text + "</ScreenPrint>\n");
            if (this.cbHighlight.Checked == true)
            {
                sb.Append("  <HighLight>Yes</HighLight>\n");
            }
            else
            {
                sb.Append("  <HighLight>No</HighLight>\n");
            }
            sb.Append(" </Project>\n");

            return sb.ToString();

        }

        private void SaveConfigFile(string configStr)
        {

            TextWriter configWriter = File.CreateText(this._projectConfigFile);
            configWriter.Write(configStr);
            configWriter.Flush();
            configWriter.Close();

        }

        #endregion



        #region action methods

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Project Config File|*.xml";
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this._projectConfigFile = this.openFileDialog1.FileName;
                LoadProjectConfigFile(this._projectConfigFile);
            }

        }

        private void printToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RunFramework();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Warning: Clear Project settings?", "AutoTester", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ClearProjectSettings();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "Project Config File|*.xml";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this._projectConfigFile = this.saveFileDialog1.FileName;
                    string configStr = GetProjectConfigStringFromWindow();
                    SaveConfigFile(configStr);
                }
                catch (Exception ex)
                {
                    AutoTesterErrorMsgBox("Can not save config file: " + ex.Message);
                }
            }
        }

        private void OnSelectIndexChanged(object sender, EventArgs e)
        {
            int index = this.tabProject.SelectedIndex;
            if (index == 0) //if the first tab, then resize it to the origin size. .
            {
                SetWindowSize(400, 300);
            }
            else if (index == 1)
            {

            }
        }

        private void btnOpenDriveFile_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClearProjectSettings();
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this._projectConfigFile))
                {
                    String configStr = GetProjectConfigStringFromWindow();
                    if (File.Exists(this._projectConfigFile))
                    {
                        File.Delete(this._projectConfigFile);
                    }

                    SaveConfigFile(configStr);
                }
            }
            catch (Exception ex)
            {
                AutoTesterErrorMsgBox(ex.Message);
            }

        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnSave_Click(sender, e);
        }

        #endregion







    }
}