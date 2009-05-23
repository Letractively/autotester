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
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
    public class TestBrowser : TestApp, IDisposable, ITestBrowser
    {
        #region Fileds

        //stack to save the browser status.
        protected static List<InternetExplorer> _browserList = new List<InternetExplorer>(16);
        //InternetExplorer is under SHDocVw namespace, we use this to attach to a browser.
        protected InternetExplorer _browser;

        //handle of client area.
        // client area means the area to display web pages, not include the menu, address bar, etc.
        // we can use Spy++ to get these information.
        protected IntPtr _ieServerHandle;
        //handle of shell doc.
        protected IntPtr _shellDocHandle;
        //handle of "showModelessDialog" and "showDialog". They contain HTML, but not a InternetExplorer instance.
        protected IntPtr _dialogHandle;

        //HTML dom, we use HTML dom to get the HTML object.
        protected HTMLDocument _rootDocument;

        //timespan to store the response time.
        //the time is the interval between starting download and downloading finish.
        protected float _startTime;
        protected float _endTime;
        //hash table to store the response time for each URL.
        //the key is URL, the value is response time.
        protected Dictionary<string, float> _performanceTimeHT = new Dictionary<string, float>(37);

        //the version of browser, eg 7.0
        protected static string _version;
        protected static int _majorVersion;
        //the name of browser, eg Internet Explorer.
        protected string _browserName;

        /*we have 3 area here.
         * 1. rectangle of browser, including menu bar, address bar.
         * 2. rectangle of client area, means the area to display web pages, not including meun bar, address bar.
         * 3. rectangle of web page, a web page may larger than the client area, in this situation, we will see
         *    scroll bar.
         */

        // the browser window's rect
        protected int _browserLeft;
        protected int _browserTop;
        protected int _browserWidth;
        protected int _browserHeight;

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

        #endregion

        #region Sync Event
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
                return _browserLeft;
            }
        }

        public int Top
        {
            get { return _browserTop; }
        }

        public int Width
        {
            get { return _browserWidth; }
        }

        public int Height
        {
            get { return _browserHeight; }
        }

        public int ClientTop
        {
            get { return _clientTop; }
        }

        public int ClientLeft
        {
            get { return _clientLeft; }
        }

        public int ClientWidth
        {
            get { return _clientWidth; }
        }

        public int ClientHeight
        {
            get { return _clientHeight; }
        }

        public int ScrollLeft
        {
            get { return _scrollLeft; }
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
            get { return _scrollWidth; }
        }
        public int ScrollHeight
        {
            get { return _scrollHeight; }
        }

        //main handle of ie window
        public IntPtr MainHandle
        {
            get { return _rootHandle; }
        }

        //handle of client area, Internet Explorer_Server
        public IntPtr IEServerHandle
        {
            get { return _ieServerHandle; }
        }

        //property to show if the browser is downloading sth.
        public bool IsBusy
        {
            get { return _isDownloading || _browser.Busy; }
        }

        #endregion

        #region Methods

        #region ctor
        //I keep the constructor as "public" because of reflecting
        public TestBrowser()
        {
            //currently, just support internet explorer
            _browserName = TestConstants.IE_Browser_Name;
            _version = GetBrowserVersion();
            _majorVersion = Convert.ToInt32(_version[0].ToString());
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
                _browserExisted.Close();
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
            Start(null, null);
        }

        public override void Start(string args)
        {
            Start(null, args);
        }

        public override void Start(string appFullPath, string args)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo();
                psInfo.FileName = TestConstants.IE_EXE;
                psInfo.Arguments = TestConstants.IE_BlankPage_Url;
                if (!String.IsNullOrEmpty(appFullPath) && appFullPath.ToUpper().EndsWith(".EXE"))
                {
                    psInfo.FileName = appFullPath;
                }
                if (!String.IsNullOrEmpty(args))
                {
                    psInfo.Arguments = args;
                }
                _appProcess = Process.Start(psInfo);
                //start a new thread to check the browser status, if OK, we will attach _ie to Internet Explorer
                Thread ieExistT = new Thread(new ParameterizedThreadStart(WaitForBrowserExist));
                ieExistT.Start("");
                //wait until the internet explorer started.
                _browserExisted.WaitOne(_maxWaitSeconds * 1000, true);
                if (_browser != null)
                {
                    SetBrowser(_browser);
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
                throw new CannotStartBrowserException("Can not start Internet explorer: " + ex.ToString());
            }
        }

        /*  void Find(object browserTitle)
         *  find an instance of browser by its title.
         *  eg: Google.com.
         */
        public override void Find(string browserTitle)
        {
            if (String.IsNullOrEmpty(browserTitle))
            {
                throw new CannotAttachBrowserException("Browser title can not be empty.");
            }

            try
            {
                //start a new thread to check the browser status, if OK, we will attach _ie to Internet Explorer
                Thread ieExistT = new Thread(new ParameterizedThreadStart(WaitForBrowserExist));
                ieExistT.Start(browserTitle);
                //wait until the internet explorer is found.
                _browserExisted.WaitOne(this._maxWaitSeconds * 1000, true);

                if (_browser != null)
                {
                    SetBrowser(_browser);
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
                throw new CannotAttachBrowserException("Can not find test browser: " + ex.ToString());
            }
        }

        /* void Close()
         * Close Browser.
         * 
         */
        public override void Close()
        {
            try
            {
                if (_browser != null)
                {
                    _browser.Quit();
                }
            }
            catch (Exception ex)
            {
                throw new CannotStopBrowserException("Can not close browser: " + ex.ToString());
            }
        }


        /* void Load(string url)
        * Load the expected url. eg: www.sina.com.cn 
        * before we load url, we need to use Start() method to start browser first.
        */
        public virtual void Load(string url)
        {
            Load(url, true);
        }

        /* void Load(string url)
         * Load the expected url. eg: www.sina.com.cn 
         * before we load url, we need to use Start() method to start browser first.
         */
        public virtual void Load(string url, bool waitForPage)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new CannotLoadUrlException("Url can not be null.");
            }

            if (_browser == null)
            {
                throw new BrowserNotFoundException("IE is not started.");
            }

            object tmp = new object();
            try
            {
                // navigate to the expected url.
                _browser.Navigate(url, ref tmp, ref tmp, ref tmp, ref tmp);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                throw new CannotLoadUrlException("Can not load url: " + url + " : " + ex.ToString());
            }

            //wait until the HTML web page is loaded successfully.
            if (waitForPage)
            {
                this._documentLoadComplete.WaitOne(_maxWaitSeconds * 1000, true);
            }
        }

        /* void Back()
         * let the browser back to the previous url, just like you click the Back button on menu bar.
         * 
         * */
        public virtual void Back()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _browser.GoBack();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not go back: " + ex.ToString());
            }
        }

        /* void Forward()
         * let the browser navigate to the next page, just like you click the Forward button on menu bar. 
         */
        public virtual void Forward()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _browser.GoForward();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not go forward: " + ex.ToString());
            }
        }

        /* void Home()
         * let the browser navigate to it's home page.
         */
        public virtual void Home()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _browser.GoHome();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not go home: " + ex.ToString());
            }
        }

        /* void Refresh()
         * Refresh the current page. 
         */
        public virtual void Refresh()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                _browser.Refresh();
            }
            catch (Exception ex)
            {
                throw new CannotNavigateException("Can not refresh: " + ex.ToString());
            }
        }

        /* void WaitForNextPage()
         * wait until the browser load a new page.
         * eg: in google.com, you input something, then you click search button, you need to wait the browser to refresh,
         * then you can see the result page.
         */
        public virtual void WaitForPage()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            WaitDocumentLoadComplete();
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
                    AsstFunctions.CloseWindow(_dialogHandle);
                    this._dialogHandle = IntPtr.Zero;
                    SetBrowser(this._browser);
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotStopBrowserException("Can not close dialog: " + ex.ToString());
                }
            }
        }

        #endregion

        #region Get IE Information

        /* string GetCurrentUrl()
         * return the current url in the address bar of browser
         * 
         */
        public virtual string GetUrl()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _browser.LocationURL;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the current url: " + ex.ToString());
            }
        }

        /* string GetTitle()
         * return title of current page.
         */
        public virtual string GetTitle()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _browser.LocationName;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the current url: " + ex.ToString());
            }
        }

        /* string GetStatusText()
         * return the status text of browser.
         */
        public virtual string GetStatusText()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _browser.StatusText;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the status text: " + ex.ToString());
            }
        }

        /* bool IsMenuVisible()
         * return true if the menu bar is visible. 
         * sometimes we will cancel the menu bar, eg: pop up window.
         */
        public virtual bool IsMenuVisiable()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _browser.MenuBar;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the menu status: " + ex.ToString());
            }
        }

        /* bool IsResizeable()
         * return true if the browser can be resized.
         * 
         */
        public virtual bool IsResizeable()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _browser.Resizable;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the resize status: " + ex.ToString());
            }
        }

        /* bool IsFullScreen()
         * return true if the  browser is full screen.
         * 
         */
        public virtual bool IsFullScreen()
        {
            if (_browser == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                return _browser.FullScreen;
            }
            catch (Exception ex)
            {
                throw new CannotGetBrowserInfoException("Can not get the full screen status: " + ex.ToString());
            }
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
                using (Microsoft.Win32.RegistryKey versionKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(TestConstants.IE_Reg_Path))
                {
                    _version = versionKey.GetValue("Version").ToString();
                }
            }

            return _version;
        }

        /* int GetLoadSeconds()
         * return the seconds time used to load  current page.
         */
        public virtual float GetPerformanceTimeForCurrentPage()
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
                    return GetPerformanceTimeByUrl(GetUrl());
                }
            }

            return -1;
        }

        /* GetLoadSecondsByURL(Uri uri)
         * get the seconds used to load the url.
         */
        public virtual float GetPerformanceTimeByUrl(string url)
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
        protected float GetPerformanceTime()
        {
            return (float)DateTime.Now.Second + ((float)DateTime.Now.Millisecond) / 1000;
        }

        /* string GetHTML()
         * return the HTML code of current page.
         */
        public virtual string GetHTMLContent()
        {
            HTMLDocument[] allDocs = GetAllDocuments();
            StringBuilder sb = new StringBuilder();
            foreach (HTMLDocument doc in allDocs)
            {
                try
                {
                    if (doc.body != null && doc.body.innerHTML != null)
                    {
                        sb.Append(doc.body.innerHTML);
                    }
                }
                catch
                {
                    continue;
                }
            }
            return sb.ToString();
        }

        public virtual HTMLDocument GetDocument()
        {
            if (_browser != null)
            {
                _rootDocument = _browser.Document as HTMLDocument;
                return _rootDocument;
            }

            return null;
        }

        //return all documents, include frames.
        public virtual HTMLDocument[] GetAllDocuments()
        {
            GetDocument();
            return GetAllDocuments(_rootDocument);
        }

        protected virtual HTMLDocument[] GetAllDocuments(HTMLDocument root)
        {
            if (root != null)
            {
                List<HTMLDocument> res = new List<HTMLDocument>();
                res.Add(root);
                try
                {
                    if (root != null && root.frames != null)
                    {
                        IHTMLFramesCollection2 frames = root.frames;
                        for (int i = 0; i < frames.length; i++)
                        {
                            object index = i;
                            IHTMLWindow2 frame = frames.item(ref index) as IHTMLWindow2;
                            HTMLDocument temp = COMUtil.GetFrameDocument(frame);
                            HTMLDocument[] curFrameDocs = GetAllDocuments(temp);
                            if (curFrameDocs != null)
                            {
                                res.AddRange(curFrameDocs);
                            }
                        }
                    }
                }
                catch
                {
                }
                return res.ToArray();
            }

            return null;
        }

        public ITestBrowser GetPage(int index)
        {
            try
            {
                if (index < 0)
                {
                    index = 0;
                }
                //we will try 30s to find an object.
                int times = 0;
                while (times <= _maxWaitSeconds)
                {
                    if (index >= 0 && index < _browserList.Count)
                    {
                        InternetExplorer ie = _browserList[index];
                        SetBrowser(ie);
                        return this;
                    }

                    Thread.Sleep(Interval * 1000);
                    times += Interval;
                }

                throw new BrowserNotFoundException("Can not find page by index: " + index);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BrowserNotFoundException("Can not find page by index: " + index + " with exception: " + ex.ToString());
            }
        }

        public ITestBrowser GetPage(string title, string url)
        {
            try
            {
                if (String.IsNullOrEmpty(title) && String.IsNullOrEmpty(url))
                {
                    throw new BrowserNotFoundException("Page title and url can not both be empty.");
                }

                //we will try 30s to find an object.
                int times = 0;
                while (times <= _maxWaitSeconds)
                {
                    if (_browserList.Count > 0)
                    {
                        foreach (InternetExplorer ie in _browserList)
                        {
                            String curTitle = ie.LocationName;
                            String curUrl = ie.LocationURL;
                            if ((String.IsNullOrEmpty(title) || String.Compare(curTitle, title, true) == 0) &&
                                (String.IsNullOrEmpty(url) || String.Compare(url, curUrl, true) == 0))
                            {
                                SetBrowser(ie);
                                return this;
                            }
                        }
                    }

                    Thread.Sleep(Interval * 1000);
                    times += Interval;
                }

                throw new BrowserNotFoundException("Can not find page by title: " + title + " and url: " + url);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BrowserNotFoundException("Can not find page by title: " + title + " and url: " + url + " with exception: " + ex.ToString());
            }
        }

        public ITestBrowser GetMostRecentPage()
        {
            try
            {
                InternetExplorer ie = _browserList[_browserList.Count - 1];
                SetBrowser(ie);
                return this;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not get most recent browser: " + ex.ToString());
            }
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

            int times = 0;
            while (times <= seconds)
            {
                bool browserFound = false;

                Process[] pArr = Process.GetProcessesByName(TestConstants.IE_Process_Name);
                foreach (Process p in pArr)
                {
                    //if title is empty, find by process id.
                    if (String.IsNullOrEmpty(title))
                    {
                        if (_appProcess != null && _appProcess.Id == p.Id)
                        {
                            browserFound = true;
                        }
                    }
                    else if (p.MainWindowTitle.IndexOf(title, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        browserFound = true;
                    }

                    if (browserFound)
                    {
                        if (p.MainWindowHandle == IntPtr.Zero)
                        {
                            //not ready, try again.
                            break;
                        }

                        _appProcess = p;
                        _rootHandle = p.MainWindowHandle;
                        _browser = AttachBrowser(_rootHandle);
                        _browserExisted.Set();

                        return;
                    }
                }

                //sleep for 3 seconds, find again.
                times += Interval;
                Thread.Sleep(Interval * 1000);
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
                if (this.GetPrevBrowser() != ie)
                {
                    _browserList.Add(ie);
                    break;
                }
                else
                {
                    Thread.Sleep(Interval * 1000);
                    times += Interval;
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
                if (allBrowsers != null && allBrowsers.Length > 0)
                {
                    for (int i = 0; i < allBrowsers.Length; i++)
                    {
                        InternetExplorer tempIE = allBrowsers[i];
                        try
                        {
                            if (tempIE != null && (int)ieHandle == tempIE.HWND)
                            {
                                return tempIE;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                times += Interval;
                Thread.Sleep(Interval * 1000);
            }

            throw new CannotAttachBrowserException("Browser: " + ieHandle + " does not exist.");
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
                for (int i = allBrowsers.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        InternetExplorer curIE = (InternetExplorer)allBrowsers.Item(i);
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
                for (int i = allBrowsers.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        InternetExplorer curIE = (InternetExplorer)allBrowsers.Item(i);
                        if (curIE != null && curIE.Document is IHTMLDocument)
                        {
                            ieList.Add(curIE);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                return ieList.ToArray();
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
                    this._browser = ie;
                    this._rootHandle = (IntPtr)ie.HWND;
                    try
                    {
                        this._rootDocument = ie.Document as HTMLDocument;
                    }
                    catch
                    {
                    }
                    //register event.
                    RegBrowserEvent(ie);
                    GetSize();

                    if (!_browserList.Contains(ie))
                    {
                        _browserList.Add(ie);
                    }
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotAttachBrowserException("Can not set browser: " + ex.ToString());
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
        protected virtual InternetExplorer GetPrevBrowser()
        {
            if (_browserList.Count > 0)
            {
                return _browserList[_browserList.Count - 1];
            }

            return this._browser;
        }

        #region get size

        /* void GetSize()
         * Get all size information of browser.
         */
        protected virtual void GetSize()
        {
            if (this._browser != null && !this._browser.Busy)
            {
                GetBrowserRect();
                GetClientRect();
                GetScrollRect();
            }
        }

        /* void GetBrowserRect()
         * browser rect: the whole rect of IE, include menu, address bar etc.
         */
        protected void GetBrowserRect()
        {
            try
            {
                _browserLeft = _browser.Left;
                _browserTop = _browser.Top;
                _browserWidth = _browser.Width;
                _browserHeight = _browser.Height;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not get the rect: " + ex.ToString());
            }
        }


        /* void GetClientRect()
         * client rect: the web page rect, not include menu, address bar ect.
         */
        protected void GetClientRect()
        {
            _rootHandle = GetMainHandle();

            if (_rootHandle == IntPtr.Zero)
            {
                throw new BrowserNotFoundException("Can not get main handle of test browser.");
            }

            _shellDocHandle = GetShellDocHandle(_rootHandle);
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
                throw new CannotAttachBrowserException("Can not get client size of test browser: " + ex.ToString());
            }
        }

        /* void GetScrollRect()
         * the whole web page rect, include invisible part, for example, some web pages are too long to display, we need to scroll them.
         * 
         */
        protected void GetScrollRect()
        {
            //if (_ie != null && _ie.Document != null)
            //{
            //    try
            //    {
            //        _rootDocument = (HTMLDocument)_ie.Document;
            //        HTMLBody bodyElement = (HTMLBody)_rootDocument.body;
            //        _scrollWidth = bodyElement.scrollWidth;
            //        _scrollHeight = bodyElement.scrollHeight;

            //        // scrollLeft means the left that Can Not been seen, scrollTop the same.
            //        _scrollLeft = bodyElement.scrollLeft;
            //        _scrollTop = bodyElement.scrollTop;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new CannotAttachBrowserException("Can not get scroll size: " + ex.ToString());
            //    }
            //}
        }

        #endregion

        #region get handles of each windows control
        /* IntPtr GetIEMainHandle()
         * return the handle of IEFrame, this is the parent handle of Internet Explorer. 
         * we can use Spy++ to get the the handle.
         */
        protected IntPtr GetMainHandle()
        {
            if (_rootHandle == IntPtr.Zero)
            {
                _rootHandle = Win32API.FindWindow(TestConstants.IE_IEframe, null);
            }

            return _rootHandle;
        }

        /* IntPtr GetShellDocHandle(IntPtr mainHandle)
         * return the handle of Shell DocObject View.
         * we can use Spy++ to get the tree structrue of Internet Explorer handles. 
         */
        protected IntPtr GetShellDocHandle(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
            {
                mainHandle = GetMainHandle();
            }

            //lower version than IE 7.0
            if (_majorVersion < 7)
            {
                _shellDocHandle = Win32API.FindWindowEx(mainHandle, IntPtr.Zero, TestConstants.IE_ShellDocView_Class, null);
            }
            else if (_majorVersion == 7)
            {
                IntPtr tabWindow = IntPtr.Zero;
                //get the active tab.
                while (!Win32API.IsWindowVisible(tabWindow))
                {
                    tabWindow = Win32API.FindWindowEx(mainHandle, tabWindow, TestConstants.IE_TabWindow_Class, null);
                }

                _shellDocHandle = Win32API.FindWindowEx(tabWindow, IntPtr.Zero, TestConstants.IE_ShellDocView_Class, null);
            }
            else if (_majorVersion == 8)
            {
                IntPtr frame = Win32API.FindWindowEx(mainHandle, IntPtr.Zero, TestConstants.IE_FrameTab_Class, null);

                if (frame != IntPtr.Zero)
                {
                    IntPtr tabWindow = IntPtr.Zero;
                    //get the active tab.
                    while (!Win32API.IsWindowVisible(tabWindow))
                    {
                        tabWindow = Win32API.FindWindowEx(frame, tabWindow, TestConstants.IE_TabWindow_Class, null);
                    }
                    _shellDocHandle = Win32API.FindWindowEx(tabWindow, IntPtr.Zero, TestConstants.IE_ShellDocView_Class, null);
                }
            }

            return _shellDocHandle;
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

            return Win32API.FindWindowEx(shellHandle, IntPtr.Zero, TestConstants.WIN_Dialog_Class, null);
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

            _ieServerHandle = Win32API.FindWindowEx(shellHandle, IntPtr.Zero, TestConstants.IE_Server_Class, null);
            return _ieServerHandle;
        }

        /* IntPtr GetDialogHandle(IntPtr mainHandle)
         * return the handle of pop up page.
         * the name is Internet Explorer_TridentDlgFrame.
         */
        protected IntPtr GetDialogHandle(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
            {
                mainHandle = GetMainHandle();
            }

            IntPtr popHandle = Win32API.FindWindow(TestConstants.IE_Dialog_Class, null);
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

        protected void CalPerformanceTime()
        {
            string key = GetUrl();
            _endTime = GetPerformanceTime();
            _performanceTimeHT.Add(key, _endTime - _startTime);
            _startTime = _endTime = 0;
        }

        #endregion

        #endregion

        #region event

        /*  void RegBrowserEvent()
        *  register IE event.
        */
        protected virtual void RegBrowserEvent(InternetExplorer ie)
        {
            object isRegistered = ie.GetProperty(TestConstants.IE_ALREADY_REGISTERED);
            if (isRegistered == null)
            {
                RegDownloadEvent(ie);
                RegDocumentCompleteEvent(ie);
                RegNavigateEvent(ie);
                RegRectChangeEvent(ie);
                RegNewWindowEvent(ie);
                RegQuitEvent(ie);
                ie.PutProperty(TestConstants.IE_ALREADY_REGISTERED, true);
            }
        }

        protected virtual void RegQuitEvent(InternetExplorer ie)
        {
            try
            {
                ie.OnQuit += new DWebBrowserEvents2_OnQuitEventHandler(OnQuit);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register Quit event: " + ex.ToString());
            }
        }

        protected virtual void RegDownloadEvent(InternetExplorer ie)
        {
            try
            {
                ie.DownloadBegin += new DWebBrowserEvents2_DownloadBeginEventHandler(OnDownloadBegin);
                ie.DownloadComplete += new DWebBrowserEvents2_DownloadCompleteEventHandler(OnDownloadComplete);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register Downloadbegin event: " + ex.ToString());
            }
        }

        protected virtual void RegNewWindowEvent(InternetExplorer ie)
        {
            try
            {
                //ie.NewWindow3 += new DWebBrowserEvents2_NewWindow3EventHandler(OnNewWindow3);
                ie.NewWindow2 += new DWebBrowserEvents2_NewWindow2EventHandler(OnNewWindow2);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register new window event: " + ex.ToString());
            }
        }

        protected virtual void RegDocumentCompleteEvent(InternetExplorer ie)
        {
            try
            {
                ie.DocumentComplete += new DWebBrowserEvents2_DocumentCompleteEventHandler(OnDocumentLoadComplete);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register document complete event: " + ex.ToString());
            }
        }

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
                throw new CannotAttachBrowserException("Can not register Rect change event: " + ex.ToString());
            }
        }

        protected virtual void RegNavigateEvent(InternetExplorer ie)
        {
            try
            {
                ie.BeforeNavigate2 += new DWebBrowserEvents2_BeforeNavigate2EventHandler(OnBeforeNavigate2);
                //ie.NavigateError += new DWebBrowserEvents2_NavigateErrorEventHandler(OnNavigateError);
                ie.NavigateComplete2 += new DWebBrowserEvents2_NavigateComplete2EventHandler(OnNavigateComplete2);
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Can not register navigate event: " + ex.ToString());
            }
        }

        #region callback functions for each event

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
                throw new CannotAttachBrowserException("Can not attach new window: " + ex.ToString());
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
                throw new CannotAttachBrowserException(ex.ToString());
            }
        }

        protected virtual void OnBeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
        {
            try
            {
                if (_startTime == 0)
                {
                    _startTime = GetPerformanceTime();
                }
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Error OnBeforeNavigate2: " + ex.ToString());
            }
        }

        protected virtual void OnNavigateComplete2(object pDisp, ref object URL)
        {
        }

        /* void OnDownloadBegin()
         * the callback function to handle the browser starting download a web page.
         */
        protected virtual void OnDownloadBegin()
        {
        }

        protected virtual void OnDownloadComplete()
        {
        }

        /* void OnDocumentLoadComplete(object pDesp, ref object pUrl)
         * the callback function to hanle the browser finishing download a web page.
         * when downloading complete, we can start to calculate the position of the web page.
         */
        protected virtual void OnDocumentLoadComplete(object pDesp, ref object pUrl)
        {
            try
            {
                if (pUrl == null || String.Compare(pUrl.ToString(), TestConstants.IE_BlankPage_Url, true) == 0)
                {
                    return;
                }

                if (_browser != null && _browser.ReadyState == tagREADYSTATE.READYSTATE_COMPLETE && Marshal.GetIUnknownForObject(pDesp) == Marshal.GetIUnknownForObject(_browser))
                {
                    _rootDocument = _browser.Document as HTMLDocument;
                    GetSize();
                    CalPerformanceTime();
                    _documentLoadComplete.Set();
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Error OnDocumentLoadComplete: " + ex.ToString());
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
                throw new CannotAttachBrowserException("Error OnRectChanged: " + ex.ToString());
            }
        }

        /* void OnQuit()
         * when close a browser.
         */
        protected virtual void OnQuit()
        {
            //get the prev test browser.
            _browserList.Remove(this._browser);
            if (_browserList.Count > 0)
            {
                InternetExplorer ie = _browserList[0];
                SetBrowser(ie);
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
