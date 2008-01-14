/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestCheckBox.cs
*
* Description: This class defines the actions provide by CheckBox.
*              The important actions include "Check","UnCheck".
* 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestCheckBox : HTMLTestGUIObject, ICheckable
    {

        #region fields

        //the HTML element of HTMLTestCheckBox.
        protected IHTMLInputElement _checkBoxElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestCheckBox(IHTMLElement element)
            : base(element)
        {

            this._type = HTMLTestObjectType.CheckBox;

            try
            {
                this._checkBoxElement = (IHTMLInputElement)element;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get IHTMLInputElement: " + e.Message);
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
                throw new CannotPerformActionException("Can not perform Check action on checkbox: " + e.Message);
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
                throw new CannotPerformActionException("Can not perform UnCheck action on checkbox: " + e.Message);
            }
        }

        public bool IsChecked()
        {
            try
            {
                return _checkBoxElement.@checked;
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not get status of checkbox: " + e.Message);
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
                throw new CannotPerformActionException("Can not click on the checkbox: " + e.Message);
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
            throw new Exception("The method or operation is not implemented.");
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

        #endregion

        #region private methods


        #endregion

        #endregion


    }
}
