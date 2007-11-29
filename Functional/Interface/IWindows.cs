/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IWindow.cs
*
* Description: This interface define the methods of standard windows control.
*              You can find a standard windows control by it's class name.
*              And once you get the Handle, you can perform any actions on it.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IWindows
    {
        IntPtr GetHandle();
        string GetClass();
    }
}
