/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestApp.cs
*
* Description: This class manage desktop application. It implements
*              ITestApp interface.  
*
* History: 2007/11/20 wan,yu Init version.
*          2008/01/15 wan,yu update, add Wait() methods. 
*
*********************************************************************/


using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
    public class TestApp : ITestApp
    {

        #region fields

        protected IntPtr _rootHandle;

        //process to start the desktop application
        protected Process _appProcess;

        protected string _appPath;
        protected string _appName;
        protected string _appVersion;
        protected string _appCompany;
        protected string _appAuthor;

        //size
        protected int _top;
        protected int _left;
        protected int _width;
        protected int _height;

        //max wait time is 120s.
        protected const int _maxWaitSeconds = 120;
        protected const int _interval = 3;

        //sync event
        protected AutoResetEvent _appStartEvent = new AutoResetEvent(false);

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
                return this._rootHandle;
            }
        }

        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region ITestApp Members

        #region operations

        /* void Start(string appFullPath)
         * Create a process to start a desktop application.
         */
        public virtual void Start(string appFullPath)
        {
            Start(appFullPath, null);
        }

        public virtual void Start(string appFullPath, string[] parameters)
        {
            if (!File.Exists(appFullPath))
            {
                throw new CannotStartAppException("Can not find test application: " + appFullPath);
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
            startInfo.UseShellExecute = false;

            //process to start application.
            _appProcess = new Process();
            _appProcess.StartInfo = startInfo;


            //if not sucessful
            if (_appProcess.Start())
            {
                int times = 0;
                while (times < _maxWaitSeconds)
                {
                    Thread.Sleep(_interval * 1000);
                    times += _interval;

                    //get the main handle.
                    this._rootHandle = _appProcess.MainWindowHandle;
                    if (this._rootHandle != IntPtr.Zero)
                    {
                        break;
                    }
                }

                if (this._rootHandle != IntPtr.Zero)
                {
                    return;
                }
            }

            throw new CannotStartAppException("Can not start test application: " + appFullPath + " with parameters: " + arg);
        }

        /* void Find(IntPtr handle)
         * Find a window by it's handle.
         */
        public virtual void Find(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new AppNotFoundExpcetion("Handle can not be 0.");
            }
            else
            {
                this._rootHandle = handle;
            }
        }

        public virtual void Find(String caption, String className)
        {
            if (!String.IsNullOrEmpty(caption) || !String.IsNullOrEmpty(className))
            {
                IntPtr handle = Win32API.FindWindow(className, caption);

                if (handle != IntPtr.Zero)
                {
                    Find(handle);
                }
                else
                {
                    throw new AppNotFoundExpcetion("Can not find window by caption and class.");
                }
            }
            else
            {
                throw new AppNotFoundExpcetion("Caption and class can not be null.");
            }
        }

        /* void Close()
         * Close a desktop application, kill the process.
         */
        public virtual void Close()
        {
            if (_appProcess != null)
            {
                try
                {
                    _appProcess.Close();
                    _appProcess = null;
                }
                catch (Exception ex)
                {
                    throw new CannotStopAppException("Can not stop test application: " + ex.Message);
                }
            }
        }

        public virtual void Move(int x, int y)
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotMoveAppException("Handle can not be 0.");
            }
            try
            {
                GetSize();
                Win32API.SetWindowPos(this._rootHandle, IntPtr.Zero, x, y, this._width, this._height, 0);
            }
            catch (Exception ex)
            {
                throw new CannotMoveAppException("Can not move window: " + ex.Message);
            }

        }

        public virtual void Resize(int left, int top, int width, int height)
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotResizeAppException("Handle can not be 0.");
            }

            try
            {
                GetSize();

                Win32API.SetWindowPos(this.Handle, IntPtr.Zero, left, top, width, height, 0);
            }
            catch (Exception ex)
            {
                throw new CannotResizeAppException("Can not resize application to " + left.ToString() + "," + top.ToString() + "," + width.ToString() + "," + height.ToString() + ": " + ex.Message);
            }

        }

        public virtual void Max()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotMaxAppException("Handle can not be 0.");
            }

            try
            {
                Win32API.PostMessage(this.Handle, Convert.ToInt32(Win32API.WindowMessages.WM_SYSCOMMAND), Convert.ToInt32(Win32API.WindowMenuMessage.SC_MAXIMIZE), 0);
            }
            catch (Exception ex)
            {
                throw new CannotMaxAppException("Can not max size test application: " + ex.Message);
            }
        }

        public virtual void Min()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotResizeAppException("Handle can not be 0.");
            }


            throw new NotImplementedException();
        }

        public virtual void Restore()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotResizeAppException("Handle can not be 0.");
            }

            try
            {
                Win32API.ShowWindow(this._rootHandle, (int)Win32API.ShowWindowCmds.SW_SHOWNORMAL);
            }
            catch (Exception ex)
            {
                throw new CannotResizeAppException("Can not restore window: " + ex.Message);
            }

        }

        public virtual void Active()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotActiveAppException("Handle can not be 0.");
            }

            try
            {
                Win32API.SetForegroundWindow(this._rootHandle);
            }
            catch (Exception ex)
            {
                throw new CannotActiveAppException("Can not active test application: " + ex.Message);
            }
        }

        public virtual void Wait(int seconds)
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotWaitAppException("Handle can not be 0.");
            }

            try
            {
                if (seconds > 0)
                {
                    Thread.Sleep(seconds * 1000);
                }
            }
            catch (Exception ex)
            {
                throw new CannotWaitAppException("Can not wait test application: " + ex.Message);
            }
        }

        public virtual void WaitForExist()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotWaitAppException("Handle can not be 0.");
            }

            try
            {
                int times = 0;
                while (times <= _maxWaitSeconds)
                {
                    if (Win32API.IsWindowVisible(this._rootHandle))
                    {
                        return;
                    }
                    else
                    {
                        Thread.Sleep(_interval * 1000);
                        times += _interval;
                    }
                }

                throw new AppNotFoundExpcetion();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppNotFoundExpcetion("Can not find test app: " + ex.Message);
            }

        }

        public virtual void WaitForDisappear()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotWaitAppException("Handle can not be 0.");
            }

            try
            {
                int times = 0;
                while (times <= _maxWaitSeconds)
                {
                    if (!Win32API.IsWindowVisible(this._rootHandle))
                    {
                        return;
                    }
                    else
                    {
                        Thread.Sleep(_interval * 1000);
                        times += _interval;
                    }
                }

                throw new CannotStopAppException();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotStopAppException("Test app is still existed: " + ex.Message);
            }
        }

        #endregion

        #region status

        public virtual bool IsActive()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                IntPtr currentActiveWindow = Win32API.GetActiveWindow();

                return currentActiveWindow == this._rootHandle;
            }
            catch (Exception ex)
            {
                throw new CannotGetAppStatusException("Can not determine if test app is active: " + ex.Message);
            }
        }

        public virtual bool IsTopMost()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                IntPtr currentTopWindow = Win32API.GetForegroundWindow();

                return currentTopWindow == this._rootHandle;
            }
            catch (Exception ex)
            {
                throw new CannotGetAppStatusException("Can not determine if test app is active: " + ex.Message);
            }
        }

        public virtual bool IsVisible()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                return Win32API.IsWindowVisible(this._rootHandle);
            }
            catch (Exception ex)
            {
                throw new CannotGetAppStatusException("Can not determine if test app is active: " + ex.Message);
            }
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

        protected virtual void WaitForApp()
        {
            if (this._appProcess == null)
            {
                throw new AppNotFoundExpcetion("Process is null.");
            }

            int times = 0;
            while (times < _maxWaitSeconds && !this._appProcess.Responding)
            {
                Thread.Sleep(_interval * 1000);
                times += _interval;
            }

            if (times >= _maxWaitSeconds)
            {
                throw new CannotWaitAppException("Wait for test app timeout.");
            }
        }

        protected virtual void WaitForApp(String title)
        {

        }

        protected virtual void WaitForApp(string title, int seconds)
        {

        }

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
            catch (Exception ex)
            {
                throw new CannotGetAppInfoException("Can not get size of test app: " + ex.Message);
            }
        }


        protected virtual void TerminateProcess(int processID)
        {
            if (processID <= 0)
            {
                Process curProcess = Process.GetProcessById(processID);

                if (curProcess != null)
                {
                    curProcess.Kill();
                    return;
                }
            }

            throw new CannotStopAppException("Can not find process by ID:" + processID);
        }

        #endregion

        #endregion
    }
}
