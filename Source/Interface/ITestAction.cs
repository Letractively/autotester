/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestAction.cs
*
* Description: This interface define the actions provide by AutoTester.
*              Once the automation tester get an object from object 
*              pool, he can perform some actions on the object. 
*
* History:  2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;

namespace Shrinerain.AutoTester.Interface
{
    [CLSCompliant(true)]
    public interface ITestAction
    {
        //Click action
        void Click(Object testObj);
        void ClickPoint(int x, int y);
        void DoubleClick(Object testObj);
        void DoubleClickPoint(int x, int y);
        void NClick(Object testObj, int n);
        void NClickPoint(int x, int y);
        void RightClick(Object testObj);
        void RightClickPoint(int x, int y);
        void MiddleClick(Object testObj);
        void MiddleClickPoint(int x, int y);

        //Check actions
        void Check(Object testObj);
        void UnCheck(Object testObj);

        //Input actions
        void Input(Object testObj, String chars);
        void InputKeys(Object testObj, String keys);
        void Clear(Object testObj);

        //Select actions
        void Select(Object testObj, String text);
        void SelectIndex(Object testObj, int index);
        void SelectRegExp(Object testObj, String regExp);

        //Tree actions
        void ClickPath(Object testObj, String path);
        void ClickIndex(Object testObj, int deep, int index);
        void ClickNode(Object testObj, String node);
        void DoubleClickPath(Object testObj, String path);
        void DoubleClickIndex(Object testObj, int deep, int index);
        void DoubleClickNode(Object testObj, String node);
        void RightClickPath(Object testObj, String path);
        void RightClickIndex(Object testObj, int deep, int index);
        void RightClickNode(Object testObj, String node);


        //Table actions
        void ClickCell(Object testObj, int row, int col);
        void ClickCellText(Object testObj, String value);
        void ClickCellRegex(Object testObj, String regEx);
        void DoubleClickCell(Object testObj, int row, int col);
        void DoubleClickCellText(Object testObj, String value);
        void DoubleClickCellRegex(Object testObj, String regEx);
        void RightClickCell(Object testObj, int row, int col);
        void RightClickCellText(Object testObj, String value);
        void RightClickCellRegex(Object testObj, String regEx);

        //Other actions
        void Hover(Object testObj);
        void Drag(Object testObj, int endX, int endY);
        void SelectText(Object testObj, String text);
        void MouseRoll(Object testObj, int steps);

    }
}
