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
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestButton : MSAATestGUIObject, IClickable, IShowInfo
    {

        #region fields

        protected string _text;

        #endregion

        #region properties


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
            this._text = GetText();
        }

        #endregion

        #region public methods

        #region IClickable Members

        public void Click()
        {
            try
            {
                _actionFinished.WaitOne();

                if (!_sendMsgOnly)
                {
                    Hover();

                    MouseOp.Click();
                }
                else
                {
                    _iAcc.accDoDefaultAction(_selfID);
                }

                _actionFinished.Set();

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
            return "Click";
        }

        public void DoAction(object parameter)
        {
            Click();
        }

        #endregion

        #region IShowInfo Members

        public string GetText()
        {
            return MSAATestObject.GetName(this._iAcc, Convert.ToInt32(this._selfID));
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


        #endregion

        #endregion

    }
}
