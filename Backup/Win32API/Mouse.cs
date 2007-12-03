using System;
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
        public static void NClick(int x, int y, int repeatTimes)
        {
            MoveTo(x, y);
            for (int i = 0; i < repeatTimes; i++)
            {
                Win32API.mouse_event((int)Win32API.MouseEventFlags.LeftDown, 0, 0, 0, IntPtr.Zero);
                Win32API.mouse_event((int)Win32API.MouseEventFlags.LeftUp, 0, 0, 0, IntPtr.Zero);
            }
        }
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
            //can not move cursor to an invisible position
            if (x > -1 && y > -1)
            {
                Win32API.SetCursorPos(x, y);
            }
        }
        #endregion
    }
}