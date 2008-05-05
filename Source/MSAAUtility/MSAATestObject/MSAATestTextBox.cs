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
                throw new CannotBuildObjectException("Can not build Text Box: " + ex.Message);
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
                throw new CannotBuildObjectException("Can not build Text Box: " + ex.Message);
            }
        }

        #endregion

        #region public methods

        #region IInputable Members

        public void Input(string values)
        {
            try
            {
                _actionFinished.WaitOne();

                if (!_sendMsgOnly)
                {
                    Hover();
                    MouseOp.Click();
                }

                KeyboardOp.SendChars(WindowHandle, values);
                _text = values;

                _actionFinished.Set();
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not input string: " + ex.Message);
            }
        }

        public void InputKeys(string keys)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            try
            {
                _actionFinished.WaitOne();

                if (!SetProperty("value", ""))
                {
                    throw new CannotPerformActionException("Can not clear text in text box");
                }
                else
                {
                    _text = "";
                }

                _actionFinished.Set();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not clear text in text box: " + ex.Message);
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

        #region IShowInfo Members

        public string GetText()
        {
            return GetValue();
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


        #endregion

        #endregion

    }
}
