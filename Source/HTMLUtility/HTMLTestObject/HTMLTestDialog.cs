using System;
using System.Collections.Generic;
using System.Text;

using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.MSAAUtility;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestDialog : HTMLTestGUIObject, IMSAA, IWindows
    {
        #region fields

        protected IntPtr _mainHandle;
        protected MSAATestObject _mainObj;
        protected MSAATestObjectMap _msaaObjMap;

        public MSAATestObjectMap WinObjects
        {
            get { return _msaaObjMap; }
        }

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

            this._mainHandle = handle;
            InitMSAA();
            if (!_sendMsgOnly)
            {
                GetRectOnScreen();
            }
        }

        protected virtual void InitMSAA()
        {
            try
            {
                _mainObj = new MSAATestObject(_mainHandle);
                MSAATestObjectPool pool = new MSAATestObjectPool(_mainObj);
                _msaaObjMap = new MSAATestObjectMap(pool);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build dialog object, MSAA error: " + ex.ToString());
            }
        }

        #endregion

        #region public

        public override System.Drawing.Rectangle GetRectOnScreen()
        {
            try
            {
                return _mainObj.GetRect();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get object position: " + ex.ToString());
            }
        }

        public override System.Drawing.Point GetCenterPoint()
        {
            try
            {
                return _mainObj.GetCenterPoint();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get object center point: " + ex.ToString());
            }
        }

        #endregion

        #endregion

        #region IMSAA Members

        public override IAccessible GetIAccInterface()
        {
            return this._mainObj.IAcc;
        }

        public override int GetChildID()
        {
            return this._mainObj.ChildID;
        }

        #endregion

        #region IWindows Members

        public IntPtr GetHandle()
        {
            return this._mainHandle;
        }

        public string GetClass()
        {
            return Win32API.GetClassName(this._mainHandle);
        }

        public string GetCaption()
        {
            return Win32API.GetWindowText(this._mainHandle);
        }

        #endregion
    }
}
