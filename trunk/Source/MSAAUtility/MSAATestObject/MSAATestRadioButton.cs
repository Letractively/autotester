/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MSAATestRadioButton.cs
*
* Description: This class define the radio button object for MSAA.
*
* History: 2008/05/02 wan,yu init version.
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
    public class MSAATestRadioButton : MSAATestGUIObject, ICheckable, IText
    {

        #region fields

        protected String _text;
        protected bool _isChecked;

        #endregion

        #region properties

        public virtual bool Checked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (value)
                {
                    this.Check();
                }
                else
                {
                    this.UnCheck();
                }
            }
        }

        public virtual String Text
        {
            get
            {
                return _text;
            }
        }

        #endregion

        #region methods

        #region ctor

        public MSAATestRadioButton(IAccessible iAcc)
            : this(iAcc, 0)
        {
        }

        public MSAATestRadioButton(IAccessible iAcc, int childID)
            : base(iAcc, childID)
        {
            try
            {
                _text = GetText();
                _isChecked = IsChecked();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build Radio Button: " + ex.Message);
            }
        }

        public MSAATestRadioButton(IntPtr handle)
            : base(handle)
        {
            try
            {
                _text = GetText();
                _isChecked = IsChecked();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build Radio Button: " + ex.Message);
            }
        }

        #endregion

        #region public methods

        #region IClickable Members

        public void Click()
        {
            try
            {
                if (!_sendMsgOnly)
                {
                    Hover();
                    MouseOp.Click();
                }
                else
                {
                    IAcc.accDoDefaultAction(ChildID);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click button: " + ex.Message);
            }
        }

        public void DoubleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RightClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void MiddleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetAction()
        {
            return "Check";
        }

        public void DoAction(object parameter)
        {
            try
            {
                bool isCheck = true;

                if (parameter != null)
                {
                    isCheck = Convert.ToBoolean(parameter);
                }

                if (isCheck)
                {
                    Check();
                }
                else
                {
                    UnCheck();
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform action: " + ex.Message);
            }
        }

        #endregion

        #region IText Members

        public string GetText()
        {
            return GetName();
        }

        public override string GetLabel()
        {
            return GetText();
        }

        public string GetFontFamily()
        {
            throw new Exception("The method or operation is not implemented.");
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

        #region ICheckable Members

        public void Check()
        {
            try
            {
                if (!IsChecked())
                {
                    Click();
                }

                _isChecked = true;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not check checkbox: " + ex.Message);
            }

        }

        public void UnCheck()
        {
            try
            {
                if (IsChecked())
                {
                    Click();
                }

                _isChecked = false;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not uncheck checkbox: " + ex.Message);
            }
        }

        public bool IsChecked()
        {
            try
            {
                String state = GetState();
                return state.IndexOf("checked", StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }

        }

        #endregion

        #endregion

        #region private methods

        protected override void GetMSAAInfo()
        {
            this._type = Type.RadioBox;
            base.GetMSAAInfo();
        }

        #endregion

        #endregion
    }
}
