using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestSession : TestSession
    {
        #region fields

        public new HTMLTestObjectManager Objects
        {
            get
            {
                Init();
                return _objManager as HTMLTestObjectManager;
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
