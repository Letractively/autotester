using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core.Helper
{
    public sealed class WindowsAsstFunctions
    {
        #region fields

        private static String IE_Version;
        private static int IE_MajorVersion = 0;

        #endregion

        #region methods

        static WindowsAsstFunctions()
        {
            IE_Version = GetIEVersion();
            IE_MajorVersion = GetIEMajorVersion();
        }

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

        public static void CloseWindow(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                Win32API.SendMessage(handle, (int)Win32API.WindowMessages.WM_CLOSE, 0, 0);
            }
        }

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

        /* IntPtr GetIEMainHandle()
      * return the handle of IEFrame, this is the parent handle of Internet Explorer. 
      * we can use Spy++ to get the the handle.
      */
        public static IntPtr GetIEMainHandle()
        {
            return Win32API.FindWindow(TestConstants.IE_IEframe, null);
        }

        /* IntPtr GetShellDocHandle(IntPtr mainHandle)
         * return the handle of Shell DocObject View.
         * we can use Spy++ to get the tree structrue of Internet Explorer handles. 
         */
        public static IntPtr GetShellDocHandle(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
            {
                mainHandle = GetIEMainHandle();
            }

            IntPtr shellDocHandle = IntPtr.Zero;

            //lower version than IE 7.0
            if (IE_MajorVersion < 7)
            {
                shellDocHandle = Win32API.FindWindowEx(mainHandle, IntPtr.Zero, TestConstants.IE_ShellDocView_Class, null);
            }
            else if (IE_MajorVersion == 7)
            {
                IntPtr tabWindow = IntPtr.Zero;
                //get the active tab.
                while (!Win32API.IsWindowVisible(tabWindow))
                {
                    tabWindow = Win32API.FindWindowEx(mainHandle, tabWindow, TestConstants.IE_TabWindow_Class, null);
                }

                shellDocHandle = Win32API.FindWindowEx(tabWindow, IntPtr.Zero, TestConstants.IE_ShellDocView_Class, null);
            }
            else if (IE_MajorVersion == 8)
            {
                IntPtr frame = Win32API.FindWindowEx(mainHandle, IntPtr.Zero, TestConstants.IE_FrameTab_Class, null);

                if (frame != IntPtr.Zero)
                {
                    IntPtr tabWindow = IntPtr.Zero;
                    //get the active tab.
                    while (!Win32API.IsWindowVisible(tabWindow))
                    {
                        tabWindow = Win32API.FindWindowEx(frame, tabWindow, TestConstants.IE_TabWindow_Class, null);
                    }
                    shellDocHandle = Win32API.FindWindowEx(tabWindow, IntPtr.Zero, TestConstants.IE_ShellDocView_Class, null);
                }
            }

            return shellDocHandle;
        }

        /* IntPtr GetIEServerHandle(IntPtr shellHandle)
         * return the handle of Internet Explorer_Server.
         * This control is used to display web page.
         */
        public static IntPtr GetIEServerHandle(IntPtr shellHandle)
        {
            if (shellHandle == IntPtr.Zero)
            {
                shellHandle = GetShellDocHandle(IntPtr.Zero);
            }

            return Win32API.FindWindowEx(shellHandle, IntPtr.Zero, TestConstants.IE_Server_Class, null);
        }

        /* IntPtr GetDialogHandle(IntPtr mainHandle)
         * return the handle of pop up page.
         * the name is Internet Explorer_TridentDlgFrame.
         */
        public static IntPtr GetDialogHandle(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
            {
                mainHandle = GetIEMainHandle();
            }

            IntPtr popHandle = Win32API.FindWindow(TestConstants.IE_Dialog_Class, null);
            if (popHandle != IntPtr.Zero)
            {
                IntPtr parentHandle = Win32API.GetParent(popHandle);
                if (parentHandle == mainHandle)
                {
                    return popHandle;
                }
            }

            return IntPtr.Zero;
        }

        public static IntPtr GetTabHandle(IntPtr mainHandle, int ieVersion)
        {
            if (ieVersion > 6 && mainHandle != IntPtr.Zero)
            {
                IntPtr directUI = Win32API.FindWindowEx(mainHandle, IntPtr.Zero, "CommandBarClass", null);
                directUI = Win32API.FindWindowEx(directUI, IntPtr.Zero, "ReBarWindow32", null);
                directUI = Win32API.FindWindowEx(directUI, IntPtr.Zero, "TabBandClass", null);
                directUI = Win32API.FindWindowEx(directUI, IntPtr.Zero, "DirectUIHWND", null);
                return directUI;
            }

            return IntPtr.Zero;
        }

        public static String GetIEVersion()
        {
            if (IE_Version == null)
            {
                using (Microsoft.Win32.RegistryKey versionKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(TestConstants.IE_Reg_Path))
                {
                    IE_Version = versionKey.GetValue("Version").ToString();
                }
            }

            return IE_Version;
        }

        public static int GetIEMajorVersion()
        {
            if (IE_MajorVersion == 0)
            {
                String version = GetIEVersion();
                IE_MajorVersion = int.Parse(version[0].ToString());
            }

            return IE_MajorVersion;
        }

        #endregion

        #endregion

    }
}
