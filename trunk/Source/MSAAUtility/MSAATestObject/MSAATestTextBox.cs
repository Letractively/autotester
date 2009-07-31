/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MSAATestTextBox.cs
*
* Description: This class defines actions for text box.
*
* History: 2008/05/01 wan,yu Init version.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestTextBox : MSAATestGUIObject, IInputable
    {
        #region fields

        protected String _text;

        #endregion

        #region properties

        public virtual String Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != null)
                {
                    this.Input(value);
                }
            }
        }

        #endregion

        #region methods

        #region ctor

        public MSAATestTextBox(IAccessible iAcc)
            : this(iAcc, 0)
        {
        }

        public MSAATestTextBox(IAccessible iAcc, int childID)
            : base(iAcc, childID)
        {
            try
            {
                _text = GetText();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build Text Box: " + ex.ToString());
            }
        }

        public MSAATestTextBox(IntPtr handle)
            : base(handle)
        {
            try
            {
                _text = GetText();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build Text Box: " + ex.ToString());
            }
        }

        #endregion

        #region public methods

        #region IInputable Members

        public void Input(string values)
        {
            try
            {
                if (!_sendMsgOnly)
                {
                    _actionFinished.WaitOne();
                    Hover();
                    MouseOp.Click();
                    KeyboardOp.SendChars(values);
                    _actionFinished.Set();
                }
                else
                {
                    IntPtr handle = GetHandle();
                    if (handle == IntPtr.Zero)
                    {
                        SetProperty("value", values.ToString());
                    }
                    else
                    {
                        KeyboardOp.SendChars(handle, values);
                    }
                }

                _text = values;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not input string: " + ex.ToString());
            }
        }

        public void Clear()
        {
            try
            {
                if (!_sendMsgOnly)
                {
                    Hover();
                    MouseOp.Click();

                    int length = this._text.Length;
                    if (length == 0)
                    {
                        length = 32;
                    }
                    int i = 0;
                    while (i < length)
                    {
                        KeyboardOp.PressBackspace();
                        i++;
                    }
                }
                else
                {
                    SetProperty("value", "");
                }
                _text = "";
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not clear text in text box: " + ex.ToString());
            }
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new NotImplementedException();
        }

        public string GetAction()
        {
            return "Input";
        }

        public void DoAction(object parameter)
        {
            if (parameter != null)
            {
                Input(parameter.ToString());
            }
        }

        #endregion

        #region IText Members

        public string GetText()
        {
            return GetValue();
        }

        public override string GetLabel()
        {
            return GetText();
        }

        public string GetFontFamily()
        {
            throw new NotImplementedException();
        }

        public string GetFontSize()
        {
            throw new NotImplementedException();
        }

        public string GetFontColor()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private methods
        protected override void GetMSAAInfo()
        {
            this._type = MSAATestObjectType.TextBox;
            base.GetMSAAInfo();

            if (this._state.IndexOf("Protected") >= 0)
            {
                this._sendMsgOnly = false;
            }
        }

        #endregion

        #endregion
    }
}
