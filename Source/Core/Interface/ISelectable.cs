/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ISelectable.cs
*
* Description: This interface defines the actoins of a selectable object.
*              Like ListBox, ComboBox etc.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public interface ISelectable : IInteractive, IShowInfo
    {
        //select the item by it's text
        void Select(string str);

        //select the item by it's index, eg: the 1st one.
        void SelectByIndex(int index);

        //get all values of a select object.
        String[] GetAllValues();
    }
}
