using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestPageMap : ITestPageMap
    {
        #region fields

        protected TestBrowser _browser;
        protected ITestObjectPool _objPool;
        protected TestObjectMap _map;

        #endregion

        #region ctor

        public TestPageMap(TestBrowser browser)
        {
            this._browser = browser;
            this._objPool = browser.GetObjectPool();
            this._map = browser.GetObjectMap() as TestObjectMap;
        }

        #endregion

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
    }
}
