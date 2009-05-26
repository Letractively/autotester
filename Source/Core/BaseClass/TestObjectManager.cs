using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestObjectManager : TestObjectMap, ITestWindowMap
    {
        #region ctor

        public TestObjectManager(TestApp app)
            : base(app)
        {
        }

        public TestObjectManager(TestBrowser browser)
            : base(browser)
        {
        }

        #endregion

        #region page and window

        public ITestObjectMap Page(int index)
        {
            if (index >= 0)
            {
                ITestBrowser browser = _browser.GetPage(index);
                if (browser != null)
                {
                    this._objPool = browser.GetObjectPool();
                    return this;
                }
            }

            throw new BrowserNotFoundException("Can not get page by index: " + index);
        }

        public ITestObjectMap Page(string title, string url)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(url))
            {
                ITestBrowser browser = _browser.GetPage(title, url);
                if (browser != null)
                {
                    this._objPool = browser.GetObjectPool();
                    return this;
                }
            }

            throw new BrowserNotFoundException("Can not get page by title: " + title + ", url: " + url);
        }

        public ITestObjectMap NewPage()
        {
            ITestBrowser browser = _browser.GetMostRecentPage();
            if (browser != null)
            {
                this._objPool = browser.GetObjectPool();
                return this;
            }

            throw new BrowserNotFoundException("Can not get new page.");
        }

        public ITestObjectMap Window(int index)
        {
            if (index >= 0)
            {
                ITestApp app = _app.GetWindow(index);
                if (app != null)
                {
                    this._objPool = app.GetObjectPool();
                    return this;
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by index: " + index);
        }

        public ITestObjectMap Window(string title, string className)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(className))
            {
                ITestApp app = _app.GetWindow(title, className);
                if (app != null)
                {
                    this._objPool = app.GetObjectPool();
                    return this;
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by title: " + title + ", className: " + className);
        }

        public ITestObjectMap NewWindow()
        {
            ITestApp app = _app.GetMostRecentWindow();
            if (app != null)
            {
                this._objPool = app.GetObjectPool();
                return this;
            }

            throw new AppNotFoundExpcetion("Can not find new window.");
        }

        #endregion
    }
}
