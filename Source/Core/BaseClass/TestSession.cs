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
        protected TestObjectMap _map;
        protected TestCheckPoint _cp;
        protected ITestObjectPool _pool;
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

        public ITestObjectPool ObjectPool
        {
            get
            {
                Init();
                return _pool;
            }
        }


        public TestObjectMap ObjectMap
        {
            get
            {
                Init();
                return _map;
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
            if ((this._app != null || this._browser != null) && this._pool == null)
            {
                if (this._app != null)
                {
                    this._map = new TestObjectMap(this._app);
                    this._pool = _app.GetObjectPool();
                    this._dispatcher = _app.GetEventDispatcher();
                }

                if (this._browser != null)
                {
                    this._map = new TestObjectMap(this._browser);
                    this._pool = _browser.GetObjectPool();
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
