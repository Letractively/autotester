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
*          2007/12/21 wan,yu update, add IHTMLButtonElement for <button> tag. 
*          2008/01/12 wan,yu update, add HTMLTestButtonType.         
*
*********************************************************************/


using System;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    //for HTML test button, we may have <input type="button">, <input type="submit">, <input type="reset">
    public enum HTMLTestButtonType : int
    {
        Custom = 0,
        Submit = 1,
        Reset = 3,
        Unknow = 4
    }

    public class HTMLTestButton : HTMLTestGUIObject, IClickable, IShowInfo, IStatus
    {

        #region fields

        //the text on the button, like "Login"
        protected string _currentStr;

        //the HTML element of the object. we build HTMLTestButton based on the HTML element.
        //<input>
        protected IHTMLInputElement _inputElement;

        //<button>
        protected IHTMLButtonElement _buttonElement;

        protected HTMLTestButtonType _btnType = HTMLTestButtonType.Unknow;

        //if the button type is not "submit" or "reset", we can get method name of "onclick" property.
        protected string _customMethodName;

        protected string _state;

        #endregion

        #region properties

        public HTMLTestButtonType ButtonType
        {
            get { return _btnType; }
        }

        public string CustomMethodName
        {
            get { return _customMethodName; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestButton(IHTMLElement element)
            : base(element)
        {
            this._type = HTMLTestObjectType.Button;

            try
            {
                if (String.Compare(element.tagName, "INPUT", true) == 0)
                {
                    this._inputElement = (IHTMLInputElement)element;

                    this._currentStr = this._inputElement.value;

                    this._btnType = GetButtonType();
                }
                else if (String.Compare(element.tagName, "BUTTON", true) == 0)
                {
                    this._buttonElement = (IHTMLButtonElement)element;

                    this._currentStr = this._buttonElement.value;

                    this._btnType = HTMLTestButtonType.Custom;
                }
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build test button: " + ex.Message);
            }

            try
            {
                this._customMethodName = GetCustomMethodName();
            }
            catch
            {
                this._customMethodName = "";
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

                if (this._sendMsgOnly)
                {
                    this._sourceElement.click();
                }
                else
                {
                    MouseOp.Click();
                }

                if (_isDelayAfterAction)
                {
                    System.Threading.Thread.Sleep(_delayTime * 1000);
                }

                //action is finished, signal.
                _actionFinished.Set();

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform click action: " + ex.Message);
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

                if (_isDelayAfterAction)
                {
                    System.Threading.Thread.Sleep(_delayTime * 1000);
                }

                _actionFinished.Set();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform double click action: " + ex.Message);
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

                if (_isDelayAfterAction)
                {
                    System.Threading.Thread.Sleep(_delayTime * 1000);
                }

                _actionFinished.Set();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform right click action: " + ex.Message);
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
            //try
            //{
            //    if (this._inputElement != null)
            //    {

            //    }
            //}
        }

        public virtual string GetAction()
        {
            return "Click";
        }

        public virtual void DoAction(object para)
        {
            Click();
        }

        #endregion

        #region IStatus Members

        /* object GetCurrentStatus()
         * get the readystate of input element. 
         */
        public virtual object GetCurrentStatus()
        {
            try
            {
                if (_inputElement != null)
                {
                    return _inputElement.readyState;
                }
                else
                {
                    throw new CannotPerformActionException("Can not get status: Element can not be null.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not get status: " + ex.Message);
            }
        }

        /* bool IsReady()
         * return true if the object is ready.
         */
        public virtual bool IsReady()
        {
            try
            {
                if (_inputElement != null)
                {
                    return _inputElement.readyState == null ||
                        _inputElement.readyState == "interactive" ||
                        _inputElement.readyState == "complete";
                }
                else if (_buttonElement != null)
                {
                    return true;
                }
                else
                {
                    throw new CannotPerformActionException("Can not get ready status: InputElement can not be null.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not get ready status: " + ex.Message);
            }
        }

        #endregion

        #region IShowInfo methods

        public virtual string GetText()
        {
            return this._currentStr;
        }

        public virtual string GetFontFamily()
        {
            throw new PropertyNotFoundException();
        }
        
        public string GetFontSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontColor()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods

        /* HTMLTestButtonType GetButtonType()
         * return the button type by checking <input type="">.
         */
        protected virtual HTMLTestButtonType GetButtonType()
        {
            if (this._sourceElement.getAttribute("type", 0) != null && this._sourceElement.getAttribute("type", 0).GetType().ToString() != "System.DBNull")
            {
                string type = this._sourceElement.getAttribute("type", 0).ToString().Trim();

                if (String.Compare(type, "SUBMIT", true) == 0)
                {
                    return HTMLTestButtonType.Submit;
                }
                else if (String.Compare(type, "RESET", true) == 0)
                {
                    return HTMLTestButtonType.Reset;
                }
            }

            return HTMLTestButtonType.Custom;
        }


        /* string GetCustomMethodName()
         * return the method name by checking "onclick" property.
         */
        protected virtual string GetCustomMethodName()
        {
            string methodName="";
            if (HTMLTestObject.TryGetProperty(this._sourceElement, "onclick", out methodName))
            {
                return methodName;
            }
            else
            {
                return "";
            }
        }

        public override void Hover()
        {
            if (!IsReady() || !_isEnable || !_isVisible)
            {
                throw new CannotPerformActionException("Button is not enabled.");
            }

            if (!_sendMsgOnly)
            {
                base.Hover();
            }

        }
        #endregion

        #endregion

    }
}
