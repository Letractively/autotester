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

        private MSAATestObject _rootObj;

        public MSAATestObject RootObject
        {
            get { return _rootObj; }
            set { _rootObj = value; }
        }

        #endregion

        #region methods

        protected override void AfterStart()
        {
            base.AfterStart();
            _rootObj = new MSAATestObject(this._rootHandle);
        }

        protected override void AfterFound()
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

        #endregion
    }
}
