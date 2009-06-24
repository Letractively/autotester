using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestSession : TestSession
    {
        #region fields

        public new MSAATestObjectMap Objects
        {
            get
            {
                return _objectMap as MSAATestObjectMap;
            }
        }

        public new MSAATestWindowMap Windows
        {
            get
            {
                return _windowMap as MSAATestWindowMap;
            }
        }

        #endregion

        #region methods

        public MSAATestSession()
        {
            this._app = new MSAATestApp();
            Init();
        }

        #endregion
    }
}
