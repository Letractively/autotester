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
                return base.Objects as MSAATestObjectMap;
            }
        }

        #endregion

        #region methods

        public MSAATestSession()
        {
            this._app = new MSAATestApp();
        }

        #endregion
    }
}
