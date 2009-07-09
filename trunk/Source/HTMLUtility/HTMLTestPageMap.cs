using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestPageMap : TestPageMap
    {
        #region fileds

        #endregion

        #region methods

        #region ctor

        public HTMLTestPageMap(HTMLTestBrowser browser)
            : base(browser)
        {
        }

        #endregion

        public new HTMLTestObjectMap Page(int index)
        {
            if (index >= 0)
            {
                HTMLTestBrowser browser = _browser.GetPage(index) as HTMLTestBrowser;
                if (browser != null)
                {
                    this._objPool = browser.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map as HTMLTestObjectMap;
                }
            }

            throw new BrowserNotFoundException("Can not get page by index: " + index);
        }

        public new HTMLTestObjectMap Page(string title, string url)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(url))
            {
                HTMLTestBrowser browser = _browser.GetPage(title, url) as HTMLTestBrowser;
                if (browser != null)
                {
                    this._objPool = browser.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map as HTMLTestObjectMap;
                }
            }

            throw new BrowserNotFoundException("Can not get page by title: " + title + ", url: " + url);
        }

        public new HTMLTestObjectMap NewPage()
        {
            HTMLTestBrowser browser = _browser.GetMostRecentPage() as HTMLTestBrowser;
            if (browser != null)
            {
                this._objPool = browser.GetObjectPool();
                _map.ObjectPool = this._objPool;
                return _map as HTMLTestObjectMap;
            }

            throw new BrowserNotFoundException("Can not get new page.");
        }

        #endregion
    }
}
