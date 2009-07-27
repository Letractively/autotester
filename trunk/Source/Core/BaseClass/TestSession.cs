using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Core
{
    public class TestSession : ITestSession
    {
        #region fields

        protected TestApp _app;
        protected TestBrowser _browser;
        protected TestObjectMap _objectMap;
        protected TestCheckPoint _cp;
        protected ITestEventDispatcher _dispatcher;

        #endregion

        #region properties

        public virtual ITestApp App
        {
            get { return _app; }
        }

        public virtual ITestBrowser Browser
        {
            get { return _browser; }
        }

        public virtual ITestObjectMap Objects
        {
            get
            {
                if (this._browser != null)
                {
                    this._objectMap = this._browser.CurrentPage.Objects as TestObjectMap;
                }
                else if (this._app != null)
                {
                    this._objectMap = this._app.CurrentWindow.Objects as TestObjectMap;
                }

                return _objectMap;
            }
        }

        public virtual ITestEventDispatcher Event
        {
            get
            {
                if (this._browser != null)
                {
                    this._dispatcher = _browser.GetEventDispatcher();
                }
                else if (this._app != null)
                {
                    this._dispatcher = _app.GetEventDispatcher();
                }

                return this._dispatcher;
            }
        }

        public virtual ITestCheckPoint CheckPoint
        {
            get { return _cp; }
        }

        #endregion

        #region methods

        #region ctor

        public TestSession()
        {
        }

        public TestSession(TestApp application)
        {
            this._app = application;
        }

        public TestSession(TestBrowser browser)
        {
            this._browser = browser;
        }

        #endregion

        #region public methods
        #endregion

        #region private methods
        #endregion

        #endregion
    }
}
