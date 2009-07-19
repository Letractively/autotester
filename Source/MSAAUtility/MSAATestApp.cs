using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestApp : TestApp
    {
        #region fields

        private MSAATestEventDispatcher _dispacher;
        private MSAATestObjectPool _objectPool;
        private MSAATestObjectMap _objectMap;
        private MSAATestWindowMap _windowMap;

        private MSAATestObject _rootObj;

        public MSAATestObject RootObject
        {
            get { return _rootObj; }
            set { _rootObj = value; }
        }

        #endregion

        #region methods

        public override void AfterStart()
        {
            base.AfterStart();
            _rootObj = new MSAATestObject(this._rootHandle);
        }

        public override void AfterFound()
        {
            base.AfterFound();
            _rootObj = new MSAATestObject(this._rootHandle);
        }

        public override ITestEventDispatcher GetEventDispatcher()
        {
            if (_dispacher == null)
            {
                _dispacher = MSAATestEventDispatcher.GetInstance();
                _dispacher.Start(this);
            }
            return _dispacher;
        }

        public override ITestObjectPool GetObjectPool()
        {
            if (_objectPool == null)
            {
                _objectPool = new MSAATestObjectPool();
                _objectPool.SetTestApp(this);
            }
            return _objectPool;
        }

        public override ITestObjectMap GetObjectMap()
        {
            if (_objectMap == null)
            {
                GetObjectPool();
                _objectMap = new MSAATestObjectMap(_objectPool);
            }
            return _objectMap;
        }

        public override ITestWindowMap GetWindowMap()
        {
            if (_windowMap == null)
            {
                _windowMap = new MSAATestWindowMap(this);
            }
            return _windowMap;
        }

        #endregion
    }
}
