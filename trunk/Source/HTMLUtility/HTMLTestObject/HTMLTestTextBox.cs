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
* History: 2007/09/04 wan,yu Init version.
*          2008/01/12 wan,yu update, add HTMLTestTextBoxType.
*          2008/01/14 wan,yu bug fix, sometimes Input may input incorrect string.
*          2008/01/14 wan,yu update, add ClickAbove() method.          
*
*********************************************************************/

using System;
using System.Text.RegularExpressions;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;


namespace Shrinerain.AutoTester.HTMLUtility
{
    //the type for textbox. <input type="text">,<input type="password">,<textarea>
    public enum HTMLTestTextBoxType : int
    {
        SingleLine = 0,
        MultiLine = 1,
        Password = 2,
        Unknow = 3
    }

    public class HTMLTestTextBox : HTMLTestGUIObject, IInputable
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

                    this._textBoxType = HTMLTestTextBoxType.MultiLine;
                }
                else
                {
                    _textInputElement = (IHTMLInputTextElement)element;

                    _currentStr = _textInputElement.value;

                    this._textBoxType = GetTextBoxType();
                }


                //for some exception issue, I change the null to "".
                if (_currentStr == null)
                {
                    _currentStr = "";
                }
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build text box: " + ex.Message);
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

                string lastStr = GetText();

                int tryTimes = 0;

                //we need to make sure the text is inputted correctly.
                while (true)
                {
                    Focus();

                    //if we tried more than 1 times, we need also input the origin string.
                    if (tryTimes > 0)
                    {
                        KeyboardOp.SendChars(lastStr + value);
                    }
                    else
                    {
                        KeyboardOp.SendChars(value);
                    }

                    if (String.Compare(GetText(), lastStr + value, 0) == 0)
                    {
                        break;
                    }
                    else
                    {
                        // incorrect, retry.
                        tryTimes++;

                        //if we already tried 3 times, break.
                        if (tryTimes > 3)
                        {
                            throw new CannotPerformActionException("Can not input string: " + value);
                        }

                        //clear old text
                        if (this._tag == "INPUT")
                        {
                            this._textInputElement.value = "";
                        }
                        else
                        {
                            this._textAreaElement.value = "";
                        }


                    }
                }

                System.Threading.Thread.Sleep(200 * 1);

                //on some website like google.com, when you are typing something in the textbox, here is a dropdown list to
                //let you to choose, this dropdown list may cover some other controls, eg: it may cover the "Google Search" button
                //and we can not click this button, so we need to elimate it. 
                //click just above the text box, to elimate it.
                ClickAbove();

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
                catch (Exception ex)
                {
                    throw new CannotPerformActionException("Can not input keys: " + keys + " to textbox :" + ex.Message);
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
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform clear action: " + ex.Message);
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
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform focus action: " + ex.Message);
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
            try
            {
                if (this._tag == "INPUT")
                {
                    return this._textInputElement.value == null ? "" : this._textInputElement.value;
                }
                else
                {
                    return this._textAreaElement.value == null ? "" : this._textAreaElement.value;
                }
            }
            catch
            {
                return "";
            }
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

        /* protected virtual void ClickAbove()
         * Click at the top of the object.
         * Sometimes we will meet auto fill like Google.com, if you input some words, there may display
         * a dropdown list to let you to select, this dropdown list may cover some objects we need, so after
         * input some string, we need click above the text box, to make the dropdown list disappear.
         */
        protected virtual void ClickAbove()
        {
            try
            {
                //move just above the text box.
                MouseOp.MoveShift(0, -(this._rect.Height / 2 + 1));
                MouseOp.Click();
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click above the text box: " + ex.Message);
            }
        }

        /* string GetAroundText()
         * return the text around textbox.
         */
        protected override string GetAroundText()
        {
            return HTMLTestObjectPool.GetLabelForTextBox(this._sourceElement);
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
