/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IInteractive.cs
*
* Description: This interface defines the actions of an IInteractive 
*              object.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface IInteractive : IVisible
    {
        void Focus();
        object GetDefaultAction();
        void PerformDefaultAction();
    }
}
