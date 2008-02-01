/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTest.cs
*
* Description: This class defines the enter point for HTML tesing.
* 
*
* History: 2008/01/31 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTest
    {
        #region fields

        private HTMLTestBrowser _browser;
        private HTMLTestObjectPool _pool;
        private HTMLTestObjectMap _map;
        private HTMLTestAction _action;
        private HTMLTestVP _vp;

        #endregion

        #region properties

        public HTMLTestBrowser Browser
        {
            get { return _browser; }
        }

        public HTMLTestObjectPool Pool
        {
            get { return _pool; }
        }

        public HTMLTestObjectMap Map
        {
            get { return _map; }
        }

        public HTMLTestAction Action
        {
            get { return _action; }
        }


        public HTMLTestVP Vp
        {
            get { return _vp; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTest()
        {
            _browser = new HTMLTestBrowser();
            _pool = new HTMLTestObjectPool(_browser);
            _map = new HTMLTestObjectMap(_pool);
            _action = new HTMLTestAction();
            _vp = new HTMLTestVP();
        }

        #endregion

        #region public methods
        #endregion

        #region private methods
        #endregion

        #endregion
    }
}
