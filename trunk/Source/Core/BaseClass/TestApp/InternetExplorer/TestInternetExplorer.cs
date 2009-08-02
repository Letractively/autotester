using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;
using Accessibility;

using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Core.Interface;


namespace Shrinerain.AutoTester.Core
{
    public class TestInternetExplorer : TestApp, IDisposable, ITestBrowser
    {
        #region Fileds

        //stack to save the browser status.
        protected List<InternetExplorer> _browserList = new List<InternetExplorer>(16);
        //InternetExplorer is under SHDocVw namespace, we use this to attach to a browser.
        protected InternetExplorer _browser;
        protected TestIEPage _currentPage;

        //handle of client area.
        // client area means the area to display web pages, not include the menu, address bar, etc.
        // we can use Spy++ to get these information.
        protected IntPtr _ieServerHandle;
        //handle of shell doc.
        protected IntPtr _shellDocHandle;
        //handle of "showModelessDialog" and "showDialog". They contain HTML, but not a InternetExplorer instance.
        protected IntPtr _dialogHandle;
        protected IntPtr _tabHandle;


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

        #endregion

        #region Properties

        public ITestPage CurrentPage
        {
            get
            {
                return this._currentPage;
            }
        }

        public int Left
        {
            get { return _browserLeft; }
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
            get { return _scrollTop; }
        }
        public int ScrollWidth
        {
            get { return _scrollWidth; }
        }
        public int ScrollHeight
        {
            get { return _scrollHeight; }
        }

        #endregion

        #region Methods

        #region ctor
        //I keep the constructor as "public" because of reflecting
        public TestInternetExplorer()
        {
            //currently, just support internet explorer
            _browserName = TestConstants.IE_Browser_Name;
            _version = GetBrowserVersion();
            _majorVersion = Convert.ToInt32(_version[0].ToString());
        }

        ~TestInternetExplorer()
        {
            // when GC, close AutoResetEvent.
            Dispose();
        }

        public virtual void Dispose()
        {
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
            Start(null, null);
            Load(args);
        }

        public override void Start(string appFullPath, string args)
        {
            try
            {
                BeforeStart();

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = TestConstants.IE_EXE;
                startInfo.Arguments = TestConstants.IE_BlankPage_Url;
                if (!String.IsNullOrEmpty(appFullPath) && appFullPath.ToUpper().EndsWith(".EXE"))
                {
                    startInfo.FileName = appFullPath;
                }
                if (!String.IsNullOrEmpty(args))
                {
                    startInfo.Arguments = args;
                }
                if (_isHide)
                {
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }

                _appProcess = Process.Start(startInfo);

                InternetExplorer ie = WaitForBrowserExist(null, null);
                if (ie != null)
                {
                    AttachBrowser(ie);
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
            finally
            {
                AfterStart();
            }
        }

        public override void Find(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new BrowserNotFoundException("Handle can not be 0.");
            }

            IntPtr oriHandle = handle;

            string title = null;
            while (handle != IntPtr.Zero)
            {
                String className = Win32API.GetClassName(handle);
                if (String.Compare(className, TestConstants.IE_IEframe, true) == 0)
                {
                    title = Win32API.GetWindowText(handle);
                    break;
                }
                else
                {
                    handle = Win32API.GetParent(handle);
                }
            }

            if (!String.IsNullOrEmpty(title))
            {
                Find(title);
            }
            else
            {
                throw new BrowserNotFoundException("Can not find browser by handle: " + oriHandle);
            }
        }

        /*  void Find(object browserTitle)
         *  find an instance of browser by its title.
         *  eg: Google.com.
         */
        public override void Find(string title)
        {
            Find(title, null);
        }

        public override void Find(string title, string url)
        {
            Find(title, url, false);
        }

        public virtual void Find(string title, string url, bool isRegex)
        {
            if (String.IsNullOrEmpty(title))
            {
                throw new CannotAttachBrowserException("Browser title can not be empty.");
            }

            try
            {
                BeforeFound();
                InternetExplorer ie = WaitForBrowserExist(title, url, isRegex);
                if (ie != null)
                {
                    AttachBrowser(ie);
                }
                else
                {
                    throw new BrowserNotFoundException(String.Format("Can not find browser by title:{0}, url:{1}.", title, url));
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
            finally
            {
                AfterFound();
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
                this._documentLoadComplete.WaitOne(AppTimeout * 1000, true);
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
                if (OnBrowserBack != null)
                {
                    OnBrowserBack(this, null);
                }
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
                if (OnBrowserForward != null)
                {
                    OnBrowserForward(this, null);
                }
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
                if (OnBrowserRefresh != null)
                {
                    OnBrowserRefresh(this, null);
                }
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
        protected virtual void CloseDialog()
        {
            if (this._dialogHandle != IntPtr.Zero)
            {
                try
                {
                    WindowsAsstFunctions.CloseWindow(_dialogHandle);
                    this._dialogHandle = IntPtr.Zero;
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

        public virtual void ActiveTab(int tabIndex)
        {
            try
            {
                if (tabIndex < 0)
                {
                    return;
                }

                IntPtr tabHandle = WindowsAsstFunctions.GetTabHandle(_rootHandle, _majorVersion);
                if (tabHandle != IntPtr.Zero)
                {
                    IAccessible iAcc = null;
                    Win32API.AccessibleObjectFromWindow(tabHandle, -4, ref Win32API.IACCUID, ref iAcc);
                    if (iAcc != null)
                    {
                        if (GetBrowserMajorVersion() > 7)
                        {
                            iAcc = iAcc.get_accChild(iAcc.accChildCount) as IAccessible;
                        }

                        if (iAcc != null)
                        {
                            //tablist
                            int childCount = iAcc.accChildCount;
                            //tabs count.
                            if (childCount > 2 && tabIndex < childCount - 1)
                            {
                                //tab we want to focus.
                                IAccessible tabToFocus = iAcc.get_accChild(tabIndex + 1) as IAccessible;
                                if (tabToFocus != null)
                                {
                                    tabToFocus.accDoDefaultAction(0);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public virtual ITestPage[] AllPages()
        {
            if (_browserList != null && _browserList.Count > 0)
            {
                List<ITestPage> pages = new List<ITestPage>();
                for (int i = 0; i < _browserList.Count; i++)
                {
                    try
                    {
                        TestIEPage page = GetPage(i) as TestIEPage;
                        pages.Add(page);
                    }
                    catch
                    {
                        continue;
                    }
                }

                return pages.ToArray();
            }

            return null;
        }

        public virtual ITestPage Page(int index)
        {
            try
            {
                if (index < 0)
                {
                    index = 0;
                }
                //we will try 30s to find an object.
                int times = 0;
                while (times <= AppTimeout)
                {
                    if (index >= 0 && index < _browserList.Count)
                    {
                        InternetExplorer ie = _browserList[index];

                        if (times >= AppTimeout || ie.ReadyState == tagREADYSTATE.READYSTATE_INTERACTIVE ||
                            ie.ReadyState == tagREADYSTATE.READYSTATE_COMPLETE)
                        {
                            return PageChange(index);
                        }
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

        public virtual ITestPage Page(string title, string url)
        {
            try
            {
                if (String.IsNullOrEmpty(title) && String.IsNullOrEmpty(url))
                {
                    throw new BrowserNotFoundException("Page title and url can not both be empty.");
                }

                //we will try 30s to find an object.
                int times = 0;
                while (times <= AppTimeout)
                {
                    if (_browserList.Count > 0)
                    {
                        for (int i = 0; i < _browserList.Count; i++)
                        {
                            InternetExplorer ie = _browserList[i];
                            String curTitle = ie.LocationName;
                            String curUrl = ie.LocationURL;
                            if ((String.IsNullOrEmpty(title) || String.Compare(curTitle, title, true) == 0) &&
                                (String.IsNullOrEmpty(url) || String.Compare(url, curUrl, true) == 0))
                            {
                                return PageChange(i);
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

        protected virtual ITestPage GetPage(int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < this._browserList.Count)
            {
                try
                {
                    InternetExplorer ie = _browserList[pageIndex];
                    return new TestIEPage(this, ie);
                }
                catch (Exception ex)
                {
                    throw new CannotGetTestPageException(ex.ToString());
                }
            }

            return null;
        }

        public virtual int GetPageIndex(ITestPage page)
        {
            if (page != null)
            {
                for (int i = 0; i < _browserList.Count; i++)
                {
                    InternetExplorer ie = _browserList[i];
                    IntPtr curIUnknown = Marshal.GetIUnknownForObject(ie.Document);
                    IntPtr pageIUnknown = Marshal.GetIUnknownForObject(page.Document);

                    if (curIUnknown == pageIUnknown)
                    {
                        return i;
                    }
                }
            }

            return -1;
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

        public virtual bool IsLoading()
        {
            try
            {
                if (_isDownloading || _browser.Busy || _browser.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE)
                {
                    return true;
                }
            }
            catch
            {
            }
            return false;
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

        public override string GetAppName()
        {
            return GetBrowserName();
        }

        /* string GetBrowserName()
         * return the name of the browser. eg: Internet Explorer.
         * 
         */
        public virtual String GetBrowserName()
        {
            return _browserName;
        }

        public override string GetVersion()
        {
            return GetBrowserVersion();
        }

        /* string GetBrowserVersion()
         * return the version number of the browser.
         */
        public virtual String GetBrowserVersion()
        {
            if (String.IsNullOrEmpty(_version))
            {
                _version = WindowsAsstFunctions.GetIEVersion();
            }

            return _version;
        }

        public virtual int GetBrowserMajorVersion()
        {
            String version = GetBrowserVersion();
            return int.Parse(version[0].ToString());
        }

        /* int GetLoadSeconds()
         * return the seconds time used to load  current page.
         */
        public virtual float GetPerformanceTimeForCurrentPage()
        {
            int times = 0;
            while (times < this.AppTimeout)
            {
                if (IsLoading())
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

        #endregion

        #region SYNC

        /* void WaitDocumentLoadComplete(int seconds)
         * wait until the HTML document load completely.
         * default will wait for 120s.
         */
        protected virtual void WaitDocumentLoadComplete()
        {
            WaitDocumentLoadComplete(this.AppTimeout);
        }

        protected virtual void WaitDocumentLoadComplete(int seconds)
        {
            if (seconds < 0)
            {
                seconds = 0;
            }

            _documentLoadComplete.WaitOne(seconds * 1000, true);
        }

        protected virtual InternetExplorer WaitForBrowserExist(string title, string url)
        {
            return WaitForBrowserExist(title, url, false);
        }

        /* void WaitForBrowser(int seconds.)
      * wait for 120 seconds max to detect if IE browser is started.
      * if title is not empty, we need to check the title of browser.
      * we will use "Fuzzy Search" in this method
      */
        protected virtual InternetExplorer WaitForBrowserExist(string title, string url, bool isReg)
        {
            Regex titleReg = null;
            Regex urlReg = null;
            if (isReg)
            {
                if (!String.IsNullOrEmpty(title))
                {
                    titleReg = new Regex(title, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                if (!String.IsNullOrEmpty(url))
                {
                    urlReg = new Regex(url, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
            }

            int times = 0;
            while (times <= AppTimeout)
            {
                bool browserFound = false;

                Process[] pArr = Process.GetProcessesByName(TestConstants.IE_Process_Name);
                for (int i = pArr.Length - 1; i >= 0; i--)
                {
                    Process p = pArr[i];
                    InternetExplorer ie = null;

                    //if all null, use any one.
                    browserFound = _appProcess == null && String.IsNullOrEmpty(title) && String.IsNullOrEmpty(url);

                    //find by process id.
                    if (_appProcess != null)
                    {
                        browserFound = _appProcess.Id == p.Id;
                    }

                    //by title
                    if (!String.IsNullOrEmpty(title))
                    {
                        try
                        {
                            if (isReg)
                            {
                                browserFound = titleReg != null && titleReg.IsMatch(p.MainWindowTitle);
                            }
                            else
                            {
                                browserFound = p.MainWindowTitle.IndexOf(title, StringComparison.CurrentCultureIgnoreCase) >= 0;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    //by URL
                    if (!String.IsNullOrEmpty(url))
                    {
                        ie = GetInternetExplorer(p.Id);
                        if (ie != null)
                        {
                            try
                            {
                                if (isReg)
                                {
                                    browserFound = urlReg != null && urlReg.IsMatch(ie.LocationURL);
                                }
                                else
                                {
                                    browserFound = ie.LocationURL.EndsWith(url, StringComparison.CurrentCultureIgnoreCase);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }

                    if (browserFound)
                    {
                        if (ie == null)
                        {
                            ie = GetInternetExplorer(p.Id);
                        }

                        if (ie != null)
                        {
                            return ie;
                        }
                    }
                }

                //IE8 is mutli process model, we may can not find it by process we started. 
                if (String.IsNullOrEmpty(url))
                {
                    if (GetBrowserMajorVersion() > 7)
                    {
                        url = TestConstants.IE_BlankPage_Url;
                        _appProcess = null;
                    }
                }

                //sleep for 3 seconds, find again.
                times += Interval;
                Thread.Sleep(Interval * 1000);
            }

            return null;
        }

        /* void WaitForNewBrowserSync()
         * Wait until a new browser exist.
         */
        protected virtual void WaitForNewBrowserSync()
        {
            int times = 0;
            while (times < this.AppTimeout)
            {
                InternetExplorer ie = GetTopmostBrowser();
                if (this._browser != ie && this._browser.Document != ie.Document)
                {
                    if (!_browserList.Contains(ie))
                    {
                        _browserList.Add(ie);
                        RegBrowserEvent(ie);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(Interval * 1000);
                    times += Interval;
                }
            }
        }

        /* void WaitForNewBrowserAsyn()
         * Wait until a new browser exist.
         */
        protected virtual void WaitForNewBrowserAsyn()
        {
            Thread ieExistT = new Thread(new ThreadStart(WaitForNewBrowserSync));
            ieExistT.Start();
        }

        //when all documents(including sub frames) load complete.
        protected virtual void AllDocumentComplete()
        {
            GetSize();
            CalPerformanceTime();
            _documentLoadComplete.Set();
            _isDownloading = false;
        }

        #endregion

        #endregion

        #region protected virtual help methods

        /* InternetExplorer AttachBrowser(IntPtr ieHandle)
         * return the instance of InternetExplorer.
         * 
         */
        protected virtual InternetExplorer GetInternetExplorer(int processID)
        {
            if (processID <= 0)
            {
                throw new CannotAttachBrowserException("IE process can not be 0.");
            }

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
                            if (curIE.HWND != 0)
                            {
                                int pid = 0;
                                Win32API.GetWindowThreadProcessId((IntPtr)curIE.HWND, out pid);
                                if (processID == pid)
                                {
                                    return curIE;
                                }
                            }
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

        /* InternetExplorer GetTopmostBrowser()
         * get the current active browser.
         */
        protected virtual InternetExplorer GetTopmostBrowser()
        {
            InternetExplorer[] browsers = GetAllBrowsers();
            if (browsers != null && browsers.Length > 0)
            {
                return browsers[0];
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

        protected virtual void AttachBrowser(InternetExplorer ie)
        {
            if (ie != null && this._browser != ie)
            {
                try
                {
                    if (!_browserList.Contains(ie))
                    {
                        _browserList.Add(ie);
                        RegBrowserEvent(ie);
                    }

                    this._browser = ie;
                    this._rootHandle = (IntPtr)ie.HWND;
                    int pid = Win32API.GetWindowThreadProcessId(_rootHandle);
                    this._appProcess = Process.GetProcessById(pid);

                    if (this._currentPage == null)
                    {
                        this._currentPage = GetPage(0) as TestIEPage;
                    }

                    GetSize();
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotAttachBrowserException("Can not set browser: " + ex.ToString());
                }
            }
        }

        protected virtual ITestPage PageChange(int pageIndex)
        {

            if (pageIndex >= 0 && pageIndex < this._browserList.Count)
            {
                InternetExplorer ie = _browserList[pageIndex];
                try
                {
                    AttachBrowser(ie);
                    ActiveTab(pageIndex);

                    _currentPage = GetPage(pageIndex) as TestIEPage;

                    if (OnSelectPageChange != null)
                    {
                        TestEventArgs e = new TestEventArgs("PageIndex", pageIndex);
                        OnSelectPageChange(this, e);
                    }
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

            return _currentPage;
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
            if (_rootHandle != IntPtr.Zero)
            {
                _shellDocHandle = WindowsAsstFunctions.GetShellDocHandle(_rootHandle);
                if (_shellDocHandle != IntPtr.Zero)
                {
                    _ieServerHandle = WindowsAsstFunctions.GetIEServerHandle(_shellDocHandle);
                    if (_ieServerHandle != IntPtr.Zero)
                    {
                        try
                        {
                            Win32API.Rect tmpRect = new Win32API.Rect();
                            if (Win32API.GetWindowRect(_ieServerHandle, ref tmpRect))
                            {
                                _clientLeft = tmpRect.left;
                                _clientTop = tmpRect.top;
                                _clientWidth = tmpRect.Width;
                                _clientHeight = tmpRect.Height;
                                return;
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
                }
            }

            throw new BrowserNotFoundException("Can not get handles of test browser");
        }

        /* void GetScrollRect()
         * the whole web page rect, include invisible part, for example, some web pages are too long to display, we need to scroll them.
         * 
         */
        protected void GetScrollRect()
        {
            if (_currentPage != null)
            {
                try
                {
                    IHTMLDocument2 doc2 = (_currentPage.GetDocument() as TestIEDocument).Document as IHTMLDocument2;
                    IAccessible pAcc = COMUtil.IHTMLElementToMSAA(doc2.body);
                    if (pAcc != null)
                    {
                        pAcc.accLocation(out _scrollLeft, out _scrollTop, out _scrollWidth, out _scrollHeight, 0);
                    }
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotAttachBrowserException("Can not get scroll size: " + ex.ToString());
                }
            }
        }

        #endregion

        #region get handles of each windows control


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
                RegWin32Event(ie);
                RegDocumentCompleteEvent(ie);
                RegNavigateEvent(ie);
                RegNewWindowEvent(ie);
                RegQuitEvent(ie);
                ie.PutProperty(TestConstants.IE_ALREADY_REGISTERED, true);

                if (OnBrowserAttached != null)
                {
                    TestEventArgs e = new TestEventArgs("Browser", ie);
                    OnBrowserAttached(this, e);
                }
            }
        }

        protected virtual void RegWin32Event(InternetExplorer ie)
        {
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

                string url = URL.ToString();
                if (!String.IsNullOrEmpty(url) && String.Compare(url, TestConstants.IE_BlankPage_Url, true) != 0)
                {
                    if (_isDownloading == false)
                    {
                        _isDownloading = true;
                        if (OnBrowserNavigate != null)
                        {
                            TestEventArgs tea = new TestEventArgs(TestConstants.PROPERTY_URL, url);
                            OnBrowserNavigate(this, tea);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CannotAttachBrowserException("Error OnBeforeNavigate2: " + ex.ToString());
            }
        }

        protected void OnNavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            String exMsg = String.Format("Error code {0} when try to navigate to URL: {1}", StatusCode, URL);
            throw new CannotNavigateException(exMsg);
        }

        protected virtual void OnNavigateComplete2(object pDisp, ref object URL)
        {
        }

        /* void OnDownloadBegin()
         * the callback function to handle the browser starting download a web page.
         */
        protected virtual void OnDownloadBegin()
        {
            _isDownloading = true;
        }

        protected virtual void OnDownloadComplete()
        {
            _isDownloading = false;
        }

        protected virtual void OnDocumentLoadComplete(object pDesp, ref object pUrl)
        {
            if (pUrl == null || String.Compare(pUrl.ToString(), TestConstants.IE_BlankPage_Url, true) == 0)
            {
                return;
            }

            try
            {
                if (Marshal.GetIUnknownForObject(pDesp) == Marshal.GetIUnknownForObject(_browser))
                {
                    AllDocumentComplete();
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

        /* void OnQuit()
         * when close a browser.
         */
        protected virtual void OnQuit()
        {
            //get the previous test browser.
            _browserList.Remove(this._browser);
            if (_browserList.Count > 0)
            {
                InternetExplorer ie = _browserList[0];
                PageChange(0);
            }
        }

        #endregion

        #endregion

        #endregion

        #region ITestBrowser events

        protected override void BeforeStart()
        {
            base.BeforeStart();
            if (OnBrowserStart != null)
                OnBrowserStart(this, null);
        }

        protected override void AfterClose()
        {
            base.AfterClose();
            if (OnBrowserClose != null)
                OnBrowserClose(this, null);
        }

        public event TestAppEventHandler OnBrowserStart;

        public event TestAppEventHandler OnBrowserNavigate;

        public event TestAppEventHandler OnSelectPageChange;

        public event TestAppEventHandler OnBrowserBack;

        public event TestAppEventHandler OnBrowserForward;

        public event TestAppEventHandler OnBrowserRefresh;

        public event TestAppEventHandler OnBrowserClose;

        public event TestAppEventHandler OnBrowserAttached;

        public event TestAppEventHandler OnNewWindow;

        public event TestAppEventHandler OnPopupDialog;

        #endregion
    }
}
