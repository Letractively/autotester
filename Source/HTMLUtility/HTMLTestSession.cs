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
                return base.Objects as HTMLTestObjectMap;
            }
        }

        public new HTMLTestEventDispatcher Event
        {
            get
            {
                return base.Event as HTMLTestEventDispatcher;
            }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestSession()
        {
            this._browser = new HTMLTestBrowser();
        }

        #endregion

        #endregion
    }
}
