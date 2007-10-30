using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class TestJob
    {

        #region fields

        public delegate void _newMsgDelegate(string message);
        public event _newMsgDelegate OnNewMsg;

        private string _projectConfigFile;
        private string _frameworkConfigFile;

        #endregion

        #region properties
        public string ProjectConfigFile
        {
            get { return _projectConfigFile; }
            set
            {
                if (value != null)
                {
                    _projectConfigFile = value;
                }
            }
        }

        public string FrameworkConfigFile
        {
            get { return _frameworkConfigFile; }
            set
            {
                if (value != null)
                {
                    _frameworkConfigFile = value;
                }
            }
        }

        #endregion

        #region methods

        #region ctor

        public TestJob()
        {
        }

        #endregion

        #region public methods

        public void StartTesting()
        {
            AutoConfig autoConfig = AutoConfig.GetInstance();
            autoConfig.ProjectConfigFile = this._projectConfigFile;
            if (!String.IsNullOrEmpty(this._frameworkConfigFile))
            {
                autoConfig.FrameworkConfigFile = this._frameworkConfigFile;
            }
            autoConfig.ParseConfigFile();
            autoConfig.Close();

            Parser parser = Parser.GetInstance();
            parser.AutoConfig = autoConfig;
            parser.ParseDriveFile();
            parser.Close();

            CoreEngine coreEngine = new CoreEngine();
            coreEngine.AutoConfig = autoConfig;
            coreEngine.KeywordParser = parser;
            coreEngine.OnNewMessage += new CoreEngine._frameworkInfoDelegate(DeliverNewMsg);
            coreEngine.Start();
        }

        #endregion

        #region private methods

        private bool CheckConfigFile()
        {
            return File.Exists(_projectConfigFile);
        }

        private void DeliverNewMsg(string message)
        {
            OnNewMsg(message);
        }


        #endregion

        #endregion

    }
}
