using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestWindowMap : ITestWindowMap
    {
        #region fields

        protected TestApp _app;
        protected TestBrowser _browser;
        protected ITestObjectPool _objPool;
        protected TestObjectMap _map;

        #endregion

        #region ctor

        public TestWindowMap(TestApp app)
        {
            this._app = app;
            this._objPool = app.GetObjectPool();
            this._map = app.GetObjectMap() as TestObjectMap;
        }

        public TestWindowMap(TestBrowser browser)
        {
            this._browser = browser;
            this._objPool = browser.GetObjectPool();
            this._map = browser.GetObjectMap() as TestObjectMap;
        }

        #endregion

        #region ITestWindowMap Members

        public virtual ITestObjectMap Page(int index)
        {
            if (index >= 0)
            {
                ITestBrowser browser = _browser.GetPage(index);
                if (browser != null)
                {
                    this._objPool = browser.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map;
                }
            }

            throw new BrowserNotFoundException("Can not get page by index: " + index);
        }

        public virtual ITestObjectMap Page(string title, string url)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(url))
            {
                ITestBrowser browser = _browser.GetPage(title, url);
                if (browser != null)
                {
                    this._objPool = browser.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map;
                }
            }

            throw new BrowserNotFoundException("Can not get page by title: " + title + ", url: " + url);
        }

        public virtual ITestObjectMap NewPage()
        {
            ITestBrowser browser = _browser.GetMostRecentPage();
            if (browser != null)
            {
                this._objPool = browser.GetObjectPool();
                _map.ObjectPool = this._objPool;
                return _map;
            }

            throw new BrowserNotFoundException("Can not get new page.");
        }

        public virtual ITestObjectMap Window(int index)
        {
            if (index >= 0)
            {
                ITestApp app = _app.GetWindow(index);
                if (app != null)
                {
                    this._objPool = app.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map;
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by index: " + index);
        }

        public virtual ITestObjectMap Window(string title, string className)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(className))
            {
                ITestApp app = _app.GetWindow(title, className);
                if (app != null)
                {
                    this._objPool = app.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map;
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by title: " + title + ", className: " + className);
        }

        public virtual ITestObjectMap NewWindow()
        {
            ITestApp app = _app.GetMostRecentWindow();
            if (app != null)
            {
                this._objPool = app.GetObjectPool();
                _map.ObjectPool = this._objPool;
                return _map;
            }

            throw new AppNotFoundExpcetion("Can not find new window.");
        }

        #endregion

    }
}
