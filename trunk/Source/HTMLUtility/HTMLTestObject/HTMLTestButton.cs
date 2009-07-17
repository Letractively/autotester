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
using System.Collections.Generic;
using System.Threading;

using Accessibility;
using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    //for HTML test button, we may have <input type="button">, <input type="submit">, <input type="reset">
    public enum HTMLTestButtonType : int
    {
        Normal = 0,
        Submit = 1,
        Reset = 2,
        File = 3,
        Unknow = 4
    }

    public class HTMLTestButton : HTMLTestGUIObject, IClickable, IText, IStatus
    {
        #region fields

        //the text on the button, like "Login"
        protected string _buttonCaption;

        //the HTML element of the object. we build HTMLTestButton based on the HTML element.
        //<input>
        protected IHTMLInputElement _inputElement;
        //<button>
        protected IHTMLButtonElement _buttonElement;

        protected HTMLTestButtonType _btnType = HTMLTestButtonType.Unknow;

        #endregion

        #region properties

        public HTMLTestButtonType ButtonType
        {
            get { return _btnType; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestButton(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestButton(IHTMLElement element, HTMLTestBrowser browser)
            : base(element, browser)
        {
            this._isDelayAfterAction = true;
            this._type = HTMLTestObjectType.Button;
            try
            {
                if (String.Compare(element.tagName, "INPUT", true) == 0)
                {
                    this._inputElement = (IHTMLInputElement)element;
                    this._btnType = GetButtonType();
                }
                else if (String.Compare(element.tagName, "BUTTON", true) == 0)
                {
                    this._buttonElement = (IHTMLButtonElement)element;
                    this._btnType = HTMLTestButtonType.Normal;
                }

                this._buttonCaption = GetCaption();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build test button: " + ex.ToString());
            }
        }

        #endregion

        #region public methods

        public override List<TestProperty> GetIdenProperties()
        {
            List<TestProperty> properties = base.GetIdenProperties();
            properties.Add(new TestProperty(TestConstants.PROPERTY_CAPTION, _buttonCaption));
            return properties;
        }

        /* void Click()
         * Click on the button
         */
        public virtual void Click()
        {
            try
            {
                BeforeAction();

                Thread t = new Thread(new ThreadStart(PerformClick));
                t.Start();
                t.Join(ActionTimeout);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform click action: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        /* void DoubleClick()
         * Double click on the button
         */
        public virtual void DoubleClick()
        {
            try
            {
                BeforeAction();

                Hover();
                MouseOp.DoubleClick();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform double click action: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        /* void RightClick()
         * right click on the button
         */
        public virtual void RightClick()
        {
            try
            {
                BeforeAction();

                Hover();
                MouseOp.RightClick();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform right click action: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        /* void MiddleClick()
         * middle click on the button
         */
        public virtual void MiddleClick()
        {
            throw new CannotPerformActionException("Can not perform middle click.");
        }

        #region IInteractive methods

        public virtual string GetAction()
        {
            return "Click";
        }

        public virtual void DoAction(object para)
        {
            Click();
        }

        #endregion

        #region IText methods

        public virtual string GetText()
        {
            return this._buttonCaption;
        }

        public override string GetLabel()
        {
            return GetText();
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
                else if (String.Compare(type, "FILE", true) == 0)
                {
                    return HTMLTestButtonType.File;
                }
            }

            return HTMLTestButtonType.Normal;
        }

        protected virtual String GetCaption()
        {
            object caption = null;
            if (_btnType == HTMLTestButtonType.File)
            {
                try
                {
                    caption = (GetIAccInterface().accNavigate(5, 0) as IAccessible).get_accDescription(0);
                }
                catch
                {
                    caption = "Browse...";
                }
            }
            else
            {
                if (!TryGetProperty("value", out caption))
                {
                    TryGetProperty("title", out caption);
                }
            }

            return caption != null ? caption.ToString().Trim() : "";
        }

        #endregion

        #endregion
    }
}
