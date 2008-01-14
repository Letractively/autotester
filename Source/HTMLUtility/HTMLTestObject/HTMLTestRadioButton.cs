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
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestRadioButton : HTMLTestGUIObject, ICheckable
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

            this._type = HTMLTestObjectType.RadioButton;

            try
            {
                this._radioElement = (IHTMLInputElement)element;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not convert to IHTMLInputElement: " + e.Message);
            }
        }

        #endregion

        #region public methods

        #region ICheckable Members

        public void Check()
        {
            try
            {
                if (!IsChecked())
                {
                    Click();
                }
            }
            catch (CannotPerformActionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not perform Check action on radio button: " + e.Message);
            }

        }

        public void UnCheck()
        {
            try
            {
                if (IsChecked())
                {
                    Click();
                }
            }
            catch (CannotPerformActionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not perform UnCheck action on radio button: " + e.Message);
            }
        }

        public bool IsChecked()
        {
            try
            {
                return _radioElement.@checked;
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not get status of radio button: " + e.Message);
            }
        }

        #endregion

        #region IClickable Members

        public void Click()
        {
            try
            {
                _actionFinished.WaitOne();

                Hover();

                MouseOp.Click();

                _actionFinished.Set();
            }
            catch (CannotPerformActionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not click on the radio button: " + e.Message);
            }
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
                Hover();
                MouseOp.Click();
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not focus on radiobox: " + e.Message);
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

        /*  string GetAroundText(int position)
         *  return the human readable text around the control.
         *  NOTE: NEED UPDATE!!!
         */
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
