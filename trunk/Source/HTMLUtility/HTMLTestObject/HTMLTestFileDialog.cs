/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestFileDialog.cs
*
* Description: This class defines the actions provide by FileDialog,
*              we use FileDialog to Open/Save File/Folder.
*              The important action include "Input", means the file/folder
*              we want to open/save. 
*       
*
* History: 2007/12/11 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    //HTMLTestFileDialog is NOT a HTML control, it is a standard Windows control.
    public class HTMLTestFileDialog : HTMLGuiTestObject, IWindows, IInputable, IClickable
    {

        #region fields


        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestFileDialog()
            : base()
        {

        }

        #endregion

        #region public methods

        //public override Rectangle GetRectOnScreen()
        //{
        //    return null;
        //}

        #region IWindows Members

        public IntPtr GetHandle()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetClass()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInputable Members

        public void Input(string values)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void InputKeys(string keys)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetDefaultAction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void PerformDefaultAction(object parameter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IShowInfo Members

        public string GetText()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontStyle()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontFamily()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion



        #region IClickable Members

        public void Click()
        {
            throw new Exception("The method or operation is not implemented.");
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
    }
}
