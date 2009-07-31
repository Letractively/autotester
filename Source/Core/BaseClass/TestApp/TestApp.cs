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

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Core
{
    public class TestApp : ITestApp
    {
        #region fields

        protected IntPtr _rootHandle = IntPtr.Zero;
        protected Process _appProcess;
        protected TestWindow _currentWindow;
        protected bool _isHide;

        protected string _appPath;
        protected string _appArgs;
        protected string _appName;
        protected string _appVersion;
        protected string _appCompany;
        protected string _appAuthor;

        //size
        protected int _top;
        protected int _left;
        protected int _width;
        protected int _height;

        //max wait time is 30.
        protected int _appTimeout = 60;
        protected const int Interval = 2;

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

        public int AppTimeout
        {
            get { return _appTimeout; }
            set { _appTimeout = value; }
        }

        public string AppArgs
        {
            get { return _appArgs; }
            set { _appArgs = value; }
        }

        //return the handle of the application.
        public IntPtr Handle
        {
            get { return this._rootHandle; }
        }

        public virtual ITestWindow CurrentWindow
        {
            get { return this._currentWindow; }
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

        public virtual void Start(string appFullPath, string parameters)
        {
            try
            {
                BeforeStart();

                if (!File.Exists(appFullPath))
                {
                    throw new CannotStartAppException("Can not find test application: " + appFullPath);
                }

                this._appArgs = parameters;

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = appFullPath;
                startInfo.Arguments = parameters;
                if (_isHide)
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }

                //process to start application.
                _appProcess = new Process();
                _appProcess.StartInfo = startInfo;

                //if not sucessful
                if (!_appProcess.Start())
                {
                    throw new CannotStartAppException("Can not start test application: " + appFullPath + " with parameters: " + parameters);
                }
                else
                {
                    int times = 0;
                    while (times < _appTimeout)
                    {
                        Thread.Sleep(Interval * 1000);
                        times += Interval;

                        if (_appProcess.MainWindowHandle != IntPtr.Zero)
                        {
                            //get the main handle.
                            this._rootHandle = _appProcess.MainWindowHandle;
                            break;
                        }
                    }

                    if (this._rootHandle == IntPtr.Zero)
                    {
                        throw new CannotStartAppException("Can not start application by path:" + appFullPath + " with parameters:" + parameters);
                    }
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotStartAppException("Can not start application by path:" + appFullPath + " with parameters:" + parameters + ": " + ex.ToString());
            }
            finally
            {
                AfterStart();
            }
        }

        //attach to an exist application
        public virtual void Find(IntPtr handle)
        {
            try
            {
                BeforeFound();

                if (handle != IntPtr.Zero)
                {
                    this._rootHandle = handle;
                    GetProcessByHandle(this._rootHandle);
                }
                else
                {
                    throw new AppNotFoundExpcetion("Can not find application by handle:" + handle);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppNotFoundExpcetion("Can not find applicatoin by handle:" + ex.ToString());
            }
            finally
            {
                AfterFound();
            }
        }

        public virtual void Find(string caption, string className)
        {
            IntPtr handle = Win32API.FindWindow(className, caption);
            Find(handle);
        }

        public virtual void Find(String processName, int index)
        {
            IntPtr handle = IntPtr.Zero;
            if (!String.IsNullOrEmpty(processName) && index >= 0)
            {
                foreach (Process p in System.Diagnostics.Process.GetProcessesByName(processName))
                {
                    if (String.Compare(p.ProcessName, processName, true) == 0)
                    {
                        handle = p.MainWindowHandle;
                        if (handle == IntPtr.Zero)
                        {
                            handle = p.Handle;
                        }
                        index--;
                        if (index < 0)
                        {
                            Find(handle);
                            return;
                        }
                    }
                }
            }
        }

        public virtual void Find(String title)
        {
            Find(title, null);
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
                    ITestEventDispatcher dispatcher = this.GetEventDispatcher();
                    if (dispatcher != null)
                    {
                        dispatcher.Stop();
                        dispatcher = null;
                    }

                    _appProcess.Close();
                    _appProcess = null;
                }
                catch (Exception ex)
                {
                    throw new CannotStopAppException("Can not stop test application: " + ex.ToString());
                }
            }
        }


        public virtual ITestWindow[] AllWindows()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ITestWindow Window(int index)
        {
            throw new NotImplementedException();
        }

        public ITestWindow Window(IntPtr handle)
        {
            throw new NotImplementedException();
        }

        public ITestWindow Window(string title, string className)
        {
            throw new NotImplementedException();
        }

        public int GetWindowIndex(ITestWindow window)
        {
            return -1;
        }

        public virtual void Move(int x, int y)
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotMoveAppException("Handle can not be 0.");
            }
            try
            {
                GetRectOnScreen();
                Win32API.SetWindowPos(this._rootHandle, IntPtr.Zero, x, y, this._width, this._height, 0);
            }
            catch (Exception ex)
            {
                throw new CannotMoveAppException("Can not move window: " + ex.ToString());
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
                GetRectOnScreen();
                Win32API.SetWindowPos(this.Handle, IntPtr.Zero, left, top, width, height, 0);
            }
            catch (Exception ex)
            {
                throw new CannotResizeAppException("Can not resize application to " + left.ToString() + "," + top.ToString() + "," + width.ToString() + "," + height.ToString() + ": " + ex.ToString());
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
                Win32API.ShowWindow(this._rootHandle, (int)Win32API.ShowWindowCmds.SW_SHOWMAXIMIZED);
            }
            catch (Exception ex)
            {
                throw new CannotMaxAppException("Can not max size test application: " + ex.ToString());
            }
        }

        public virtual void Min()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotResizeAppException("Handle can not be 0.");
            }

            try
            {
                Win32API.ShowWindow(this._rootHandle, (int)Win32API.ShowWindowCmds.SW_SHOWMINIMIZED);
            }
            catch (Exception ex)
            {
                throw new CannotMaxAppException("Can not max size test application: " + ex.ToString());
            }
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
                throw new CannotResizeAppException("Can not restore window: " + ex.ToString());
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
                Win32API.SetActiveWindow(this._rootHandle);
            }
            catch (Exception ex)
            {
                throw new CannotActiveAppException("Can not active test application: " + ex.ToString());
            }
        }

        public virtual void Hide(bool hide)
        {
            _isHide = hide;

            if (this._rootHandle != IntPtr.Zero)
            {
                try
                {
                    if (hide)
                    {
                        Win32API.ShowWindow(this._rootHandle, (int)Win32API.ShowWindowCmds.SW_HIDE);
                    }
                    else
                    {
                        Win32API.ShowWindow(this._rootHandle, (int)Win32API.ShowWindowCmds.SW_SHOW);
                    }
                }
                catch (Exception ex)
                {
                    throw new CannotActiveAppException("Can not hide/unhide test application: " + ex.ToString());
                }
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
                throw new CannotWaitAppException("Can not wait test application: " + ex.ToString());
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
                while (times <= _appTimeout)
                {
                    if (Win32API.IsWindowVisible(this._rootHandle))
                    {
                        return;
                    }
                    else
                    {
                        Thread.Sleep(Interval * 1000);
                        times += Interval;
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
                throw new AppNotFoundExpcetion("Can not find test app: " + ex.ToString());
            }
        }

        public virtual void WaitForClose()
        {
            if (this._rootHandle == IntPtr.Zero)
            {
                throw new CannotWaitAppException("Handle can not be 0.");
            }

            try
            {
                int times = 0;
                while (times <= _appTimeout)
                {
                    if (!Win32API.IsWindowVisible(this._rootHandle))
                    {
                        return;
                    }
                    else
                    {
                        Thread.Sleep(Interval * 1000);
                        times += Interval;
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
                throw new CannotStopAppException("Test app is still existed: " + ex.ToString());
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
                throw new CannotGetAppStatusException("Can not determine if test app is active: " + ex.ToString());
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
                throw new CannotGetAppStatusException("Can not determine if test app is active: " + ex.ToString());
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
                throw new CannotGetAppStatusException("Can not determine if test app is active: " + ex.ToString());
            }
        }

        #endregion

        #region size

        public System.Drawing.Rectangle GetRectOnScreen()
        {
            try
            {
                Win32API.Rect rect = new Win32API.Rect();
                Win32API.GetWindowRect(this.Handle, ref rect);

                this._left = rect.left;
                this._top = rect.top;
                this._width = rect.Width;
                this._height = rect.Height;

                return new System.Drawing.Rectangle(_left, _top, _width, _height);
            }
            catch (Exception ex)
            {
                throw new CannotGetAppInfoException("Can not get size of test app: " + ex.ToString());
            }
        }

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

        #endregion

        #endregion

        #endregion

        #region private methods

        protected virtual void WaitForApp()
        {

        }

        protected virtual void WaitForApp(Object title)
        {

        }

        protected virtual void WaitForApp(string title, int seconds)
        {

        }

        protected void GetProcessByHandle(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                foreach (Process p in System.Diagnostics.Process.GetProcesses())
                {
                    if (p.MainWindowHandle == handle)
                    {
                        this._appProcess = p;
                        return;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region ITestApp Members

        public event TestAppEventHandler OnBeforeAppStart;

        public event TestAppEventHandler OnAfterAppStart;

        public event TestAppEventHandler OnBeforeAppClose;

        public event TestAppEventHandler OnAfterAppClose;

        public event TestAppEventHandler OnBeforeAppFound;

        public event TestAppEventHandler OnAfterAppFound;


        protected virtual void BeforeStart()
        {
            if (OnBeforeAppStart != null)
                OnBeforeAppStart(this, null);
        }

        protected virtual void AfterStart()
        {
            if (OnAfterAppStart != null)
                OnAfterAppStart(this, null);
        }

        protected virtual void BeforeFound()
        {
            if (OnBeforeAppFound != null)
                OnBeforeAppFound(this, null);
        }

        protected virtual void AfterFound()
        {
            if (OnAfterAppFound != null)
                OnAfterAppFound(this, null);
        }

        protected virtual void BoforeClose()
        {
            if (OnBeforeAppClose != null)
                OnBeforeAppClose(this, null);
        }

        protected virtual void AfterClose()
        {
            if (OnAfterAppClose != null)
                OnAfterAppClose(this, null);
        }

        public virtual ITestEventDispatcher GetEventDispatcher()
        {
            return null;
        }

        #endregion
    }
}
