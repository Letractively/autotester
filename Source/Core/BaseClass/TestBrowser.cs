/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestBrowser.cs
*
* Description: TestBrowser support Internet Explorer. It implent 
*              ITestBrowser interface. You can use TestBrowser to 
*              interactive with Internet Explorer, and get the information
*              of Internet Explorer. 
*
* History: 2007/09/04 wan,yu Init version.
*          2007/12/26 wan,yu add void Find(object) method. 
*          2007/12/27 wan,yu rename WaitForIEExist to WaitForBrowser.     
*          2008/01/15 wan,yu update, modify some static members to instance.
*          2008/02/24 wan,yu update, add GetAllBrowsers() , GetTopmostBrowser() and SetBrowser().          
*
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Net;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Helper;

namespace Shrinerain.AutoTester.Core
{

    public class TestBrowser : IDisposable, ITestBrowser
    {
        #region Fileds

        //this struct is used to save the browser status.
        //Sometimes the web app will pop up a new window, it is a new browser, and we will switch to
        //the new browser, after operation finished, we need to go back to the origin browser.
        //so we need a stack to save the status, after pop window disappear, we need to pop the old status from the stack.


        //stack to save the browser status.
        protected Stack<InternetExplorer> _ieStack = new Stack<InternetExplorer>(5);

        protected TestBrowser _testBrowser;

        //InternetExplorer is under SHDocVw namespace, we use this to attach to a browser.
        protected InternetExplorer _ie;

        protected Process _browserProcess;

        //Handle of IE, class name is "IEFRAME"
        protected IntPtr _mainHandle;

        //handle of client area.
        // client area means the area to display web pages, not include the menu, address bar, etc.
        // we can use Spy++ to get these information.
        protected IntPtr _ieServerHandle;

        //handle of shell doc.
        protected IntPtr _shellDocHandle;

        //handle of "showModelessDialog" and "showDialog". They contain HTML, but not a InternetExplorer instance.
        protected IntPtr _dialogHandle;

        //HTML dom, we use HTML dom to get the HTML object.
        protected HTMLDocument _HTMLDom;

        //wait for 120 secs, for example, to wait for the browser exist.
        protected int _maxWaitSeconds = 120;

        //every time sleep for 3 secs if browser is not found.
        protected const int _interval = 3;

        //timespan to store the response time.
        //the time is the interval between starting download and downloading finish.
        protected float _startTime;
        protected float _endTime;
        protected float _responseTime;

        //hash table to store the response time for each URL.
        //the key is URL, the value is response time.
        protected Dictionary<string, float> _performanceTimeHT = new Dictionary<string, float>();

        //the version of browser, eg 7.0
        protected string _version;

        //the name of browser, eg Internet Explorer.
        protected string _browserName;

        /*we have 3 area here.
         * 1. rectangle of borwser, including menu bar, address bar.
         * 2. rectangle of client area, means the area to display web pages, not including meun bar, address bar.
         * 3. rectangle of web page, a web page may larger than the client area, in this situation, we will see
         *    scroll bar.
         */

        // the browser window's rect
        protected int _ieLeft;
        protected int _ieTop;
        protected int _ieWidth;
        protected int _ieHeight;

        // the client area's rect
        protected int _clientTop;
        protected int _clientLeft;
        protected int _clientWidth;
        protected int _clientHeight;

        //the web page's rect, may larger than client area because of scroll bar.
        protected int _scrollLeft;
        protected int _scrollTop;
        protected int _scrollWidth;
        protected int _scrollHeight;

        //flag to show wether the browser is downloading sth.
        protected bool _isDownloading = false;

        protected bool _sendMsgOnly = false;

        #endregion

        #region Sync Event
        //sync event is to mark the important event.

        //start download happened we the browser start to load a web page.
        //protected AutoResetEvent _startDownload = new AutoResetEvent(false);

        //document load complete happened when the web page is loaded, then we can start to 
        //search the HTML object.
        protected AutoResetEvent _documentLoadComplete = new AutoResetEvent(false);

        //happened when internet explorer found.
        protected AutoResetEvent _browserExisted = new AutoResetEvent(false);

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

        public int Left
        {
            get
            {
                return _ieLeft;
            }
        }

        public int Top
        {
            get
            {
                return _ieTop;
            }
        }

        public int Width
        {
            get
            {
                return _ieWidth;
            }
        }

        public int Height
        {
            get
            {
                return _ieHeight;
            }
        }

        public int ClientTop
        {
            get
            {
                return _clientTop;
            }
        }

        public int ClientLeft
        {
            get
            {
                return _clientLeft;
            }
        }

        public int ClientWidth
        {
            get
            {
                return _clientWidth;
            }
        }

        public int ClientHeight
        {
            get
            {
                return _clientHeight;
            }
        }

        public int ScrollLeft
        {
            get
            {
                //GetScrollRect();
                return _scrollLeft;
            }
        }
        public int ScrollTop
        {
            get
            {
                GetScrollRect();
                return _scrollTop;
            }
        }
        public int ScrollWidth
        {
            get
            {
                //GetScrollRect();
                return _scrollWidth;
            }
        }
        public int ScrollHeight
        {
            get
            {
                //GetScrollRect();
                return _scrollHeight;
            }

        }

        //main handle of ie window
        public IntPtr MainHandle
        {
            get { return _mainHandle; }
        }

        //handle of client area, Internet Explorer_Server
        public IntPtr IEServerHandle
        {
            get { return _ieServerHandle; }
        }

        public IntPtr DialogHandle
        {
            get { return _dialogHandle; }
        }

        //response time of current page.
        public float ResponseTime
        {
            get { return _responseTime; }
            set { _responseTime = value; }
        }

        //property to show if the browser is downloading sth.
        public bool IsBusy
        {
            get
            {
                CheckModelessDialog();

                return _isDownloading || _ie.Busy;
            }
        }

        public InternetExplorer[] AllBrowsers
        {
            get
            {
                return GetAllBrowsers();
            }
        }

        public bool SendMsgOnly
        {
            get { return _sendMsgOnly; }
            set { _sendMsgOnly = value; }
        }


        #endregion

        #region Methods

        #region ctor
        //I keep the constructor as "public" because of reflecting
        public TestBrowser()
        {
            //currently, just support internet explorer
            _browserName = "Internet Explorer";
            _version = GetBrowserVersion();

        }

        ~TestBrowser()
        {
            // when GC, close AutoResetEvent.
            Dispose();
        }

        public virtual void Dispose()
        {
            if (_browserExisted != null)
            {
                //  _ieExisted.Close();
                _browserExisted.Close();
                //_ieExisted = null;
                _browserExisted = null;

            }

            if (_documentLoadComplete != null)
            {
                _documentLoadComplete.Close();
                _documentLoadComplete = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region public methods

        #region operate IE

        /*  void Start()
         *  start Internet Explorer, and register the event.
         *  if failed, throw CannotStartTestBrowserException
         */
        public virtual void Start()
        {
            try
            {
                _browserProcess = Process.Start("iexplore.exe", "about:blank");

                //start a new thread to check the browser status, if OK, we will attach _ie to Internet Explorer
                Thread ieExistT = new Thread(new ThreadStart(WaitForBrowserExist));
                ieExistT.Start();

                //wait until the internet explorer started.
                _browserExisted.WaitOne(_maxWaitSeconds * 1000, true);

                if (_ie != null)
                {
                    SetBrowser(_ie);
                }
                else
                {
                    throw new CannotStartBrowserException("Can not start test browser.");
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotStartBrowserException("Can not start Internet explorer: " + ex.Message);
            }

        }

        /*  void Find(object browserTitle)
         *  find an instance of browser by its title.
         *  eg: Google.com.
         */
        public virtual void Find(object browserTitle)
        {
            string title;
            if (browserTitle == null || String.IsNullOrEmpty(browserTitle.ToString()))
            {
                throw new CannotAttachBrowserException("Browser title can not be empty.");
            }
            else
            {
                title = browserTitle.ToString();

                //add standard title to the string.
                if (!title.EndsWith(" - Windows Internet Explorer"))
                {
                    title += " - Windows Internet Explorer";
                }
            }

            try
            {
                //start a new thread to check the browser status, if OK, we will attach _ie to Internet Explorer
                Thread ieExistT = new Thread(new ParameterizedThreadStart(WaitForBrowserExist));
                ieExistT.Start(title);

                //wait until the internet explorer is found.
                _browserExisted.WaitOne(this._maxWaitSeconds * 1000, true);

                if (_ie != null)
                {
                    SetBrowser(_ie);
                }
                else
                {
                    throw new CannotAttachBrowserException("Can not find test browser.");
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not find test browser: " + ex.Message);
            }
        }

        /* void Close()
         * Close Browser.
         * 
         */
        public virtual void Close()
        {
            try
            {
                if (_ie != null)
                {
                    _ie.Quit();
                }
            }
            catch (Exception ex)
            {
                throw new CannotStopBrowserException("Can not close browser: " + ex.Message);
            }
        }

        /* void Load(string url)
         * Load the expected url. eg: www.sina.com.cn 
         * before we load url, we need to use Start() method to start browser first.
         */
        public virtual void Load(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new CannotLoadUrlException("Url can not be null.");
            }

            // if ie is not started, wait for 120s.
            if (_ie == null)
            {
                Start();
            }

            object tmp = new object();
            try
            {
                // navigate to the expected url.
                _ie.Navigate(url, ref tmp, ref tmp, ref tmp, ref tmp);
            }
            catch (Exception ex)
            {
                throw new CannotLoadUrlException("Can not load url: " + url + " : " + ex.Message);
            }

            //wait until the HTML web page is loaded successfully.
            this._documentLoadComplete.WaitOne(_maxWaitSeconds * 1000, true);

        }

        public virtual void Load(Uri url)
        {
            Load(url.ToString());
        }

        /* void Move(int top,int left)
         * move the browser to expected position. 
         * left,top means the position of left corner point
         */
        public virtual void Move(int top, int left)
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _ie.Top = top;
                _ie.Left = left;
            }
            catch (Exception ex)
            {
                throw new CannotActiveBrowserException("Can not move browser: " + ex.Message);
            }
        }

        /* void Resize(int width, int height) 
         * resize the browser, set it's width and height
         */
        public virtual void Resize(int width, int height)
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

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
                _ie.Width = width;
                _ie.Height = height;
            }
            catch (Exception ex)
            {
                throw new CannotActiveBrowserException("Can not resize browser: " + ex.Message);
            }
        }

        /* void Back()
         * let the browser back to the previous url, just like you click the Back button on menu bar.
         * 
         * */
        public virtual void Back()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _ie.GoBack();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not go back: " + ex.Message);
            }

        }

        /* void Forward()
         * let the browser navigate to the next page, just like you click the Forward button on menu bar. 
         */
        public virtual void Forward()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _ie.GoForward();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not go forward: " + ex.Message);
            }
        }

        /* void Home()
         * let the browser navigate to it's home page.
         */
        public virtual void Home()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _ie.GoHome();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not go home: " + ex.Message);
            }

        }

        /* void Refresh()
         * Refresh the current page. 
         */
        public virtual void Refresh()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _ie.Refresh();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not refresh: " + ex.Message);
            }
        }

        /* void Wait(int seconds)
         * make the browser wait for seconds.
         * if the parameter is smaller than 0, we will wait for int.MaxValue(2,147,483,647) seconds. 
         */
        public virtual void Wait(int seconds)
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            if (seconds > 0)
            {
                Thread.Sleep(seconds * 1000);
            }
        }

        /* void WaitForNewWindow()
         * wait until a new Internet Explorer exist. 
         */
        public virtual void WaitForNewWindow()
        {

            try
            {
                WaitForNewBrowserAsyn();

                _browserExisted.WaitOne(_maxWaitSeconds * 1000, true);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BrowserNotFoundException(ex.Message);
            }
        }

        /* void WaitForNewTab()
         *  in tabbed browser, like Internet Explorer 7, wait until a new tab exist.
         * 
         */
        public virtual void WaitForNewTab()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                // WaitForNewBrowserSync();
                WaitForNewBrowserAsyn();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BrowserNotFoundException(ex.Message);
            }
        }

        /* void WaitForNextPage()
         * wait until the browser load a new page.
         * eg: in google.com, you input something, then you click search button, you need to wait the browser to refresh,
         * then you can see the result page.
         */
        public virtual void WaitForNextPage()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            WaitDocumentLoadComplete();
        }

        /* void WaitForPopWindow()
         * wait until the browser pop up a new HTML window.
         * It is a new Internet Explorer_Server control.
         * 
         */
        public virtual void WaitForPopWindow()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            int times = 0;
            while (times < _maxWaitSeconds)
            {
                IntPtr popWindowHandle = GetDialogHandle(_mainHandle);

                if (popWindowHandle != IntPtr.Zero)
                {
                    IntPtr popWindowIEServerHandle = GetIEServerHandle(popWindowHandle);

                    if (popWindowIEServerHandle != IntPtr.Zero)
                    {

                        //save the current browser status.
                        _ieStack.Push(_ie);

                        _mainHandle = popWindowHandle;
                        _ieServerHandle = popWindowIEServerHandle;
                        _HTMLDom = GetHTMLDomFromHandle(popWindowIEServerHandle);

                        // get the pop up window's size
                        GetSize();

                        break;
                    }
                }

                Thread.Sleep(_interval * 1000);
                times += _interval;
            }
        }

        /* void MaxSize()
         * max the browser.
         */
        public virtual void MaxSize()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                Win32API.SendMessage(_mainHandle, Convert.ToInt32(Win32API.WindowMessages.WM_SYSCOMMAND), Convert.ToInt32(Win32API.WindowMenuMessage.SC_MAXIMIZE), 0);
            }
            catch (Exception ex)
            {
                throw new CannotActiveBrowserException("Can not MAX Internet Explorer: " + ex.Message);
            }
        }

        /* void Active()
         * make the browser active, set it focus and to top most window, then we can interactive with it.
         */
        public virtual void Active()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                if (!_sendMsgOnly)
                {
                    Win32API.SetForegroundWindow(_mainHandle);
                }
            }
            catch (Exception ex)
            {
                throw new CannotActiveBrowserException("Can not Active browser: " + ex.Message);
            }
        }

        /* void Print(String printerName)
         * print current page.
         */
        public virtual void Print(String printerName, int copies)
        {
            if (Printer.GetActivePrintDialog())
            {
                try
                {
                    if (!String.IsNullOrEmpty(printerName))
                    {
                        Printer.ChoosePrinter(printerName);
                    }

                    if (copies > 0)
                    {
                        Printer.SetCopyCount(copies);
                    }

                    Printer.PressPrint();
                }
                catch (Exception ex)
                {
                    throw new CannotPrintException("Can not print current page: " + ex.Message);
                }
            }
            else
            {
                throw new CannotPrintException("Can not find print dialog.");
            }
        }

        /* void CloseDialog()
         * 
         */
        public virtual void CloseDialog()
        {
            if (this._dialogHandle != IntPtr.Zero)
            {
                try
                {
                    Win32API.SendMessage(this._dialogHandle, (int)Win32API.WindowMessages.WM_CLOSE, 0, 0);

                    this._dialogHandle = IntPtr.Zero;

                    SetBrowser(this._ie);
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotStopBrowserException("Can not close dialog: " + ex.Message);
                }
            }
        }

        #endregion

        #region Get IE Information

        /* string GetCurrentUrl()
         * return the current url in the address bar of browser
         * 
         */
        public virtual string GetCurrentUrl()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _ie.LocationURL;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the current url: " + ex.Message);
            }
        }

        /* string GetTitle()
         * return title of current page.
         */
        public virtual string GetTitle()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _ie.LocationName;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the current url: " + ex.Message);
            }
        }

        /* string GetStatusText()
         * return the status text of browser.
         */
        public virtual string GetStatusText()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _ie.StatusText;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the status text: " + ex.Message);
            }
        }

        /* bool IsMenuVisible()
         * return true if the menu bar is visible. 
         * sometimes we will cancel the menu bar, eg: pop up window.
         */
        public virtual bool IsMenuVisiable()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _ie.MenuBar;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the menu status: " + ex.Message);
            }
        }

        /* bool IsResizeable()
         * return true if the browser can be resized.
         * 
         */
        public virtual bool IsResizeable()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _ie.Resizable;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the resize status: " + ex.Message);
            }
        }

        /* bool IsFullScreen()
         * return true if the  browser is full screen.
         * 
         */
        public virtual bool IsFullScreen()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _ie.FullScreen;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the full screen status: " + ex.Message);
            }

        }

        /*  bool IsModelessDialog()
         * 
         */
        public virtual bool IsModelessDialog(IntPtr dialogHandle)
        {
            if (this._mainHandle != IntPtr.Zero && dialogHandle != IntPtr.Zero && this._mainHandle != dialogHandle)
            {
                Win32API.SetForegroundWindow(this._mainHandle);

                return Win32API.GetForegroundWindow() == this._mainHandle;
            }

            return false;
        }

        /* string GetBrowserName()
         * return the name of the browser. eg: Internet Explorer.
         * 
         */
        public virtual String GetBrowserName()
        {
            return _browserName;
        }

        /* string GetBrowserVersion()
         * return the version number of the browser.
         */
        public virtual String GetBrowserVersion()
        {
            if (String.IsNullOrEmpty(_version))
            {
                using (Microsoft.Win32.RegistryKey versionKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Internet Explorer"))
                {
                    _version = versionKey.GetValue("Version").ToString();
                }
            }

            return _version;
        }

        /* int GetLoadSeconds()
         * return the seconds time used to load  current page.
         */
        public virtual float GetLoadSeconds()
        {

            int times = 0;
            while (times < this._maxWaitSeconds)
            {
                if (IsBusy)
                {
                    Thread.Sleep(1 * 1000);
                    times++;
                }
                else
                {
                    return GetLoadSecondsByURL(GetCurrentUrl());
                }
            }

            return -1;
        }

        /* GetLoadSecondsByURL(Uri uri)
         * get the seconds used to load the url.
         */
        public virtual float GetLoadSecondsByURL(Uri uri)
        {
            return GetLoadSecondsByURL(uri.ToString());
        }

        public virtual float GetLoadSecondsByURL(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return -1;
            }

            string key = url;
            float responseTime;
            if (_performanceTimeHT.TryGetValue(key, out responseTime))
            {
                return responseTime;
            }
            else
            {
                return -1;
            }
        }

        /* float GetCurrentSeconds()
         * get the seconds, used for performance calc.
         */
        protected float GetCurrentSeconds()
        {
            return (float)DateTime.Now.Second + ((float)DateTime.Now.Millisecond) / 1000;
        }

        /* string GetHTML()
         * return the HTML code of current page.
         */
        public virtual string GetHTML()
        {
            return _HTMLDom == null ? "" : _HTMLDom.body.innerHTML;
        }

        #endregion

        #region SYNC

        /* void WaitDocumentLoadComplete(int seconds)
         * wait until the HTML document load completely.
         * default will wait for 120s.
         */
        public virtual void WaitDocumentLoadComplete()
        {
            WaitDocumentLoadComplete(this._maxWaitSeconds);
        }

        public virtual void WaitDocumentLoadComplete(int seconds)
        {
            if (seconds < 0)
            {
                seconds = 0;
            }

            // Console.WriteLine(DateTime.Now.ToString());

            //_startDownload.Reset();
            //_startDownload.WaitOne(seconds * 1000, true);

            //_documentLoadComplete.Reset();
            _documentLoadComplete.WaitOne(seconds * 1000, true);

        }

        /* void WaitForBrowser(int seconds.)
      * wait for 120 seconds max to detect if IE browser is started.
      * if title is not empty, we need to check the title of browser.
      * we will use "Fuzzy Search" in this method
      */
        protected virtual void WaitForBrowserExist(string title, int seconds)
        {
            if (seconds < 0)
            {
                seconds = 0;
            }

            bool isByTitle = true;

            //if the title is empty, we need to find the browser by process id.
            if (String.IsNullOrEmpty(title))
            {
                isByTitle = false;
            }

            int simPercent = 100;

            int times = 0;
            while (times <= seconds)
            {
                bool browserFound = false;

                Process[] pArr = Process.GetProcessesByName("iexplore");

                foreach (Process p in pArr)
                {
                    //find the browser by it's title
                    if (isByTitle)
                    {
                        if (Searcher.IsStringLike(p.MainWindowTitle, title, simPercent))
                        {
                            browserFound = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else //find the browser by process id.
                    {
                        //if we didn't start a browser process, we will use an exist browser.
                        if (_browserProcess == null)
                        {
                            browserFound = true;
                        }
                        else
                        {
                            // after starting an iexplorer, if we use _browserProcess.MainWindowHandle, it is always 0 and will not change.
                            // currently I don't know why????
                            // so we use this way to find the main handle.
                            if (p.Id == _browserProcess.Id)
                            {
                                browserFound = true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    if (browserFound)
                    {
                        if (p.MainWindowHandle == IntPtr.Zero)
                        {
                            //not ready, try again.
                            break;
                        }

                        _browserProcess = p;

                        //main window handle is the handle of Internet explorer.
                        _mainHandle = p.MainWindowHandle;

                        _ie = AttachBrowser(_mainHandle);

                        //we attached to IE successfully.
                        _browserExisted.Set();

                        return;

                    }
                }

                //sleep for 3 seconds, find again.
                times += _interval;
                Thread.Sleep(_interval * 1000);

                //try to use lower similarity to find the browser.
                if (!browserFound)
                {
                    if (simPercent > 70)
                    {
                        simPercent -= 10;
                    }
                    else
                    {
                        simPercent = 100;
                    }
                }

            }

            throw new BrowserNotFoundException();

        }

        protected virtual void WaitForBrowserExist()
        {
            WaitForBrowserExist(null, _maxWaitSeconds);
        }

        protected virtual void WaitForBrowserExist(object title)
        {
            WaitForBrowserExist(title.ToString(), _maxWaitSeconds);
        }

        /* void WaitForNewBrowserSync()
         * Wait until a new browser exist.
         */
        protected virtual void WaitForNewBrowserSync()
        {

            this._isDownloading = true;

            int times = 0;

            while (times < this._maxWaitSeconds)
            {
                InternetExplorer ie = GetTopmostBrowser();

                if (this._ie != ie)
                {
                    try
                    {
                        SetBrowser(ie);
                        break;
                    }
                    catch (TestException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new BrowserNotFoundException(ex.Message);
                    }
                }
                else
                {
                    Thread.Sleep(_interval * 1000);
                    times += _interval;
                }
            }

            this._isDownloading = false;

            this._browserExisted.Set();
        }

        /* void WaitForNewBrowserAsyn()
         * Wait until a new browser exist.
         */
        protected virtual void WaitForNewBrowserAsyn()
        {
            Thread ieExistT = new Thread(new ThreadStart(WaitForNewBrowserSync));
            ieExistT.Start();
        }

        /* void CheckModelessDialog()
         * Wait until a new browser exist.
         */
        protected virtual void CheckModelessDialog()
        {
            IntPtr dialogHandle = GetDialogHandle(this._mainHandle);

            if (dialogHandle != IntPtr.Zero && IsModelessDialog(dialogHandle))
            {
                WaitForPopWindow();
            }
        }
        #endregion

        #endregion

        #region protected virtual help methods

        /* InternetExplorer AttachBrowser(IntPtr ieHandle)
         * return the instance of InternetExplorer.
         * 
         */
        protected virtual InternetExplorer AttachBrowser(IntPtr ieHandle)
        {
            if (ieHandle == IntPtr.Zero)
            {
                throw new CannotAttachBrowserException("IE handle can not be 0.");
            }

            //try until timeout, the default is 120s.
            int times = 0;
            while (times < this._maxWaitSeconds)
            {
                //get all shell browser.
                InternetExplorer[] allBrowsers = GetAllBrowsers();

                if (allBrowsers != null)
                {
                    InternetExplorer tempIE = null;

                    for (int i = 0; i < allBrowsers.Length; i++)
                    {
                        tempIE = (InternetExplorer)allBrowsers[i];

                        if (tempIE != null && (int)ieHandle == tempIE.HWND)
                        {
                            return tempIE;
                        }
                    }
                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new CannotAttachBrowserException();
        }

        /* InternetExplorer GetTopmostBrowser()
         * get the current active browser.
         */
        protected virtual InternetExplorer GetTopmostBrowser()
        {
            //get all shell browser.
            SHDocVw.ShellWindows allBrowsers = new ShellWindows();

            if (allBrowsers.Count > 0)
            {
                InternetExplorer curIE = null;

                for (int i = allBrowsers.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        curIE = (InternetExplorer)allBrowsers.Item(i);

                        if (curIE != null && curIE.Document is IHTMLDocument)
                        {
                            return curIE;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return null;

        }

        /* InternetExplorer[] GetAllBrowsers()
         * get all the browsers currently.
         */
        protected virtual InternetExplorer[] GetAllBrowsers()
        {
            //get all shell browser.
            SHDocVw.ShellWindows allBrowsers = new ShellWindows();

            if (allBrowsers.Count > 0)
            {
                List<InternetExplorer> ieList = new List<InternetExplorer>(allBrowsers.Count);

                InternetExplorer curIE = null;

                bool found = false;

                for (int i = allBrowsers.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        curIE = (InternetExplorer)allBrowsers.Item(i);

                        if (curIE != null && curIE.Document is IHTMLDocument)
                        {
                            ieList.Add(curIE);

                            found = true;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (found)
                {
                    return ieList.ToArray();
                }
            }

            return null;
        }

        /* void RefreshBrowser(InternetExplorer ie)
         * use the new ie to replace old instance.
         */
        protected virtual void SetBrowser(InternetExplorer ie)
        {
            if (ie != null)
            {
                try
                {
                    this._isDownloading = true;

                    InternetExplorer tmp = _ie;

                    //set handle
                    this._mainHandle = (IntPtr)ie.HWND;

                    MaxSize();

                    //register event.
                    RegBrowserEvent(ie);

                    try
                    {
                        if (ie.Document != null)
                        {
                            //set HTML DOM
                            this._HTMLDom = (HTMLDocument)ie.Document;
                        }
                    }
                    catch
                    {
                    }

                    //set Internet Explorer.
                    this._ie = ie;

                    if (!ie.Busy)
                    {
                        GetSize();
                    }

                    if (tmp != null && tmp != ie)
                    {
                        InternetExplorer[] allBrowsers = GetAllBrowsers();
                        foreach (InternetExplorer i in allBrowsers)
                        {
                            if (i == tmp)
                            {
                                _ieStack.Push(tmp);
                                break;
                            }
                        }
                    }
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotAttachBrowserException("Can not set browser: " + ex.Message);
                }
                finally
                {
                    this._isDownloading = false;
                }
            }
        }

        /* void GetPrevTestBrowserStatus()
         * Get previous browser status. eg: when pop up window disapper, we need to return to the main window.
         */
        protected virtual void GetPrevTestBrowser()
        {
            if (_ieStack.Count > 0)
            {
                try
                {
                    //get the previous status from stack.
                    SetBrowser(_ieStack.Pop());
                }
                catch (Exception ex)
                {
                    throw new CannotAttachBrowserException("Can not get previous handle: " + ex.Message);
                }
            }
        }

        /* bool IsConnected(string url)
         * return true if the url is online.
         */
        protected virtual bool IsOnline(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }

            try
            {
                HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(url);
                wReq.AllowAutoRedirect = true;
                wReq.Timeout = this._maxWaitSeconds;

                HttpWebResponse wResp = (HttpWebResponse)wReq.GetResponse();

                return wReq.HaveResponse;
            }
            catch
            {
                return false;
            }
        }

        ///* void SetDialog()
        // * use the dialog to find object.
        // */
        //protected virtual void SetDialog()
        //{
        //    this._isDownloading = true;

        //    if (this._dialogHandle == IntPtr.Zero)
        //    {

        //    }

        //    this._isDownloading = false;
        //}

        #region get size

        /* void GetSize()
         * Get all size information of browser.
         */
        protected virtual void GetSize()
        {
            MaxSize();

            GetBrowserRect();
            GetClientRect();
            GetScrollRect();
        }

        /* void GetBrowserRect()
         * browser rect: the whole rect of IE, include menu, address bar etc.
         */
        protected void GetBrowserRect()
        {
            if (_ie == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _ieLeft = _ie.Left;
                _ieTop = _ie.Top;
                _ieWidth = _ie.Width;
                _ieHeight = _ie.Height;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not get the rect: " + ex.Message);
            }
        }


        /* void GetClientRect()
         * client rect: the web page rect, not include menu, address bar ect.
         */
        protected void GetClientRect()
        {

            _mainHandle = GetIEMainHandle();

            if (_mainHandle == IntPtr.Zero)
            {
                throw new BrowserNotFoundException("Can not get main handle of test browser.");
            }

            _shellDocHandle = GetShellDocHandle(_mainHandle);
            if (_shellDocHandle == IntPtr.Zero)
            {
                throw new BrowserNotFoundException("Can not get shell handle of test browser");
            }


            //determine the yellow warning bar, for example, if the web page contains ActiveX, we can see the yellow bar at the top of the web page.
            //if the warning bar exist, we need to add 20 height to each html control.
            IntPtr warnBar = GetWarnBarHandle(_shellDocHandle);
            int addHeight = 0;
            if (warnBar != IntPtr.Zero)
            {
                Win32API.Rect warnRect = new Win32API.Rect();
                if (Win32API.GetWindowRect(warnBar, ref warnRect))
                {
                    addHeight = warnRect.Height;
                }
                else
                {
                    throw new CannotAttachBrowserException("Can not get warn bar size.");
                }

            }

            //Get the actual client area rect, which shows web page to the end user.

            _ieServerHandle = GetIEServerHandle(_shellDocHandle);
            if (_ieServerHandle == IntPtr.Zero)
            {
                throw new BrowserNotFoundException("Can not get ie_server handle of test browser");
            }

            try
            {
                Win32API.Rect tmpRect = new Win32API.Rect();
                if (Win32API.GetWindowRect(_ieServerHandle, ref tmpRect))
                {
                    _clientLeft = tmpRect.left;
                    _clientTop = tmpRect.top + addHeight;
                    _clientWidth = tmpRect.Width;
                    _clientHeight = tmpRect.Height;
                }
                else
                {
                    throw new CannotAttachBrowserException("Can not get client size of test browser.");
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not get client size of test browser: " + ex.Message);
            }

        }

        /* void GetScrollRect()
         * the whole web page rect, include invisible part, for example, some web pages are too long to display, we need to scroll them.
         * 
         */
        protected void GetScrollRect()
        {
            if (_ie != null && _ie.Document != null)
            {
                try
                {
                    _HTMLDom = (HTMLDocument)_ie.Document;
                    HTMLBody bodyElement = (HTMLBody)_HTMLDom.body;
                    _scrollWidth = bodyElement.scrollWidth;
                    _scrollHeight = bodyElement.scrollHeight;

                    // scrollLeft means the left that Can Not been seen, scrollTop the same.
                    _scrollLeft = bodyElement.scrollLeft;
                    _scrollTop = bodyElement.scrollTop;
                }
                catch (Exception ex)
                {
                    throw new CannotAttachBrowserException("Can not get scroll size: " + ex.Message);
                }
            }
        }



        #endregion

        #region get handles of each windows control
        /* IntPtr GetIEMainHandle()
         * return the handle of IEFrame, this is the parent handle of Internet Explorer. 
         * we can use Spy++ to get the the handle.
         */
        protected IntPtr GetIEMainHandle()
        {
            if (_mainHandle == IntPtr.Zero)
            {
                return Win32API.FindWindow("IEFrame", null);
            }
            else
            {
                return _mainHandle;
            }
        }

        /* IntPtr GetShellDocHandle(IntPtr mainHandle)
         * return the handle of Shell DocObject View.
         * we can use Spy++ to get the tree structrue of Internet Explorer handles. 
         */
        protected IntPtr GetShellDocHandle(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
            {
                mainHandle = GetIEMainHandle();
            }

            //update for Internet Explorer 7
            //Internet Explorer 7 is a tab browser, we need to find "TabWindowClass" before we get the "Sheel DocObject View"

            //lower version than IE 7.0
            if (Convert.ToInt32(_version[0].ToString()) < 7)
            {
                return Win32API.FindWindowEx(mainHandle, IntPtr.Zero, "Shell DocObject View", null);
            }
            else
            {
                IntPtr tabWindow = IntPtr.Zero;

                //get the active tab.
                while (!Win32API.IsWindowVisible(tabWindow))
                {
                    tabWindow = Win32API.FindWindowEx(mainHandle, tabWindow, "TabWindowClass", null);
                }

                return Win32API.FindWindowEx(tabWindow, IntPtr.Zero, "Shell DocObject View", null);
            }

        }

        /* IntPtr GetWarnBarHandle(IntPtr shellHandle)
         * return the warning bar handle. in Internet Explorer 6.0 with XP2 or Internet Explorer 7.0.
         * when the web page contains ActiveX (eg: FlashPlayer), we can see a yellow bar on the browser.
         */
        protected IntPtr GetWarnBarHandle(IntPtr shellHandle)
        {
            if (shellHandle == IntPtr.Zero)
            {
                shellHandle = GetShellDocHandle(IntPtr.Zero);
            }

            return Win32API.FindWindowEx(shellHandle, IntPtr.Zero, "#32770 (Dialog)", null);
        }

        /* IntPtr GetIEServerHandle(IntPtr shellHandle)
         * return the handle of Internet Explorer_Server.
         * This control is used to display web page.
         */
        protected IntPtr GetIEServerHandle(IntPtr shellHandle)
        {
            if (shellHandle == IntPtr.Zero)
            {
                shellHandle = GetShellDocHandle(IntPtr.Zero);
            }

            return Win32API.FindWindowEx(shellHandle, IntPtr.Zero, "Internet Explorer_Server", null);
        }

        /* IntPtr GetDialogHandle(IntPtr mainHandle)
         * return the handle of pop up page.
         * the name is Internet Explorer_TridentDlgFrame.
         */
        protected IntPtr GetDialogHandle(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
            {
                mainHandle = GetIEMainHandle();
            }
            IntPtr popHandle = Win32API.FindWindow("Internet Explorer_TridentDlgFrame", null);

            if (popHandle != IntPtr.Zero)
            {
                IntPtr parentHandle = Win32API.GetParent(popHandle);

                if (parentHandle == mainHandle)
                {
                    return popHandle;
                }
            }

            return IntPtr.Zero;
        }

        /*  HTMLDocument GetHTMLDomFromHandle(IntPtr ieServerHandle)
         *  return HTMLDocument from a handle.
         *  When we get a pop up window, we need to get it's HTML Dom to get HTML object.
         *  So we need to convert it's handle to HTML Document.
         */
        protected HTMLDocument GetHTMLDomFromHandle(IntPtr ieServerHandle)
        {
            if (ieServerHandle == IntPtr.Zero)
            {
                ieServerHandle = GetIEServerHandle(IntPtr.Zero);
            }

            if (ieServerHandle != IntPtr.Zero)
            {
                int nMsg = Win32API.RegisterWindowMessage("WM_HTML_GETOBJECT");

                UIntPtr lRes;

                if (Win32API.SendMessageTimeout(ieServerHandle, nMsg, 0, 0, Win32API.SMTO_ABORTIFHUNG, 1000, out lRes) == 0)
                {
                    return null;
                }

                return (HTMLDocument)Win32API.ObjectFromLresult(lRes, typeof(IHTMLDocument).GUID, IntPtr.Zero);
            }
            else
            {
                return null;
            }
        }


        #endregion

        #endregion

        #region event

        /*  void RegBrowserEvent()
        *  register IE event.
        */
        protected virtual void RegBrowserEvent(InternetExplorer ie)
        {
            RegDownloadEvent(ie);
            RegDocumentLoadCompleteEvent(ie);
            RegNavigateEvent(ie);
            RegRectChangeEvent(ie);
            RegScrollEvent(ie);
            RegNewWindowEvent(ie);
            RegQuitEvent(ie);
        }

        /* void RegQuitEvent(InternetExplorer ie)
         * 
         */
        protected virtual void RegQuitEvent(InternetExplorer ie)
        {
            try
            {
                ie.OnQuit += new DWebBrowserEvents2_OnQuitEventHandler(OnQuit);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register Quit event: " + ex.Message);
            }
        }

        /*  void RegDownloadEvent()
         *  Register event when the browser is starting to download a web page.
         * 
         */
        protected virtual void RegDownloadEvent(InternetExplorer ie)
        {
            try
            {
                ie.DownloadBegin += new DWebBrowserEvents2_DownloadBeginEventHandler(OnDownloadBegin);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register Downloadbegin event: " + ex.Message);
            }
        }

        /* void RegOnNewWindowEvent()
         * register event when a new window pops up.
         * Notice: need update!
         */
        protected virtual void RegNewWindowEvent(InternetExplorer ie)
        {
            try
            {
                //ie.NewWindow3 += new DWebBrowserEvents2_NewWindow3EventHandler(OnNewWindow3);
                ie.NewWindow2 += new DWebBrowserEvents2_NewWindow2EventHandler(OnNewWindow2);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register new window event: " + ex.Message);
            }

        }

        /* void RegScrollEvent()
         * register event when scroll bar scroll.
         * Notice: current we don't need this event.
         */
        protected virtual void RegScrollEvent(InternetExplorer ie)
        {

        }

        /* void RegDocumentLoadCompleteEvent()
         * register event when the browser load a web page succesfully.
         */
        protected virtual void RegDocumentLoadCompleteEvent(InternetExplorer ie)
        {
            try
            {
                ie.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentLoadComplete);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register document complete event: " + ex.Message);
            }
        }

        /*  void RegRectChangeEvent()
         *  register event when the size of the browser changed.
         */
        protected virtual void RegRectChangeEvent(InternetExplorer ie)
        {
            try
            {
                ie.WindowSetTop += new DWebBrowserEvents2_WindowSetTopEventHandler(OnRectChanged);
                ie.WindowSetLeft += new DWebBrowserEvents2_WindowSetLeftEventHandler(OnRectChanged);
                ie.WindowSetWidth += new DWebBrowserEvents2_WindowSetWidthEventHandler(OnRectChanged);
                ie.WindowSetHeight += new DWebBrowserEvents2_WindowSetHeightEventHandler(OnRectChanged);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register Rect change event: " + ex.Message);
            }
        }

        /* void RegNavigateFailedEvent()
         * register event when the browser failed to load a url.
         */
        protected virtual void RegNavigateEvent(InternetExplorer ie)
        {
            try
            {
                ie.BeforeNavigate2 += new DWebBrowserEvents2_BeforeNavigate2EventHandler(OnBeforeNavigate2);
                ie.NavigateError += new DWebBrowserEvents2_NavigateErrorEventHandler(OnNavigateError);
                ie.NavigateComplete2 += new DWebBrowserEvents2_NavigateComplete2EventHandler(OnNavigateComplete2);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register navigate event: " + ex.Message);
            }
        }


        #region callback functions for each event

        /* void OnBeforeNavigate
         * Before start downloading.
         */
        protected virtual void OnBeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
        {
            //this._isDownloading = true;
        }

        /* void OnNavigateError
         * the callback function to handle navigate error.
         */
        protected virtual void OnNavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            throw new CannotLoadUrlException("Can not navigate to URL: " + URL.ToString());
        }

        /* void OnNavigateComplete2(object pDisp, ref object URL)
         * after navigate complete.
         */
        protected virtual void OnNavigateComplete2(object pDisp, ref object URL)
        {
            //this._isDownloading = false;
        }

        /* void OnNewWindow2(ref object ppDisp, ref bool Cancel)
         * the callback function to handle new "tab", it is not like IE 7 tab.
         * Notice: need update!!
         */
        protected void OnNewWindow2(ref object ppDisp, ref bool Cancel)
        {
            try
            {
                WaitForNewBrowserAsyn();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not attach new window: " + ex.Message);
            }

        }

        /* void void OnNewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
         * Notice: need update!!
         */
        protected void OnNewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            try
            {
                OnNewWindow2(ref ppDisp, ref Cancel);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException(ex.Message);
            }
        }

        /* void OnDownloadBegin()
         * the callback function to handle the browser starting download a web page.
         */
        protected virtual void OnDownloadBegin()
        {
            try
            {
                // _isDownloading = true;

                //get the start time, used to cal response time.
                _startTime = GetCurrentSeconds();

                //this._startDownload.Set();

                //this._documentLoadComplete.Reset();
                //this._documentLoadComplete.WaitOne(_maxWaitSeconds * 1000, true);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Error OnDownLoadBegin: " + ex.Message);
            }

        }

        /* void OnDocumentLoadComplete(object pDesp, ref object pUrl)
         * the callback function to hanle the browser finishing download a web page.
         * when downloading complete, we can start to calculate the position of the web page.
         */
        protected virtual void OnDocumentLoadComplete(object pDesp, ref object pUrl)
        {
            try
            {

                string locationName = _ie.LocationName;

                if (locationName.IndexOf("HTTP 404") >= 0
                    || locationName.IndexOf("can not") > 0
                    || locationName.IndexOf("counld not") > 0
                    || locationName.IndexOf("ERROR") == 0)
                {
                    throw new CannotNavigateException("Can not load url: " + _ie.LocationURL);
                }

                _endTime = GetCurrentSeconds();

                _responseTime = _endTime - _startTime;

                string key = GetCurrentUrl();

                _performanceTimeHT.Add(key, _responseTime);

                _HTMLDom = (HTMLDocument)_ie.Document;

                GetSize();

                _documentLoadComplete.Set();

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Error OnDocumentLoadComplete: " + ex.Message);
            }

        }

        /* void OnRectChanged(int size)
         * the callback function to handle the browser changed it's position or size.
         * when the browser chaneged it's size, we need to re-calculate the position.
         */
        protected virtual void OnRectChanged(int size)
        {
            try
            {
                GetSize();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Error OnRectChanged: " + ex.Message);
            }
        }

        /* void OnQuit()
         * when close a browser.
         */
        protected virtual void OnQuit()
        {
            //get the prev test browser.
            GetPrevTestBrowser();
        }

        #endregion

        #endregion

        #endregion
    }
}
