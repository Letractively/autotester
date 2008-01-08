﻿/********************************************************************
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

namespace Shrinerain.AutoTester.Core
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

        //return the handle of the application.
        public IntPtr Handle
        {
            get
            {
                return this._appProcess.MainWindowHandle;
            }
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
                throw new CannotStartAppException("Can not find application: " + appFullPath);
            }

            string arg = "";
            if (parameters != null && parameters.Length > 0)
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
                throw new CannotStartAppException("Can not start test application: " + appFullPath + " with parameters: " + arg);
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
                    throw new CannotStopAppException("Can not stop test application: " + e.Message);
                }
            }
        }

        public virtual void Move(int x, int y)
        {
            //Win32API
        }

        public virtual void Resize(int left, int top, int width, int height)
        {
            try
            {
                Win32API.SetWindowPos(this.Handle, IntPtr.Zero, left, top, width, height, 0);
            }
            catch (Exception e)
            {
                throw new CannotResizeAppException("Can not resize application to " + left.ToString() + "," + top.ToString() + "," + width.ToString() + "," + height.ToString() + ": " + e.Message);
            }

        }

        public virtual void Max()
        {
            //try
            //{

            //}
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
            try
            {
                IntPtr currentActiveWindow = Win32API.GetActiveWindow();

                return currentActiveWindow == this.Handle ? true : false;
            }
            catch (Exception e)
            {
                throw new CannotGetAppStatusException("Can not determine if test app is active.");
            }
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
            return this._top;
        }

        public virtual int GetLeft()
        {
            return this._left;
        }

        public virtual int GetHeight()
        {
            return this._height;
        }

        public virtual int GetWidth()
        {
            return this._width;
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
            return this._appProcess.Id;
        }

        public virtual Process GetProcess()
        {
            return this._appProcess;
        }

        public virtual int GetThreadCount()
        {
            return this._appProcess.Threads.Count;
        }

        #endregion

        #region performance information

        public virtual long GetCPUTime()
        {
            return this._appProcess.TotalProcessorTime.Milliseconds;
        }

        public virtual long GetMemory()
        {
            return this._appProcess.WorkingSet64;
        }

        public virtual long GetIORead()
        {
            return 0;
        }

        public virtual long GetIOWrite()
        {
            return 0;
        }

        #endregion

        #region other information

        public virtual string GetAppName()
        {
            if (!String.IsNullOrEmpty(this._appName))
            {
                return this._appName;
            }
            else
            {
                return this._appProcess.ProcessName;
            }

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

            Win32API.Rect rect = new Win32API.Rect();

            try
            {
                Win32API.GetWindowRect(this.Handle, ref rect);

                this._left = rect.left;
                this._top = rect.top;
                this._width = rect.Width;
                this._height = rect.Height;
            }
            catch (Exception e)
            {
                throw new CannotGetAppInfoException("Can not get size of test app: " + e.Message);
            }
        }


        protected virtual void TerminateProcess(int processID)
        {

        }

        #endregion

        #endregion
    }
}
