/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: Mouse.cs
*
* Description: This class defines the actions for mouse.
*              You can move the mouse to any position.
*              Left click and right click. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Shrinerain.AutoTester.Win32
{
    public sealed class MouseOp
    {

        #region ctor

        //not allow to create instance
        private MouseOp()
        {

        }

        #endregion

        #region public methods

        /*  void NClick(int x, int y, int repeatTimes)
         *  Left click at (x,y) for repeatTimes.
         */
        public static void NClick(int x, int y, int repeatTimes)
        {
            MoveTo(x, y);
            for (int i = 0; i < repeatTimes; i++)
            {
                Win32API.mouse_event((int)Win32API.MouseEventFlags.LeftDown, 0, 0, 0, IntPtr.Zero);
                Win32API.mouse_event((int)Win32API.MouseEventFlags.LeftUp, 0, 0, 0, IntPtr.Zero);
            }
        }

        /*  void NRightClick(int x, int y, int repeatTimes)
         *  right click at (x,y) for repeatTimes
         */
        public static void NRightClick(int x, int y, int repeatTimes)
        {
            MoveTo(x, y);
            for (int i = 0; i < repeatTimes; i++)
            {
                Win32API.mouse_event((int)Win32API.MouseEventFlags.RightDown, 0, 0, 0, IntPtr.Zero);
                Win32API.mouse_event((int)Win32API.MouseEventFlags.RightUp, 0, 0, 0, IntPtr.Zero);
            }
        }

        public static void Click()
        {
            NClick(-99, -99, 1);
        }

        public static void Click(int x, int y)
        {
            NClick(x, y, 1);
        }

        public static void Click(Point point)
        {
            Click(point.X, point.Y);
        }

        public static void DoubleClick()
        {
            NClick(-99, -99, 2);
        }
        public static void DoubleClick(int x, int y)
        {
            NClick(x, y, 2);
        }

        public static void RightClick()
        {
            NRightClick(-99, -99, 1);
        }
        public static void RightClick(int x, int y)
        {
            NRightClick(x, y, 1);
        }

        public static void MoveTo(int x, int y)
        {
            //can not move cursor to an invisible position.
            // the new position must be larger than -1.
            // if the new position is smaller than -1, then don't move.
            if (x > -1 && y > -1)
            {
                Win32API.SetCursorPos(x, y);
            }
        }

        public static void MoveTo(Point p)
        {
            MoveTo(p.X, p.Y);
        }
        #endregion
    }
}