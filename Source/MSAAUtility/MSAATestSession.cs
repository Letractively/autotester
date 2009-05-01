using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestSession : TestSession
    {
        #region fields
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
