using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.HTMLUtility
{
    [ComVisible(true)]
    public class HTMLTestDocument : TestIEDocument
    {
        #region fields

        #endregion

        #region property

        public new HTMLTestBrowser Browser
        {
            get { return this._browser as HTMLTestBrowser; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestDocument(IHTMLDocument2 doc)
            : this(doc, null)
        {
        }

        public HTMLTestDocument(IHTMLDocument2 doc, HTMLTestBrowser browser)
            : base(doc, browser)
        {
        }

        #endregion

        #endregion
    }
}
