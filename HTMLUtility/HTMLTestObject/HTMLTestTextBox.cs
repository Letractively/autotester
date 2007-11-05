using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using mshtml;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;


namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestTextBox : HTMLGuiTestObject, IInputable, IVisible
    {

        #region fields

        protected string _currentStr;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestTextBox(IHTMLElement element)
            : base(element)
        {
            try
            {
                if (this.Tag == "TEXTAERA")
                {
                    _currentStr = element.getAttribute("innerText", 0).ToString();
                }
                else
                {
                    _currentStr = element.getAttribute("value", 0).ToString();
                }
            }
            catch
            {
                _currentStr = "";
            }
        }

        #endregion

        #region public methods

        public virtual void Input(string value)
        {
            if (value == null)
            {
                value = "";
            }
            try
            {
                _actionFinished.WaitOne();

                Focus();
                KeyboardOp.SendChars(value);

                _actionFinished.Set();

            }
            catch
            {
                throw new CanNotPerformActionException("Can not perform Input action");
            }
        }

        public virtual void InputKeys(string keys)
        {

        }

        public virtual void Clear()
        {
            try
            {
                _actionFinished.WaitOne();

                this._sourceElement.setAttribute("value", "", 0);

                _actionFinished.Set();
            }
            catch
            {
                throw new CanNotPerformActionException("Can not perform clear action.");
            }
        }

        public virtual void Focus()
        {
            try
            {
                Hover();
                MouseOp.Click();
            }
            catch
            {
                throw new CanNotPerformActionException("Can not perform focus action.");
            }
        }

        public virtual object GetDefaultAction()
        {
            return "Input";
        }

        public virtual void PerformDefaultAction()
        {
            Input("");
        }

        public virtual string GetText()
        {
            return this._currentStr;
        }

        public virtual string GetFontStyle()
        {
            throw new PropertyNotFoundException("Can not get Font style.");
        }

        public virtual string GetFontFamily()
        {
            throw new PropertyNotFoundException("Can not get Font family");
        }

        #endregion

        #region private methods

        //protected void 

        #endregion

        #endregion
    }
}
