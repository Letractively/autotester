using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Core
{
    public class TestWindowMap : ITestWindowMap
    {
        #region fields

        protected TestApp _app;
        protected ITestObjectPool _objPool;
        protected TestObjectMap _map;

        #endregion

        #region ctor

        public TestWindowMap(TestApp app)
        {
            this._app = app;
            this._objPool = app.GetObjectPool();
            this._map = app.GetObjectMap() as TestObjectMap;
        }

        #endregion

        #region ITestWindowMap Members

        public virtual ITestObjectMap Window(int index)
        {
            if (index >= 0)
            {
                ITestApp app = _app.GetWindow(index);
                if (app != null)
                {
                    this._objPool = app.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map;
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by index: " + index);
        }

        public virtual ITestObjectMap Window(string title, string className)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(className))
            {
                ITestApp app = _app.GetWindow(title, className);
                if (app != null)
                {
                    this._objPool = app.GetObjectPool();
                    _map.ObjectPool = this._objPool;
                    return _map;
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by title: " + title + ", className: " + className);
        }

        public virtual ITestObjectMap NewWindow()
        {
            ITestApp app = _app.GetMostRecentWindow();
            if (app != null)
            {
                this._objPool = app.GetObjectPool();
                _map.ObjectPool = this._objPool;
                return _map;
            }

            throw new AppNotFoundExpcetion("Can not find new window.");
        }

        #endregion

    }
}
