/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IClickable.cs
*
* Description: This interface define the actions of CheckBox, RadioButton
*              and other object.
*
*
* History:  2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Function.Interface
{
    public interface ICheckable : IClickable
    {
        void Check();
        void UnCheck();
        void IsChecked();
    }
}
