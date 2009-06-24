using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestSession : ITestSession
    {
        #region fields

        protected TestApp _app;
        protected TestBrowser _browser;
        protected TestObjectMap _objectMap;
        protected TestWindowMap _windowMap;
        protected TestPageMap _pageMap;
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
            get { return _objectMap; }
        }

        public virtual ITestWindowMap Windows
        {
            get { return _windowMap; }
        }

        public virtual ITestPageMap Pages
        {
            get { return _pageMap; }
        }

        public virtual ITestEventDispatcher Event
        {
            get { return _dispatcher; }
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
            Init();
        }

        public TestSession(TestBrowser browser)
        {
            this._browser = browser;
            Init();
        }

        protected virtual void Init()
        {
            if (this._objectMap == null)
            {
                try
                {
                    if (this._app != null)
                    {
                        this._objectMap = this._app.GetObjectMap() as TestObjectMap;
                        this._windowMap = this._app.GetWindowMap() as TestWindowMap;
                        this._dispatcher = _app.GetEventDispatcher();
                    }

                    if (this._browser != null)
                    {
                        this._objectMap = this._browser.GetObjectMap() as TestObjectMap;
                        this._pageMap = this._browser.GetPageMap() as TestPageMap;
                        this._dispatcher = _browser.GetEventDispatcher();
                    }
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new TestSessionException(ex.ToString());
                }
            }

            if (this._objectMap == null)
            {
                throw new TestSessionException("Can not start test session");
            }
        }

        #endregion

        #region public methods

        #endregion

        #region private methods
        #endregion

        #endregion
    }
}
