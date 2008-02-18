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
*          2008/01/24 wan,yu update, add GetLabelForTextBox(). 
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

            this._isDelayAfterAction = false;

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

                Focus();

                try
                {
                    //or send the chars by keyboard
                    KeyboardOp.SendChars(value);

                    //on some website like google.com, when you are typing something in the textbox, here is a dropdown list to
                    //let you to choose, this dropdown list may cover some other controls, eg: it may cover the "Google Search" button
                    //and we can not click this button, so we need to elimate it. 
                    //click just above the text box, to elimate it.
                    ClickAbove();

                }
                catch
                {
                    string curStr = this.GetText() + value;
                    //set the text directly.
                    if (this._tag == "INPUT")
                    {
                        this._textInputElement.value = curStr;
                    }
                    else
                    {
                        this._textAreaElement.value = curStr;
                    }
                }

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
                throw new CannotPerformActionException("Can not perform Input action: " + ex.Message);
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
                    _actionFinished.WaitOne();

                    Focus();
                    KeyboardOp.SendKey(keys);

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
                if (!_isEnable || !_isVisible || _isReadOnly)
                {
                    throw new CannotPerformActionException("Textbox is not enabled.");
                }

                _actionFinished.WaitOne();

                if (this._tag == "INPUT")
                {
                    this._textInputElement.value = "";
                }
                else
                {
                    this._textAreaElement.value = "";
                }

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
                throw new CannotPerformActionException("Can not perform clear action: " + ex.Message);
            }
        }

        #region IInteractive interface
        public virtual void Focus()
        {
            try
            {
                if (!_isEnable || !_isVisible || _isReadOnly)
                {
                    throw new CannotPerformActionException("Textbox is not enabled.");
                }

                Hover();
                MouseOp.Click();

                System.Threading.Thread.Sleep(500 * 1);
            }
            catch (TestException)
            {
                throw;
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

        /* string GetLabelForTextBox(IHTMLElement element)
         * return the text around textbox, firstly we will try current cell, then try left cell and up cell.
         */
        public static string GetLabelForTextBox(IHTMLElement element)
        {
            try
            {
                //firstly, try to get text in the same cell/span/div/label
                string label = GetAroundText(element);

                //for textbox, we think the text on the left is it's label.
                if (!String.IsNullOrEmpty(label) && !String.IsNullOrEmpty(label.Split(new string[] { _labelSplitter }, StringSplitOptions.RemoveEmptyEntries)[0]))
                {
                    return label.Split(new string[] { _labelSplitter }, StringSplitOptions.RemoveEmptyEntries)[0];
                }

                //if failed, try to get text from left cell or up cell.
                IHTMLElement cellParentElement = element.parentElement;

                if (cellParentElement != null && cellParentElement.tagName == "TD")
                {
                    IHTMLTableCell parentCellElement = (IHTMLTableCell)cellParentElement;
                    int cellId = parentCellElement.cellIndex;

                    IHTMLTableRow parentRowElement = (IHTMLTableRow)cellParentElement.parentElement;
                    int rowId = parentRowElement.rowIndex;

                    object index = (object)(cellId - 1);

                    //if the current cell is not the most left cell, we will try the left cell to find the around text.
                    if (cellId > 0)
                    {
                        //get the left cell.
                        IHTMLElement prevCellElement = (IHTMLElement)parentRowElement.cells.item(index, index);

                        if (HTMLTestObject.TryGetValueByProperty(prevCellElement, "innerText", out label))
                        {
                            return label;
                        }

                    }

                    //if the current cell is the first cell OR we didn't find text in the left cell, we will try the up cell.
                    //of coz, if we want to try up row, current row can NOT be the first row, that means the row id should >0
                    if (cellId == 0 && rowId > 0)
                    {
                        //                                                     cell              row           table
                        IHTMLTableSection tableSecElement = (IHTMLTableSection)cellParentElement.parentElement.parentElement;

                        index = (object)(rowId - 1);
                        IHTMLTableRow upRowElement = (IHTMLTableRow)tableSecElement.rows.item(index, index);

                        index = (object)cellId;
                        IHTMLElement upCellElement = (IHTMLElement)upRowElement.cells.item(index, index);

                        if (HTMLTestObject.TryGetValueByProperty(upCellElement, "innerText", out label))
                        {
                            return label;
                        }
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }

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
            catch
            {
                //throw new CannotPerformActionException("Can not click above the text box: " + ex.Message);
            }
        }

        /* string GetAroundText()
         * return the text around textbox.
         */
        protected override string GetLabelText()
        {
            return GetLabelForTextBox(this._sourceElement);
        }

        #endregion

        #endregion
    }
}
