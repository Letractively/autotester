/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IInputable.cs
*
* Description: This interface defines the actions of an object which 
*              you can input something to it.Like Button, Listbox.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IInputable : IVisible, IInteractive, IShowInfo
    {
        void Input(string values);
        void InputKeys(string keys);
        void Clear(); //clear text.
    }
}