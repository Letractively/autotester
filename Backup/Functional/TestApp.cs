/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestApp.cs
*
* Description: This class manage desktop application. It implement
*              ITestApp interface.  
*
* History: 2007/11/20 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Function
{
    public class TestApp : ITestApp
    {

        #region fields

        //process to start the desktop application
        Process _appProcess;

        string _appPath;
        string _appName;
        string _appVersion;
        string _appCompany;
        string _appAuthor;

        //size
        int _top;
        int _left;
        int _width;
        int _height;

        #endregion

        #region properties

        public string AppAuthor
        {
            get { return _appAuthor; }
            set { _appAuthor = value; }
        }

        public string AppCompany
        {
            get { return _appCompany; }
            set { _appCompany = value; }
        }

        public string AppVersion
        {
            get { return _appVersion; }
            set { _appVersion = value; }
        }

        public string AppName
        {
            get { return _appName; }
            set { _appName = value; }
        }

        public string AppPath
        {
            get { return _appPath; }
            set { _appPath = value; }
        }

        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region ITestApp Members

        #region operations

        public virtual void Start(string appFullPath)
        {
            Start(appFullPath, null);
        }

        public virtual void Start(string appFullPath, string[] parameters)
        {
            if (!File.Exists(appFullPath))
            {
                throw new CanNotStartAppException("Can not find application: " + appFullPath);
            }

            string arg = "";
            if (parameters != null)
            {
                foreach (string i in parameters)
                {
                    arg += (i + " ");
                }
            }


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = appFullPath;
            startInfo.Arguments = arg;

            //process to start application.
            _appProcess = new Process();
            _appProcess.StartInfo = startInfo;

            //if not sucessful
            if (!_appProcess.Start())
            {
                throw new CanNotStartAppException("Can not start test application: " + appFullPath + " with parameters: " + arg);
            }
        }

        public virtual void Close()
        {
            if (_appProcess != null)
            {
                try
                {
                    _appProcess.Close();
                    _appProcess = null;
                }
                catch (Exception e)
                {
                    throw new CanNotStopAppException("Can not stop test application: " + e.Message);
                }
            }
        }

        public virtual void Move(int x, int y)
        {
            throw new NotImplementedException();
        }

        public virtual void Resize(int left, int top, int width, int height)
        {
            throw new NotImplementedException();
        }

        public virtual void Max()
        {
            throw new NotImplementedException();
        }

        public virtual void Min()
        {
            throw new NotImplementedException();
        }

        public virtual void Restore()
        {
            throw new NotImplementedException();
        }

        public virtual void Active()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region status

        public virtual bool IsActive()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsTopMost()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsVisible()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsMax()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsMin()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsIcon()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsBusy()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsVisualStyle()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsTaskbar()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region size

        public virtual int GetTop()
        {
            throw new NotImplementedException();
        }

        public virtual int GetLeft()
        {
            throw new NotImplementedException();
        }

        public virtual int GetHeight()
        {
            throw new NotImplementedException();
        }

        public virtual int GetWidth()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region network information

        public virtual int[] GetPortNumber()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsConnected()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region process information

        public virtual int GetProcessID()
        {
            throw new NotImplementedException();
        }

        public virtual Process GetProcess()
        {
            return this._appProcess;
        }

        public virtual int GetThreadCount()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region performance information

        public virtual int GetCPUTime()
        {
            throw new NotImplementedException();
        }

        public virtual int GetMemory()
        {
            throw new NotImplementedException();
        }

        public virtual int GetIORead()
        {
            throw new NotImplementedException();
        }

        public virtual int GetIOWrite()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region other information

        public virtual string GetAppName()
        {
            return this._appName;
        }

        public virtual string GetVersion()
        {
            return this._appVersion;
        }

        public virtual string GetCompany()
        {
            return this._appCompany;
        }

        public virtual string GetAuthor()
        {
            return this._appAuthor;
        }

        #endregion

        #endregion

        #endregion

        #region private methods

        protected virtual void GetSize()
        {

        }


        protected virtual void TerminateProcess(int processID)
        {

        }

        #endregion

        #endregion
    }
}
