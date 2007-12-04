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

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface ISelectable : IInteractive, IShowInfo
    {
        void Select(string str);
        void SelectByIndex(int index);
        String[] GetAllValues();
    }
}
