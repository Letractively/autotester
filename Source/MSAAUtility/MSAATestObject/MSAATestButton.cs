/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MSAATestButton.cs
*
* Description: This class define the button object for MSAA.
*
* History: 2008/04/11 wan,yu init version.
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
    public class MSAATestButton : MSAATestGUIObject, IClickable, IText
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
        }
        #endregion

        #region methods

        #region ctor

        public MSAATestButton(IAccessible iAcc)
            : this(iAcc, 0)
        {
        }

        public MSAATestButton(IAccessible iAcc, int childID)
            : base(iAcc, childID)
        {
            try
            {
                _text = GetText();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build Button: " + ex.ToString());
            }
        }

        public MSAATestButton(IntPtr handle)
            : base(handle)
        {
            try
            {
                _text = GetText();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build Button: " + ex.ToString());
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
                    IntPtr handle = GetHandle();
                    if (handle == IntPtr.Zero)
                    {
                        IAcc.accDoDefaultAction(ChildID);
                    }
                    else
                    {
                        MouseOp.Click(handle);
                    }
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click button: " + ex.ToString());
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

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetAction()
        {
            return "Click";
        }

        public void DoAction(object parameter)
        {
            Click();
        }

        #endregion

        #region IText Members

        public string GetText()
        {
            return GetName();
        }

        public override string GetLabel()
        {
            return GetName();
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

        #endregion

        #region private methods

        protected override void GetMSAAInfo()
        {
            this._type = Type.Button;
            base.GetMSAAInfo();
        }

        #endregion

        #endregion

    }
}
