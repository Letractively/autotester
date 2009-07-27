using System;
using System.Collections.Generic;
using System.Text;

using Accessibility;
using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.MSAAUtility;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestActiveXObject : HTMLTestGUIObject, IMSAA, IWindows
    {
        #region fields

        protected IHTMLObjectElement _objElement;

        protected IntPtr _mainHandle;
        protected MSAATestObject _rootObj;
        protected MSAATestObjectMap _msaaObjMap;

        public MSAATestObjectMap WinObjects
        {
            get { return _msaaObjMap; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestActiveXObject()
            : base()
        {
        }

        public HTMLTestActiveXObject(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestActiveXObject(IHTMLElement element, HTMLTestPage page)
            : base(element, page)
        {
            try
            {
                _objElement = element as IHTMLObjectElement;
                this._rootObj = new MSAATestObject(element);

                this._isEnable = IsEnable();
                this._isVisible = IsVisible();
                this._isReadonly = false;

                if (!_sendMsgOnly)
                {
                    GetRectOnScreen();
                }

                InitMSAA();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build html activex object: " + ex.ToString());
            }
        }

        #endregion

        private void InitMSAA()
        {
            MSAATestObjectPool pool = new MSAATestObjectPool(this._rootObj);
            _msaaObjMap = new MSAATestObjectMap(pool);
        }

        #region IMSAA Members

        public override IAccessible GetIAccInterface()
        {
            return this._rootObj.IAcc;
        }

        public override int GetChildID()
        {
            return this._rootObj.ChildID;
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

        #endregion

    }
}
