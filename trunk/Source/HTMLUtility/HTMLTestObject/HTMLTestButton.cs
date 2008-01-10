/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestButton.cs
*
* Description: This class defines the actions provide by button.
*              The most important action is Click.
*
* History: 2007/09/04 wan,yu Init version
*          2007/12/21 wan,yu add IHTMLButtonElement for <button> tag. 
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using mshtml;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestButton : HTMLGuiTestObject, IClickable, IShowInfo
    {

        #region fields

        //the text on the button, like "Login"
        protected string _currentStr;

        //the HTML element of the object. we build HTMLTestButton based on the HTML element.
        protected IHTMLInputElement _inputElement;
        protected IHTMLButtonElement _buttonElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestButton(IHTMLElement element)
            : base(element)
        {
            try
            {
                // get the text , in HTML , it is .value property.
                this._currentStr = element.getAttribute("value", 0).ToString();
            }
            catch
            {
                this._currentStr = "";
            }

            try
            {
                if (String.Compare(element.tagName, "INPUT", true) == 0)
                {
                    this._inputElement = (IHTMLInputElement)element;
                }
                else if (String.Compare(element.tagName, "BUTTON", true) == 0)
                {
                    this._buttonElement = (IHTMLButtonElement)element;
                }
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not build test button: " + e.Message);
            }
        }

        #endregion

        #region public methods

        /* void Click()
         * Click on the button
         */
        public virtual void Click()
        {
            try
            {
                //wait last action finished.
                _actionFinished.WaitOne();

                //move the mouse to the center point of button.
                // see the definition in HTMLGUiTestObject.cs
                Hover();

                MouseOp.Click();

                //action is finished, signal.
                _actionFinished.Set();

            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not perform click action: " + e.Message);
            }
        }

        /* void DoubleClick()
         * Double click on the button
         */
        public virtual void DoubleClick()
        {
            try
            {
                _actionFinished.WaitOne();

                Hover();
                MouseOp.DoubleClick();

                _actionFinished.Set();
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not perform double click action: " + e.Message);
            }
        }

        /* void RightClick()
         * right click on the button
         */
        public virtual void RightClick()
        {
            try
            {
                _actionFinished.WaitOne();

                Hover();
                MouseOp.RightClick();

                _actionFinished.Set();
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not perform right click action: " + e.Message);
            }
        }

        /* void MiddleClick()
         * middle click on the button
         */
        public virtual void MiddleClick()
        {
            throw new CannotPerformActionException("Can not perform middle click.");
        }

        #region IClickable methods
        /* void Focus()
         * make the button get focus, we click it
         */
        public virtual void Focus()
        {
            Click();
        }

        public virtual object GetDefaultAction()
        {
            return "Click";
        }

        public virtual void PerformDefaultAction(object para)
        {
            Click();
        }

        #endregion


        #region IShowInfo methods

        public virtual string GetText()
        {
            return this._currentStr;
        }

        public virtual string GetFontStyle()
        {
            throw new PropertyNotFoundException();
        }

        public virtual string GetFontFamily()
        {
            throw new PropertyNotFoundException();
        }
        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion


    }
}
