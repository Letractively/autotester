/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: IDragable.cs
*
* Description: This interface defines actions provide by drag object.
*              eg: you can drag one item from one listbox to another.
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System.Drawing;

namespace Shrinerain.AutoTester.Core
{
    public interface IDragable : IInteractive
    {
        //drag the object to a point
        void Drag(Point start, Point end);
    }
}
