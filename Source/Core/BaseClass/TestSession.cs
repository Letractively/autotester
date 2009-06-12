using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestSession
    {
        #region fields

        protected TestApp _app;
        protected TestBrowser _browser;
        protected TestObjectMap _objectMap;
        protected TestWindowMap _windowMap;
        protected TestWindowMap _pageMap;
        protected TestCheckPoint _cp;
        protected ITestEventDispatcher _dispatcher;

        #endregion

        #region properties

        public TestApp App
        {
            get
            {
                return _app;
            }
            set
            {
                _app = value;
                Init();
            }
        }

        public TestBrowser Browser
        {
            get
            {
                return _browser;
            }
            set
            {
                _browser = value;
                Init();
            }
        }

        public TestObjectMap Objects
        {
            get
            {
                Init();
                return _objectMap;
            }
        }

        public TestWindowMap Windows
        {
            get
            {
                Init();
                return _windowMap;
            }
        }

        public TestWindowMap Pages
        {
            get
            {
                Init();
                return _pageMap;
            }
        }

        public ITestEventDispatcher Event
        {
            get
            {
                Init();
                return _dispatcher;
            }
        }

        public TestCheckPoint CheckPoint
        {
            get
            {
                Init();
                return _cp;
            }
        }


        #endregion

        #region methods

        #region ctor

        public TestSession()
            : this(null)
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

        protected virtual void Init()
        {
            if (this._objectMap == null)
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
                    this._pageMap = this._browser.GetWindowMap() as TestWindowMap;
                    this._dispatcher = _browser.GetEventDispatcher();
                }
            }

            if (this._objectMap == null)
            {
                throw new TestObjectPoolExcpetion("Can not get test object map from application.");
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
