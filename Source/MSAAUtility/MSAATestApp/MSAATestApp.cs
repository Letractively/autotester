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

        protected MSAATestEventDispatcher _dispacher;

        #endregion

        #region properties

        public override ITestWindow CurrentWindow
        {
            get
            {
                if (_currentWindow == null)
                {
                    _currentWindow = new MSAATestWindow(this._rootHandle, this);
                }

                return _currentWindow;
            }
        }

        #endregion

        #region methods

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
