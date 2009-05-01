using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestApp : TestApp
    {
        #region fields

        private static MSAAEventDispatcher _dispacher;
        private static MSAATestObjectPool _objectPool;

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
                _dispacher = MSAAEventDispatcher.GetInstance();
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

        #endregion
    }
}
