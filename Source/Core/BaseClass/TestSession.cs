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
        protected TestObjectManager _objManager;
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

        public TestObjectManager Objects
        {
            get
            {
                Init();
                return _objManager;
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
            if ((this._app != null || this._browser != null) && this._objManager == null)
            {
                if (this._app != null)
                {
                    this._objManager = new TestObjectManager(this._app);
                    this._dispatcher = _app.GetEventDispatcher();
                }

                if (this._browser != null)
                {
                    this._objManager = new TestObjectManager(this._browser);
                    this._dispatcher = _browser.GetEventDispatcher();
                }
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
