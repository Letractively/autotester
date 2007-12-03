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
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestAction
    {
        //Click action
        void Click(Object obj);
        void ClickPoint(int x, int y);
        void DoubleClick(Object obj);
        void DoubleClickPoint(int x, int y);
        void NClick(Object obj, int n);
        void NClickPoint(int x, int y);
        void RightClick(Object obj);
        void RightClickPoint(int x, int y);
        void MiddleClick(Object obj);
        void MiddleClickPoint(int x, int y);

        //Check actions
        void Check(Object obj);
        void UnCheck(Object obj);

        //Input actions
        void Input(Object obj, String chars);
        void InputKeys(Object obj, String keys);
        void Clear(Object obj);

        //Select actions
        void Select(Object obj, String value);
        void SelectIndex(Object obj, int index);
        void SelectRegExp(Object obj, String regExp);

        //Tree actions
        void ClickPath(Object obj, String path);
        void ClickIndex(Object obj, int deep, int index);
        void ClickNode(Object obj, String node);
        void DoubleClickPath(Object obj, String path);
        void DoubleClickIndex(Object obj, int deep, int index);
        void DoubleClickNode(Object obj, String node);
        void RightClickPath(Object obj, String path);
        void RightClickIndex(Object obj, int deep, int index);
        void RightClickNode(Object obj, String node);


        //Table actions
        void ClickCell(Object obj, int row, int col);
        void ClickCellText(Object obj, String value);
        void ClickCellRegex(Object obj, String regEx);
        void DoubleClickCell(Object obj, int row, int col);
        void DoubleClickCellText(Object obj, String value);
        void DoubleClickCellRegex(Object obj, String regEx);
        void RightClickCell(Object obj, int row, int col);
        void RightClickCellText(Object obj, String value);
        void RightClickCellRegex(Object obj, String regEx);

        //Other actions
        void Hover(Object obj);
        void Drag(Object obj, int endX, int endY);
        void SelectText(Object obj, String text);
        void MouseRoll(Object obj, int steps);

    }
}
