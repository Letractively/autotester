using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestSession : TestSession
    {
        #region fields

        public new HTMLTestBrowser Browser
        {
            get
            {
                return _browser as HTMLTestBrowser;
            }
        }

        public new HTMLTestObjectMap Objects
        {
            get
            {
                return _objectMap as HTMLTestObjectMap;
            }
        }

        public new HTMLTestPageMap Pages
        {
            get
            {
                return _pageMap as HTMLTestPageMap;
            }
        }

        public new HTMLTestEventDispatcher Event
        {
            get
            {
                return _dispatcher as HTMLTestEventDispatcher;
            }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestSession()
        {
            this._browser = new HTMLTestBrowser();
            Init();
        }

        #endregion

        #endregion
    }
}
