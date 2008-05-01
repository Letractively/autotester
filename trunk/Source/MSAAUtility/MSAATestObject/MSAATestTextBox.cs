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
    public class MSAATestTextBox : MSAATestGUIObject, IInputable, IWindows
    {

        #region fields

        protected IntPtr _handle;
        protected String _class;
        protected String _caption;

        #endregion

        #region properties


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
            this._handle = GetWindowsHandle(iAcc, childID);
        }

        #endregion

        #region public methods

        #region IInputable Members

        public void Input(string values)
        {
            try
            {
                KeyboardOp.SendChars(this._handle, values);
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
            throw new NotImplementedException();
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
            return MSAATestObject.GetValue(this._iAcc, _selfID);
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

        #region IWindows Members

        public IntPtr GetHandle()
        {
            return MSAATestObject.GetWindowsHandle(this._iAcc, this._selfID);
        }

        public string GetClass()
        {
            throw new NotImplementedException();
        }

        public String GetCaption()
        {
            try
            {
                StringBuilder sb = new StringBuilder(128);
                Win32API.GetWindowText(_handle, sb, 128);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get windows caption: " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
