/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestTextBox.cs
*
* Description: This class defines the actions provide by TextBox.
*              The important actions include "Input" and "InputKeys". 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

        //the text in the textbox currently.
        protected string _currentStr;

        //we may have 2 types of textbox, one is <input type="text"> and the other maybe "<textarea>"
        protected IHTMLInputTextElement _textElement;
        protected IHTMLTextAreaElement _textAreaElement;

        protected static Regex _specialKeysReg = new Regex(@"{[a-zA-Z]+}", RegexOptions.Compiled);

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

                    _textAreaElement = (IHTMLTextAreaElement)element;
                }
                else
                {
                    _currentStr = element.getAttribute("value", 0).ToString();

                    _textElement = (IHTMLInputTextElement)element;
                }
            }
            catch
            {
                _currentStr = "";
            }
        }

        #endregion

        #region public methods

        /* void Input(string value)
         * Input normal characters to textbox.
         */
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

        /* void InputKeys(string keys)
         * Input special keys , eg: {tab}
         */
        public virtual void InputKeys(string keys)
        {
            if (!String.IsNullOrEmpty(keys))
            {
                try
                {
                    Focus();
                    KeyboardOp.SendKey(keys);
                }
                catch (Exception e)
                {
                    throw new CanNotPerformActionException("Can not input keys: " + keys + " to textbox :" + e.Message);
                }
            }
        }

        /* void Clear()
         * Clear text in the textbox. like we press "Backspace".
         */
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

        #region IInteractive interface
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

        public virtual void PerformDefaultAction(object para)
        {
            Input(para.ToString());
        }

        #endregion

        #region IShowInfo interface
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

        #endregion

        #region private methods

        /* string[] ExtractSpecialKeys(string text)
         * Return array of special keys.
         */
        protected virtual string[] ExtractSpecialKeys(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                MatchCollection mc = _specialKeysReg.Matches(text);

                if (mc.Count > 0)
                {
                    string[] specialKeys = new string[mc.Count];

                    string currentKey;
                    for (int i = 0; i < mc.Count; i++)
                    {
                        currentKey = mc[i].Value.ToUpper();
                        if (currentKey == "{TAB}" ||
                            currentKey == "{CTRL}" ||
                            currentKey == "{ALT}" ||
                            currentKey == "{SHIFT}")
                        {
                            specialKeys[i] = currentKey;
                        }

                    }

                    return specialKeys;

                }
            }

            return null;
        }

        /* string[] ExtractNormalCharacters(string text)
         * return array of normal characters.
         */
        protected virtual string[] ExtractNormalCharacters(string text)
        {


            return null;
        }

        #endregion

        #endregion
    }
}
