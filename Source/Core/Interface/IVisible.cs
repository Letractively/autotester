/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IVisible.cs
*
* Description: This interface defines the actions of an object which
*              is visible to end-users.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Shrinerain.AutoTester.Core
{
    public interface IVisible
    {
        //get the center point of the object
        Point GetCenterPoint();

        //get the rectangle of the object on screen
        Rectangle GetRectOnScreen();

        //get the image of the object
        Bitmap GetControlPrint();

        //move mouse to the top of control
        void Hover();
    }
}
