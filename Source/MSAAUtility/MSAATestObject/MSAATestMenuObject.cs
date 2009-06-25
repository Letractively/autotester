using System;
using System.Collections.Generic;
using System.Text;

using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestMenuObject : MSAATestGUIObject
    {
        #region fields
        #endregion

        #region methods

        #region ctor

        public MSAATestMenuObject(IAccessible iAcc)
            : this(iAcc, 0)
        {
        }

        public MSAATestMenuObject(IAccessible iAcc, int childID)
            : base(iAcc, childID)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build menu box: " + ex.ToString());
            }
        }

        public MSAATestMenuObject(IntPtr handle)
            : base(handle)
        {
            try
            {
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build menu box: " + ex.ToString());
            }
        }
        #endregion

        #endregion
    }
}
