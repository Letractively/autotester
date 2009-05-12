using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
    public class AsstFunctions
    {
        private AsstFunctions()
        {
        }


        public static void CloseWindow(IntPtr handle)
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    Win32API.SendMessage(handle, (int)Win32API.WindowMessages.WM_CLOSE, 0, 0);
                }
            }
            catch
            {
            }
        }
    }
}
