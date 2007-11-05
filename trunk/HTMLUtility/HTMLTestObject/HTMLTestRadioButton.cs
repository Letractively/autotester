using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestRadioButton : HTMLGuiTestObject, ICheckable
    {
        #region fields

        protected IHTMLInputElement _radioElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestRadioButton(IHTMLElement element)
            : base(element)
        {

        }

        #endregion

        #region public methods

        #region ICheckable Members

        public void Check()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void UnCheck()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void IsChecked()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IClickable Members

        public void Click()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DoubleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RightClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void MiddleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetDefaultAction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void PerformDefaultAction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        #region private methods


        #endregion

        #endregion

        #endregion

    }
}
