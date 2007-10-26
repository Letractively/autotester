using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using SHDocVw;
using mshtml;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Function
{
    public class TestBrowser : IDisposable, ITestBrowser
    {
        #region Fileds

        protected static TestBrowser _testBrowser;

        //Handle of IE.
        protected static IntPtr _mainHandle;

        //handle of client area.
        protected static IntPtr _ieServerHandle;

        //handle of shell doc.
        protected static IntPtr _shellDocHandle;

        protected InternetExplorer _ie = null;
        protected HTMLDocument _HTMLDom = null;
        protected HTMLBody bodyElement = null;

        protected int _maxWaitSeconds = 120; //wait for 120 secs
        protected const int _interval = 3;   //every time sleep for 3 secs if IE is not found.


        // the IE window's rect
        protected static int _ieLeft;
        protected static int _ieTop;
        protected static int _ieWidth;
        protected static int _ieHeight;

        // the client area window's rect
        protected static int _clientTop;
        protected static int _clientLeft;
        protected static int _clientWidth;
        protected static int _clientHeight;

        //the web page's rect, may larger than client area because of scroll bar.
        protected static int _scrollLeft;
        protected static int _scrollTop;
        protected static int _scrollWidth;
        protected static int _scrollHeight;


        #endregion

        #region Sync Event

        protected AutoResetEvent _documentLoadComplete = new AutoResetEvent(false);
        protected AutoResetEvent _ieStarted = new AutoResetEvent(false);
        protected AutoResetEvent _ieExisted = new AutoResetEvent(false);

        #endregion

        #region Properties

        public int MaxWaitSeconds
        {
            get { return this._maxWaitSeconds; }
            set
            {
                if (value >= 0)
                {
                    this._maxWaitSeconds = value;
                }
            }
        }

        public static int Left
        {
            get
            {
                return _ieLeft;
            }
        }

        public static int Top
        {
            get
            {
                return _ieTop;
            }
        }

        public static int Width
        {
            get
            {
                return _ieWidth;
            }
        }

        public static int Height
        {
            get
            {
                return _ieHeight;
            }
        }

        public static int ClientTop
        {
            get
            {
                return _clientTop;
            }
        }

        public static int ClientLeft
        {
            get
            {
                return _clientLeft;
            }
        }

        public static int ClientWidth
        {
            get
            {
                return _clientWidth;
            }
        }

        public static int ClientHeight
        {
            get
            {
                return _clientHeight;
            }
        }

        public static int ScrollLeft
        {
            get
            {
                // GetScrollRect();
                return _scrollLeft;
            }
        }
        public static int ScrollTop
        {
            get
            {
                // GetScrollRect();
                return _scrollTop;
            }
        }
        public static int ScrollWidth
        {
            get
            {
                //GetScrollRect();
                return _scrollWidth;
            }
        }
        public static int ScrollHeight
        {
            get
            {
                // GetScrollRect();
                return _scrollHeight;
            }

        }

        //main handle of ie window
        public static IntPtr MainHandle
        {
            get { return _mainHandle; }
        }

        //handle of client area, Internet Explorer_Server
        public static IntPtr IEServerHandle
        {
            get { return _ieServerHandle; }
        }

        #endregion

        #region Methods

        protected TestBrowser()
        {

        }

        //singleton
        public static TestBrowser GetInstance()
        {
            if (_testBrowser == null)
            {
                _testBrowser = new TestBrowser();
            }

            return _testBrowser;
        }

        ~TestBrowser()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (this._ieExisted != null)
            {
                this._ieExisted.Close();
                this._ieStarted.Close();
                this._documentLoadComplete.Close();

                this._ieExisted = null;
                this._ieStarted = null;
                this._documentLoadComplete = null;
            }

            GC.SuppressFinalize(this);
        }

        #region public methods

        #region operate IE
        /*  bool Start(string url)
         * --------------------------------
         *  Start IE with the specific url and attach IE.
         *  Return true if IE started successfully, else return false;
         * -----------------------------------------------------------
         * 2007-09-12 Wan,Yu Init
         *  
         */
        public virtual void Start()
        {
            try
            {
                Process p = Process.Start("iexplore.exe");

                Thread ieExistT = new Thread(new ThreadStart(WaitForIEExist));
                ieExistT.Start();

                this._ieExisted.WaitOne();

                SHDocVw.ShellWindows allBrowsers = null;

                int j = 0;

                while (j < 3)
                {
                    allBrowsers = new ShellWindows();

                    if (allBrowsers.Count == 0)
                    {
                        throw new Exception("Error: Can not find Internet Explorer.");
                    }

                    for (int i = 0; i < allBrowsers.Count; i++)
                    {
                        InternetExplorer tempIE = null;
                        try
                        {
                            tempIE = (InternetExplorer)allBrowsers.Item(j++);
                            if (tempIE == null)
                            {
                                continue;
                            }
                            if (tempIE.HWND == (int)p.MainWindowHandle)
                            {
                                _ie = tempIE;

                                RegIEEvent();

                                _ieStarted.Set();

                                break;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    if (_ie != null)
                    {
                        break;
                    }
                    j++;

                    Thread.Sleep(_interval * 1000);

                }
            }
            catch
            {
                throw new CanNotStartTestBrowserException("Can not start Internet explorer");
            }

        }

        public virtual void Close()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            this._ie.Quit();
        }

        public virtual void Load(string url)
        {

            if (this._ie == null)
            {
                _ieStarted.WaitOne(this._maxWaitSeconds * 1000, true);
            }
            if (String.IsNullOrEmpty(url))
            {
                throw new CanNotLoadUrlException("Url can not be null.");
            }

            object tmp = new object();
            try
            {
                this._ie.Navigate(url, ref tmp, ref tmp, ref tmp, ref tmp);
                //Thread.Sleep(1 * 1000);
            }
            catch
            {
                throw new CanNotLoadUrlException();
            }

            this._documentLoadComplete.WaitOne(_maxWaitSeconds * 1000, true);

        }

        public virtual void Load(Uri url)
        {
            Load(url.ToString());
        }

        public virtual void Move(int top, int left)
        {

            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                this._ie.Top = top;
                this._ie.Left = left;
            }
            catch
            {
                throw new CanNotActiveTestBrowserException("Can not move IE.");
            }
        }

        public virtual void Resize(int width, int height)
        {
            if (width < 1)
            {
                width = 1;
            }
            if (height < 1)
            {
                height = 1;
            }
            try
            {
                this._ie.Width = width;
                this._ie.Height = height;
            }
            catch
            {
                throw new CanNotActiveTestBrowserException("Can not resize IE.");
            }
        }

        public virtual void Back()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                this._ie.GoBack();
            }
            catch
            {
                throw new CanNotNavigateException("Can not go back.");
            }

        }

        public virtual void Forward()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                this._ie.GoForward();
            }
            catch
            {
                throw new CanNotNavigateException("Can not go forward.");
            }
        }

        public virtual void Home()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                this._ie.GoHome();
            }
            catch
            {
                throw new CanNotNavigateException("Can not go home.");
            }

        }

        public virtual void Refresh()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                this._ie.Refresh();
            }
            catch
            {
                throw new CanNotNavigateException("Can not go home.");
            }
        }

        public virtual void Wait(int data)
        {
            if (data > 0)
            {
                Thread.Sleep(data * 1000);
            }
        }

        public virtual void WaitForNewWindow()
        {
            throw new CanNotPerformActionException("Can not wait for new window.");
        }

        public virtual void WaitForNewPage()
        {
            throw new CanNotPerformActionException("Can not wait for new page");
        }

        #endregion

        #region Get IE Information

        public virtual string GetCurrentUrl()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._ie.LocationURL;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not get the current url.");
            }
        }

        public virtual string GetStatusText()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._ie.StatusText;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not get the status text.");
            }
        }

        public virtual bool IsMenuVisiable()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._ie.MenuBar;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not get the menu status.");
            }
        }

        public virtual bool IsResizeable()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._ie.Resizable;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not get the resize status.");
            }
        }

        public virtual bool IsFullScreen()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._ie.FullScreen;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not the full screen status.");
            }

        }


        #endregion

        #region SYNC

        public virtual void WaitDocumentLoadComplete(int seconds)
        {
            if (seconds < 0)
            {
                seconds = 0;
            }
            _documentLoadComplete.WaitOne(seconds * 1000, true);
        }

        public virtual void WaitDocumentLoadComplete()
        {
            WaitDocumentLoadComplete(this._maxWaitSeconds);
        }

        public virtual void WaitIEStart(int secondes)
        {
            _ieStarted.WaitOne(secondes * 1000, true);
        }

        public virtual void WaitIEStart()
        {
            WaitIEStart(this._maxWaitSeconds);
        }

        #endregion


        #endregion

        #region protected virtual help methods

        //wait for 120 seconds max to detect if IE browser is started.
        protected virtual void WaitForIEExist(int senconds)
        {
            if (senconds < 0)
            {
                senconds = 0;
            }
            int curSleepTime = 0;
            while (curSleepTime <= senconds)
            {
                Thread.Sleep(_interval * 1000);
                curSleepTime += _interval;

                IntPtr ieHwd = IntPtr.Zero;
                ieHwd = Win32API.FindWindow("IEFrame", null);
                if (ieHwd != IntPtr.Zero)
                {
                    _mainHandle = ieHwd;
                    Thread.Sleep(_interval * 1000);
                    this._ieExisted.Set();
                    break;
                }

            }
        }

        protected virtual void WaitForIEExist()
        {
            WaitForIEExist(this._maxWaitSeconds);
        }

        //client rect: the web page rect, not include menu, address bar ect.
        protected virtual void GetClientRect()
        {
            if (_mainHandle != IntPtr.Zero)
            {
                if (_shellDocHandle == IntPtr.Zero)
                {
                    _shellDocHandle = Win32API.FindWindowEx(_mainHandle, IntPtr.Zero, "Shell DocObject View", null);
                    if (_shellDocHandle == IntPtr.Zero)
                    {
                        throw new TestBrowserNotFoundException("Can not get IE Client location.");
                    }
                }


                //determine the yellow warning bar, for example, if the web page contains ActiveX, we can see the yellow bar at the top of the web page.
                //if the warning bar exist, we need to add 20 height to each html control.
                IntPtr warnBar = Win32API.FindWindowEx(_shellDocHandle, IntPtr.Zero, "#32770 (Dialog)", null);
                int addHeight = 0;
                if (warnBar != IntPtr.Zero)
                {
                    Win32API.Rect warnRect = new Win32API.Rect();
                    Win32API.GetClientRect(warnBar, ref warnRect);
                    addHeight = warnRect.Height;
                }

                //Get the actual client area rect, which shows web page to the end user.
                if (_ieServerHandle == IntPtr.Zero)
                {
                    _ieServerHandle = Win32API.FindWindowEx(_shellDocHandle, IntPtr.Zero, "Internet Explorer_Server", null);

                    if (_ieServerHandle == IntPtr.Zero)
                    {
                        throw new TestBrowserNotFoundException("Can not get IE Client location.");
                    }
                }

                Win32API.Rect tmpRect = new Win32API.Rect();
                Win32API.GetWindowRect(_ieServerHandle, ref tmpRect);

                _clientLeft = tmpRect.left;
                _clientTop = tmpRect.top + addHeight;
                _clientWidth = tmpRect.Width;
                _clientHeight = tmpRect.Height;

            }
            else
            {
                throw new TestBrowserNotFoundException("Can not get IE Client location.");
            }
        }

        //browser rect, the whole rect of IE, include menu, address bar etc.
        protected virtual void GetBrowserRect()
        {
            if (this._ie == null)
            {
                throw new TestBrowserNotFoundException();
            }

            try
            {
                _ieLeft = this._ie.Left;
                _ieTop = this._ie.Top;
                _ieWidth = this._ie.Width;
                _ieHeight = this._ie.Height;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not get the rect.");
            }
        }

        //the whole web page rect, include invisible part, for example, some web pages are too long to display, we need to scroll them.
        protected virtual void GetScrollRect()
        {
            if (bodyElement == null)
            {
                bodyElement = (HTMLBody)this._HTMLDom.body;
            }

            try
            {
                _scrollWidth = bodyElement.scrollWidth;
                _scrollHeight = bodyElement.scrollHeight;

                // scrollLeft means the left that Can Not been seen, scrollTop the same.
                _scrollLeft = bodyElement.scrollLeft;
                _scrollTop = bodyElement.scrollTop;
            }
            catch
            {
                _scrollWidth = 0;
                _scrollHeight = 0;
                _scrollLeft = 0;
                _scrollTop = 0;
            }

        }

        protected virtual void GetSize()
        {
            GetBrowserRect();
            GetClientRect();
            GetScrollRect();
        }

        #endregion

        #region event

        //register IE events
        protected virtual void RegIEEvent()
        {

            RegDocumentLoadCompleteEvent();
            RegNavigateFailedEvent();
            RegRectChangeEvent();
            RegScrollEvent();

        }

        protected virtual void RegScrollEvent()
        {

            // HTMLBody body = (HTMLBody)_HTMLDom.body;
            //_HTMLDom.parentWindow.
            //body.s

        }

        //document/(html page) load complete
        protected virtual void RegDocumentLoadCompleteEvent()
        {
            try
            {
                _ie.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentLoadComplete);
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not register document complete event.");
            }
        }

        //resize the window
        protected virtual void RegRectChangeEvent()
        {
            try
            {
                _ie.WindowSetTop += new DWebBrowserEvents2_WindowSetTopEventHandler(OnRectChanged);
                _ie.WindowSetLeft += new DWebBrowserEvents2_WindowSetLeftEventHandler(OnRectChanged);
                _ie.WindowSetWidth += new DWebBrowserEvents2_WindowSetWidthEventHandler(OnRectChanged);
                _ie.WindowSetHeight += new DWebBrowserEvents2_WindowSetHeightEventHandler(OnRectChanged);
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not register Rect change event.");
            }
        }

        //load url failed
        protected virtual void RegNavigateFailedEvent()
        {
            try
            {
                _ie.NavigateError += new DWebBrowserEvents2_NavigateErrorEventHandler(OnNavigateError);
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not register load error event.");
            }
        }

        protected virtual void OnNavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            throw new CanNotLoadUrlException();
        }

        //when document load complete, we can start to operate the html controls
        protected virtual void OnDocumentLoadComplete(object pDesp, ref object pUrl)
        {
            try
            {
                this._HTMLDom = (HTMLDocument)_ie.Document;

                GetSize();
            }
            catch (TestBrowserNotFoundException)
            {
                throw;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not parse html document.");
            }
            _documentLoadComplete.Set();
        }

        //when the position or rect of ie is changed, we need to re-calculate the position of html controls.
        protected virtual void OnRectChanged(int size)
        {
            try
            {
                GetSize();
            }
            catch (TestBrowserNotFoundException)
            {
                throw;
            }
            catch
            {
                throw new CanNotAttachTestBrowserException("Can not register rect change event.");
            }
        }

        #endregion

        #endregion
    }
}
