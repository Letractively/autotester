using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Core.TestExceptions;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestWindow : TestWindow
    {
        #region fields

        protected MSAATestObjectPool _objectPool;
        protected MSAATestObject _rootObj;

        #region properties

        public new MSAATestObjectMap Objects
        {
            get
            {
                return GetObjectMap() as MSAATestObjectMap;
            }
        }

        public MSAATestObject RootObject
        {
            get { return _rootObj; }
            set { _rootObj = value; }
        }

        #endregion

        #endregion

        #region methods

        #region ctor

        public MSAATestWindow(IntPtr handle, MSAATestApp app)
            : base(handle, app)
        {
        }


        #endregion

        public override ITestObjectPool GetObjectPool()
        {
            if (_objectPool == null)
            {
                _rootObj = new MSAATestObject(this._handle);
                _objectPool = new MSAATestObjectPool(_rootObj);
                _objectPool.SetTestWindow(this);
            }

            return _objectPool;
        }

        public override ITestObjectMap GetObjectMap()
        {
            if (_objMap == null)
            {
                ITestObjectPool pool = GetObjectPool();
                _objMap = new MSAATestObjectMap(pool as MSAATestObjectPool);
            }

            return _objMap as MSAATestObjectMap;
        }

        #endregion

    }
}
