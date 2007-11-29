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

        protected string _aroundText;

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
            Focus();
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
            try
            {
                base.Hover();
                MouseOp.Click();
            }
            catch (Exception e)
            {
                throw new CanNotPerformActionException("Can not focus on radiobox: " + e.Message);
            }
        }

        public object GetDefaultAction()
        {
            return "Check";
        }

        public void PerformDefaultAction()
        {
            Check();
        }

        #endregion


        #region private methods

        protected virtual string GetAroundText(int position)
        {
            //position : 0 for right, 1 for up, 2 for left, 3 for down

            return null;
        }


        #endregion

        #endregion

        #endregion

    }
}
