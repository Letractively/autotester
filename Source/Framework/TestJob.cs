/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestJob.cs
*
* Description: This is the entry point of Automation Framework.
*              TestJob will do some init work, create an instance of
*              CoreEngine to do real testing.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class TestJob
    {

        #region fields

        //event to deliver message to other program
        public delegate void _newMsgDelegate(string message);
        public event _newMsgDelegate OnNewMsg;

        //we have two config file, one is for project, one is for framework.
        private string _projectConfigFile;
        private string _frameworkConfigFile;

        private AutoConfig _autoConfig;
        private Parser _parser;
        private CoreEngine _coreEngine;

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

        public TestJob(string projectConfigFile)
        {
            this._projectConfigFile = projectConfigFile;
        }

        public TestJob(string frameworkConfigFile, string projectConfigFile)
        {
            this._frameworkConfigFile = frameworkConfigFile;
            this._projectConfigFile = projectConfigFile;
        }

        #endregion

        #region public methods

        /* void StartTesting()
         * Do some init job, and call CoreEngine to start testing.
         */
        public void StartTesting()
        {

            InitProject();

            _coreEngine = new CoreEngine();

            //deliver message while testing.
            //we can get the runtime message from CoreEngine
            _coreEngine.OnNewMessage += new CoreEngine._frameworkInfoDelegate(DeliverNewMsg);
            _coreEngine.AutoConfig = _autoConfig;
            _coreEngine.KeywordParser = _parser;

            //start testing.
            _coreEngine.Start();
        }

        #endregion

        #region private methods

        /* void InitProject()
         * Init AutoConfig and Parser to parse config file and driver file.
         * 
         */
        private void InitProject()
        {

            _autoConfig = AutoConfig.GetInstance();
            _autoConfig.ProjectConfigFile = this._projectConfigFile;

            //if user input the framework config file, we will use it,
            // or we will search the framework config file in current folder.
            if (!String.IsNullOrEmpty(this._frameworkConfigFile))
            {
                _autoConfig.FrameworkConfigFile = this._frameworkConfigFile;
            }

            _autoConfig.ParseConfigFile();
            _autoConfig.Close();

            _parser = Parser.GetInstance();
            _parser.AutoConfig = _autoConfig;

            _parser.ParseDriveFile();
            _parser.Close();
        }


        /* void DeliverNewMsg(string message)
         * deliver message to other compent.
         */
        private void DeliverNewMsg(string message)
        {
            if (OnNewMsg != null)
            {
                OnNewMsg(message);
            }

        }


        #endregion

        #endregion

    }
}
