/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestRadioButton.cs
*
* Description: This class defines the actions provide by Radio Button.
*              The important actions include "Check" , and "IsChecked".
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

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

        //for radio button, we may have some text around it.
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

        /* void Check()
         * Check the radio button.
         * 
         */
        public void Check()
        {
            try
            {
                _actionFinished.WaitOne();

                Hover();

                MouseOp.Click();

                _actionFinished.Set();

            }
            catch (Exception e)
            {
                throw new CanNotPerformActionException("Can not check radio button: " + e.Message);
            }
        }

        public void UnCheck()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IsChecked()
        {
            return false;
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

        public void PerformDefaultAction(object para)
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
