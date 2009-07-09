using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.MSAAUtility;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestDialog : HTMLTestGUIObject, IMSAA, IWindows
    {
        #region fields

        protected IntPtr _mainHandle;
        protected MSAATestObject _mainObj;

        #endregion

        #region methods

        #region cotr
        public HTMLTestDialog()
            : base()
        {
        }

        public HTMLTestDialog(IntPtr handle)
            : this(handle, null)
        {
        }

        public HTMLTestDialog(IntPtr handle, HTMLTestBrowser browser)
        {
            if (handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Can not build windows dialog: handle can not be 0.");
            }

            _mainObj = new MSAATestObject(handle);

            if (!_sendMsgOnly)
            {
                GetRectOnScreen();
            }
        }
        #endregion

        #endregion

        #region IMSAA Members

        public override Accessibility.IAccessible GetIAccInterface()
        {
            throw new NotImplementedException();
        }

        public override int GetChildID()
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
