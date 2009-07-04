using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.MSAAUtility;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestDialog : HTMLTestGUIObject, IMSAA, IWindows
    {
        #region IMSAA Members

        public Accessibility.IAccessible GetIAccInterface()
        {
            throw new NotImplementedException();
        }

        public int GetChildID()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IWindows Members

        public IntPtr GetHandle()
        {
            throw new NotImplementedException();
        }

        public string GetClass()
        {
            throw new NotImplementedException();
        }

        public string GetCaption()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
