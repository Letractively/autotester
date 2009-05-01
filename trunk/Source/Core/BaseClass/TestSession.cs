﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestSession
    {
        #region fields

        protected ITestApp _app;
        protected ITestBrowser _browser;
        protected ITestObjectPool _pool;
        protected TestObjectMap _map;
        protected ITestCheckPoint _cp;

        #endregion

        #region properties

        public ITestApp App
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

        public ITestBrowser Browser
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

        public ITestCheckPoint CheckPoint
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
        {
        }

        public TestSession(ITestApp application)
        {
            this._app = application;
        }

        public TestSession(ITestBrowser browser)
        {
            this._browser = browser;
        }

        protected virtual void Init()
        {
            if ((this._app != null || this._browser != null) && this._pool == null)
            {
                if (this._app != null)
                {
                    this._pool = _app.GetObjectPool();
                }

                if (this._browser != null)
                {
                    this._pool = _browser.GetObjectPool();
                }
                this._map = new TestObjectMap(this._pool);
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
