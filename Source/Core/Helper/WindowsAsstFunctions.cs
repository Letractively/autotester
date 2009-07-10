using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
    public class WindowsAsstFunctions
    {
        private const int MAX_TIMEOUT = 3;

        #region methods

        #region private

        private static bool EnumWindows(IntPtr hwnd, IntPtr lParam)
        {
            if (hwnd != IntPtr.Zero)
            {
                GCHandle gch = (GCHandle)lParam;
                List<IntPtr> windowsList = gch.Target as List<IntPtr>;
                if (Win32API.IsWindow(hwnd) && Win32API.IsWindowVisible(hwnd))
                {
                    windowsList.Add(hwnd);
                }
            }
            return true;
        }

        private static bool EnumChildren(IntPtr hwnd, IntPtr lParam)
        {
            if (hwnd != IntPtr.Zero)
            {
                GCHandle gch = (GCHandle)lParam;
                List<IntPtr> childrenList = gch.Target as List<IntPtr>;
                if (Win32API.IsWindowVisible(hwnd))
                {
                    childrenList.Add(hwnd);
                }
            }
            return true;
        }

        #endregion

        #region public methods

        //get the sub windows of specific handle.
        public static List<IntPtr> GetSubWindows(IntPtr parent)
        {
            List<IntPtr> windowsList = new List<IntPtr>();
            GCHandle gch = GCHandle.Alloc(windowsList, GCHandleType.Normal);
            Win32API.EnumWindowEventHandler callbackProc = new Win32API.EnumWindowEventHandler(EnumWindows);
            Win32API.EnumWindows(callbackProc, (IntPtr)gch);

            if (parent != IntPtr.Zero && windowsList.Count > 0)
            {
                for (int i = 0; i < windowsList.Count; i++)
                {
                    IntPtr handle = windowsList[i];
                    if (parent != Win32API.GetParent(handle))
                    {
                        windowsList.Remove(handle);
                        i--;
                    }
                }
            }

            return windowsList;
        }

        public static List<IntPtr> GetChildren(IntPtr handle)
        {
            List<IntPtr> childrenList = new List<IntPtr>();
            GCHandle gch = GCHandle.Alloc(childrenList, GCHandleType.Normal);

            Win32API.EnumWindowsCallback callbackProc = new Win32API.EnumWindowsCallback(EnumChildren);
            Win32API.EnumChildWindows(handle, callbackProc, (IntPtr)gch);

            return childrenList;
        }

        public static IntPtr GetActiveDialog()
        {
            IntPtr handle = Win32API.GetForegroundWindow();
            if (handle != IntPtr.Zero)
            {
                String windowClass = Win32API.GetClassName(handle);
                if (String.Compare(TestConstants.WIN_Dialog_Class, windowClass, true) == 0)
                {
                    List<IntPtr> children = GetSubWindows(handle);
                    if (children != null && children.Count == 0)
                    {
                        foreach (IntPtr child in children)
                        {
                            string childClass = Win32API.GetClassName(handle);
                            if (String.Compare(TestConstants.WIN_Dialog_Class, childClass, true) == 0)
                            {
                                handle = child;
                                break;
                            }
                        }
                    }
                    return handle;
                }
            }
            return IntPtr.Zero;
        }

        #endregion

        #endregion

    }
}
