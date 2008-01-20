/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestAction.cs
*
* Description: This class implement ITestAction, support the actions 
*              on HTML page. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Text.RegularExpressions;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestAction : ITestAction
    {

        #region fields

        //we convert an object to an interface, use this interface to perform actual action
        private IClickable _clickObj;
        private IInputable _inputObj;
        private ISelectable _selectObj;
        private ICheckable _checkObj;

        #endregion

        #region ITestAction Members

        #region Click action
        public void Click(object obj)
        {
            _clickObj = (IClickable)obj;
            _clickObj.Click();
        }

        public void ClickPoint(int x, int y)
        {
            MouseOp.Click(x, y);
        }

        public void DoubleClick(object obj)
        {
            _clickObj = (IClickable)obj;
            _clickObj.DoubleClick();
        }

        public void DoubleClickPoint(int x, int y)
        {
            MouseOp.DoubleClick(x, y);
        }

        public void NClick(object obj, int n)
        {
            _clickObj = (IClickable)obj;
            for (int i = 0; i < n; i++)
            {
                _clickObj.Click();
            }
        }

        public void NClickPoint(int x, int y)
        {
            MouseOp.NClick(x, y, 1);
        }

        public void RightClick(object obj)
        {
            _clickObj = (IClickable)obj;
            _clickObj.RightClick();
        }

        public void RightClickPoint(int x, int y)
        {
            MouseOp.RightClick(x, y);
        }

        public void MiddleClick(object obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void MiddleClickPoint(int x, int y)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region checkbox action

        public void Check(object obj)
        {
            _checkObj = (ICheckable)obj;
            _checkObj.Check();
        }

        public void UnCheck(object obj)
        {
            _checkObj = (ICheckable)obj;
            _checkObj.UnCheck();
        }

        #endregion

        #region textbox action
        public void Input(object obj, string chars)
        {
            _inputObj = (IInputable)obj;
            _inputObj.Input(chars);
        }

        public void InputKeys(object obj, string keys)
        {
            _inputObj = (IInputable)obj;
            _inputObj.InputKeys(keys);
        }

        public void Clear(object obj)
        {
            _inputObj = (IInputable)obj;
            _inputObj.Clear();
        }

        #endregion

        #region list action
        public void Select(object obj, string value)
        {
            _selectObj = (ISelectable)obj;
            _selectObj.Select(value);
        }

        public void SelectIndex(object obj, int index)
        {
            _selectObj = (ISelectable)obj;
            _selectObj.SelectByIndex(index);
        }

        public void SelectRegExp(object obj, string regExp)
        {
            _selectObj = (ISelectable)obj;
            string[] values = _selectObj.GetAllValues();

            Regex reg;
            try
            {
                reg = new Regex(regExp);
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Invalid regular expression: " + ex.Message);
            }

            foreach (string v in values)
            {
                if (reg.IsMatch(v))
                {
                    _selectObj.Select(v);
                    return;
                }
            }

            throw new CannotPerformActionException("Can not find item by regular expression: " + regExp);

        }

        #endregion

        #region tree action
        public void ClickPath(object obj, string path)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void ClickIndex(object obj, int deep, int index)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void ClickNode(object obj, string node)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void DoubleClickPath(object obj, string path)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void DoubleClickIndex(object obj, int deep, int index)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void DoubleClickNode(object obj, string node)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void RightClickPath(object obj, string path)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void RightClickIndex(object obj, int deep, int index)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void RightClickNode(object obj, string node)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        #endregion

        #region table action
        public void ClickCell(object obj, int row, int col)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void ClickCellText(object obj, string value)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void ClickCellRegex(object obj, string regEx)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void DoubleClickCell(object obj, int row, int col)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void DoubleClickCellText(object obj, string value)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void DoubleClickCellRegex(object obj, string regEx)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void RightClickCell(object obj, int row, int col)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void RightClickCellText(object obj, string value)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        public void RightClickCellRegex(object obj, string regEx)
        {
            throw new CannotPerformActionException("The method or operation is not implemented.");
        }

        #endregion

        #region other action

        public void Hover(object obj)
        {
            _clickObj = (IClickable)obj;
            _clickObj.Hover();
        }

        public void Drag(object obj, int endX, int endY)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SelectText(object obj, string text)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ClearText(object obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void MouseRoll(object obj, int steps)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion
    }

}
