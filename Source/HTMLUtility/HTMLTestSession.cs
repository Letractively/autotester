using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestSession : TestSession
    {
        #region fields

        public new HTMLTestObjectMap Objects
        {
            get
            {
                Init();
                return _objectMap as HTMLTestObjectMap;
            }
        }

        public new HTMLTestPageMap Pages
        {
            get
            {
                Init();
                return _pageMap as HTMLTestPageMap;
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
