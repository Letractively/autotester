using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.MSAAUtility;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestActiveXObject : HTMLTestGUIObject, IMSAA, IWindows
    {
        #region fields

        MSAATestObject _rootObj;

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

        public HTMLTestActiveXObject(IHTMLElement element, HTMLTestBrowser browser)
            : base(element, browser)
        {
            if (!_sendMsgOnly)
            {
                GetRectOnScreen();
            }
            this._rootObj = new MSAATestObject(element);
            this._isEnable = IsEnable();
            this._isReadonly = IsReadonly();
            this._isVisible = IsVisible();
        }

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
        #endregion

    }
}
