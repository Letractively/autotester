/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: LogEngine.cs
*
* Description: LogEngine will generate log at runtime for framework.
*              LogEngine call TestLog class to write the actual log. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.IO;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class LogEngine
    {

        #region fields

        //current log file to use.
        private string _logFile;

        //test log to write log
        private TestLog _myLog;

        private AutoConfig _config = AutoConfig.GetInstance();


        //information
        private string _testStep;
        private string _exception;
        private string _testResult;


        #endregion

        #region properties

        public string TestStepInfo
        {
            get { return _testStep; }
            set { _testStep = value; }
        }

        public string ExceptionInfo
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public string TestResultInfo
        {
            get { return _testResult; }
            set { _testResult = value; }
        }

        public string LogFile
        {
            get { return _logFile; }
            set { _logFile = value; }
        }

        #endregion

        #region methods

        #region ctor

        public LogEngine()
        {

        }

        public LogEngine(string logFile)
        {
            if (string.IsNullOrEmpty(LogFile))
            {
                this._logFile = logFile;
            }
        }

        ~LogEngine()
        {
            Close();
        }

        #endregion

        #region public methods

        public void Close()
        {
            this._myLog.Close();
        }

        /* void WriteLog()
         * WriteLog will replace actual value with the parameters of TestLog.
         */
        public void WriteLog()
        {
            WriteLog("");
        }

        public void WriteLog(string message)
        {
            Init();

            this._myLog.TestStep = this._testStep;
            this._myLog.TestResultInfo = this._testResult;
            this._myLog.Exception = this._exception;
            this._myLog.UserData = message;

            this._myLog.WriteLog();

        }

        public void Clear()
        {
            this._testStep = null;
            this._testResult = null;
            this._exception = null;
        }

        /* void SaveScreenPrint()
         * Save screen print for check point.
         */
        public void SaveScreenPrint()
        {
            Init();

            if (!Directory.Exists(this._config.ScreenPrintDir))
            {
                try
                {
                    Directory.CreateDirectory(this._config.ScreenPrintDir);
                }
                catch (Exception e)
                {
                    throw new CannotSaveScreenPrintException("Can not get folder of screen print: " + e.Message);
                }
            }

            SaveScreenPrint(GenerateScreenPrintFileName());
        }

        public void SaveScreenPrint(string fileName)
        {
            this._myLog.SaveScreenPrint(fileName);
        }

        #endregion

        #region private methods

        /* void Init()
         * Init TestLog parameters.
         */
        private void Init()
        {
            if (this._myLog == null)
            {
                this._myLog = new TestLog();
                this._myLog.TestlogTemplate = _config.LogTemplate;

                if (!Directory.Exists(this._config.LogDir))
                {
                    Directory.CreateDirectory(this._config.LogDir);
                }

                if (string.IsNullOrEmpty(this._logFile))
                {
                    this._logFile = GenerateLogFileName();
                }

                this._myLog.LogFile = this._logFile;

                this._myLog.ProjectName = this._config.ProjectName;
            }

        }

        /* string GenerateLogFileName()
         * return the log file name.
         * the log name is "project name" +"_"+"timestamp".
         */
        private string GenerateLogFileName()
        {
            try
            {
                StringBuilder fileName = new StringBuilder();
                fileName.Append(this._config.LogDir);
                fileName.Append("\\");
                fileName.Append(this._config.ProjectName);
                fileName.Replace(@"\\\", @"\\");
                fileName.Replace(" ", "");
                fileName.Append("_");
                fileName.Append(GenerateTimeStamp() + ".htm");
                return fileName.ToString();
            }
            catch (Exception e)
            {
                throw new CannotWriteLogException("Can not generate log file name: " + e.Message);
            }
        }

        private string GenerateScreenPrintFileName()
        {
            try
            {
                StringBuilder fileName = new StringBuilder();
                fileName.Append(this._config.ScreenPrintDir);
                fileName.Append("\\");
                fileName.Append(this._config.ProjectName);
                fileName.Replace(@"\\\", @"\\");
                fileName.Replace(" ", "");
                fileName.Append("_");
                fileName.Append(GenerateTimeStamp() + ".jpg");
                return fileName.ToString();
            }
            catch (Exception e)
            {
                throw new CannotSaveScreenPrintException("Can not generate screen print file name: " + e.Message);
            }
        }

        /* string GenerateTimeStamp()
         * return time string for log file name and screen print file name.
         */
        private static string GenerateTimeStamp()
        {
            return DateTime.Now.ToShortDateString().Replace("/", "") + DateTime.Now.ToShortTimeString().Replace(":", "");
        }

        #endregion

        #endregion

    }
}
