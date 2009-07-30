/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestBrowser.cs
*
* Description: This class defines the the actions to support HTML test.
*              we use HTML DOM to get the object from Internet Explorer.
*
* History: 2007/09/04 wan,yu Init version
*          2008/01/15 wan,yu update, modify some static memebers to instance 
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.MSAAUtility;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestBrowser : TestInternetExplorer
    {
        #region Fileds

        private HTMLTestEventDispatcher _dispatcher;

        #endregion

        #region Properties

        public new HTMLTestPage CurrentPage
        {
            get
            {
                return _currentPage as HTMLTestPage;
            }
        }

        #endregion

        #region Methods

        #region public methods

        #region GetObject methods

        protected override ITestPage GetPage(int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < this._browserList.Count)
            {
                try
                {
                    InternetExplorer ie = _browserList[pageIndex];
                    return new HTMLTestPage(this, ie);
                }
                catch (Exception ex)
                {
                    throw new CannotGetTestPageException(ex.ToString());
                }
            }

            return null;
        }

        public override ITestEventDispatcher GetEventDispatcher()
        {
            if (_dispatcher == null)
            {
                _dispatcher = HTMLTestEventDispatcher.GetInstance();
                _dispatcher.Start(this);
            }
            return _dispatcher;
        }

        #endregion

        #endregion

        #region private help methods

        protected override void RegBrowserEvent(InternetExplorer ie)
        {
            base.RegBrowserEvent(ie);
            if (_dispatcher != null)
            {
                _dispatcher.RegisterEvents(ie.Document as IHTMLDocument2);
            }
        }

        protected override void RegWin32Event(InternetExplorer ie)
        {
            HTMLTestEventMonitor monitor = new HTMLTestEventMonitor();
        }


        #endregion

        #endregion
    }
}
