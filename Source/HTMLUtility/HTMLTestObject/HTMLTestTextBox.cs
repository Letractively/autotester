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
*          2008/01/12 wan,yu update, add HTMLTestTextBoxType
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using mshtml;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Core;


namespace Shrinerain.AutoTester.HTMLUtility
{
    //the type for textbox. <input type="text">,<input type="password">,<textarea>
    public enum HTMLTestTextBoxType : int
    {
        SingleLine = 0,
        MutilLine = 1,
        Password = 2,
        Unknow = 3
    }

    public class HTMLTestTextBox : HTMLGuiTestObject, IInputable
    {

        #region fields

        //the text in the textbox currently.
        protected string _currentStr;

        //we may have 2 types of textbox, one is <input type="text"> and the other maybe "<textarea>"
        protected IHTMLInputTextElement _textInputElement;
        protected IHTMLTextAreaElement _textAreaElement;

        protected HTMLTestTextBoxType _textBoxType = HTMLTestTextBoxType.Unknow;

        protected static Regex _specialKeysReg = new Regex(@"{[a-zA-Z]+}", RegexOptions.Compiled);

        #endregion

        #region properties

        public HTMLTestTextBoxType HTMLTextBoxType
        {
            get { return _textBoxType; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestTextBox(IHTMLElement element)
            : base(element)
        {

            this._type = HTMLTestObjectType.TextBox;

            try
            {
                if (this.Tag == "TEXTAERA")
                {
                    _textAreaElement = (IHTMLTextAreaElement)element;

                    _currentStr = _textAreaElement.value;

                    this._textBoxType = HTMLTestTextBoxType.MutilLine;
                }
                else
                {
                    _textInputElement = (IHTMLInputTextElement)element;

                    _currentStr = _textInputElement.value;

                    this._textBoxType = GetTextBoxType();
                }
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not build text box: " + e.Message);
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
                throw new CannotPerformActionException("Can not perform Input action");
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
                    throw new CannotPerformActionException("Can not input keys: " + keys + " to textbox :" + e.Message);
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

                if (this._tag == "INPUT")
                {
                    this._textInputElement.value = "";
                }
                else
                {
                    this._textAreaElement.value = "";
                }

                _actionFinished.Set();
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not perform clear action: " + e.Message);
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
                throw new CannotPerformActionException("Can not perform focus action.");
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

        /* HTMLTestTextBoxType GetTextBoxType()
         * return the text box type by checking the "type" property.
         */
        protected virtual HTMLTestTextBoxType GetTextBoxType()
        {
            if (this._sourceElement.getAttribute("type", 0) != null && this._sourceElement.getAttribute("type", 0).GetType().ToString() == "System.DBNull")
            {
                string type = this._sourceElement.getAttribute("type", 0).ToString().Trim();

                if (String.Compare(type, "PASSWORD", 0) == 0)
                {
                    return HTMLTestTextBoxType.Password;
                }
            }

            return HTMLTestTextBoxType.SingleLine;
        }


        /* string[] ExtractSpecialKeys(string text)
         * Return array of special keys.
         * NEED UPDATE!!!
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
