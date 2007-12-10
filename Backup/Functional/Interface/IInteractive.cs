/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IInteractive.cs
*
* Description: This interface defines the action provide by interactive object.
*              It means user can see this object, and can perform some actions
*              on it. 
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
        //make the object get focus
        void Focus();

        //get the default action, eg: "click" is the default action of a button
        object GetDefaultAction();

        //for a button, the default action is "click"
        void PerformDefaultAction(object parameter);
    }
}
