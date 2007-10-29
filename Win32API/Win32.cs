using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;

namespace Shrinerain.AutoTester.Win32
{
    public sealed class Win32API
    {

        //Not allow to create instance

        private Win32API()
        {

        }


        public const uint MAX_PATH = 260;

        #region Hooks - user32.dll

        /// <summary>
        /// Defines the layout of the MouseHookStruct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public Point Point;
            public int MouseData;
            public int Flags;
            public int Time;
            public int ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            //     public int type; 
            //     struct SendEvent{
            //            MOUSEINPUT mi; 
            //            KEYBDINPUT ki;
            //            HARDWAREINPUT hi;
            //           };
        }

        //[StructLayout(LayoutKind.Sequential)]
        //public struct MOUSEINPUT {
        //                          LONG dx;
        //                          LONG dy;
        //                          DWORD mouseData;
        //                          DWORD dwFlags;
        //                          DWORD time;
        //                          ULONG_PTR dwExtraInfo;
        //                          } 

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            /// <summary>
            /// Specifies a POINT structure that contains the x- and y-coordinates of the cursor, in screen coordinates.
            /// </summary>
            public POINT Point;

            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public UInt32 ExtraInfo;
        }

        /// <summary>
        /// 键盘钩子事件结构定义
        /// </summary>
        /// <remarks>详细说明请参考MSDN中关于 KBDLLHOOKSTRUCT 的说明</remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookStruct
        {
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            /// </summary>
            public UInt32 VKCode;

            /// <summary>
            /// Specifies a hardware scan code for the key.
            /// </summary>
            public UInt32 ScanCode;

            /// <summary>
            /// Specifies the extended-key flag, event-injected flag, context code, 
            /// and transition-state flag. This member is specified as follows. 
            /// An application can use the following values to test the keystroke flags. 
            /// </summary>
            public UInt32 Flags;

            /// <summary>
            /// Specifies the time stamp for this message. 
            /// </summary>
            public UInt32 Time;

            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public UInt32 ExtraInfo;
        }

        /// <summary>
        ///  EventArgs class for use as parameters by HookEventHandler delegates
        /// </summary>
        public class HookEventArgs : EventArgs
        {
            private int _code;
            private IntPtr _wParam;
            private IntPtr _lParam;

            /// <summary>
            /// Initializes a new instance of the HookEventArgs class
            /// </summary>
            /// <param name="code">the hook code</param>
            /// <param name="wParam">hook specific information</param>
            /// <param name="lParam">hook specific information</param>
            public HookEventArgs(int code, IntPtr wParam, IntPtr lParam)
            {
                _code = code;
                _wParam = wParam;
                _lParam = lParam;
            }

            /// <summary>
            /// The hook code
            /// </summary>
            public int Code
            {
                get
                {
                    return _code;
                }
            }

            /// <summary>
            /// A pointer to data
            /// </summary>
            public IntPtr wParam
            {
                get
                {
                    return _wParam;
                }
            }

            /// <summary>
            /// A pointer to data
            /// </summary>
            public IntPtr lParam
            {
                get
                {
                    return _lParam;
                }
            }
        }

        /// <summary>
        /// Event delegate for use with the HookEventArgs class
        /// </summary>
        public delegate void HookEventHandler(object sender, HookEventArgs e);

        /// <summary>
        /// Defines the various types of hooks that are available in Windows
        /// </summary>
        public enum HookTypes : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public enum ShellHookMessages
        {
            HSHELL_WINDOWCREATED = 1,
            HSHELL_WINDOWDESTROYED = 2,
            HSHELL_ACTIVATESHELLWINDOW = 3,
            HSHELL_WINDOWACTIVATED = 4,
            HSHELL_GETMINRECT = 5,
            HSHELL_REDRAW = 6,
            HSHELL_TASKMAN = 7,
            HSHELL_LANGUAGE = 8,
            HSHELL_ACCESSIBILITYSTATE = 11
        }
        public enum WM_MOUSE : int
        {
            /// <summary>
            /// 鼠标开始
            /// </summary>
            WM_MOUSEFIRST = 0x200,

            /// <summary>
            /// 鼠标移动
            /// </summary>
            WM_MOUSEMOVE = 0x200,

            /// <summary>
            /// 左键按下
            /// </summary>
            WM_LBUTTONDOWN = 0x201,

            /// <summary>
            /// 左键释放
            /// </summary>
            WM_LBUTTONUP = 0x202,

            /// <summary>
            /// 左键双击
            /// </summary>
            WM_LBUTTONDBLCLK = 0x203,

            /// <summary>
            /// 右键按下
            /// </summary>
            WM_RBUTTONDOWN = 0x204,

            /// <summary>
            /// 右键释放
            /// </summary>
            WM_RBUTTONUP = 0x205,

            /// <summary>
            /// 右键双击
            /// </summary>
            WM_RBUTTONDBLCLK = 0x206,

            /// <summary>
            /// 中键按下
            /// </summary>
            WM_MBUTTONDOWN = 0x207,

            /// <summary>
            /// 中键释放
            /// </summary>
            WM_MBUTTONUP = 0x208,

            /// <summary>
            /// 中键双击
            /// </summary>
            WM_MBUTTONDBLCLK = 0x209,

            /// <summary>
            /// 滚轮滚动
            /// </summary>
            /// <remarks>WINNT4.0以上才支持此消息</remarks>
            WM_MOUSEWHEEL = 0x020A
        }

        //message for list box
        public enum LISTBOXMSG : int
        {
            LB_ADDSTRING = 0x0180,
            LB_INSERTSTRING = 0x0181,
            LB_DELETESTRING = 0x0182,
            LB_SELITEMRANGEEX = 0x0183,
            LB_RESETCONTENT = 0x0184,
            LB_SETSEL = 0x0185,
            LB_SETCURSEL = 0x0186,
            LB_GETSEL = 0x0187,
            LB_GETCURSEL = 0x0188,
            LB_GETTEXT = 0x0189,
            LB_GETTEXTLEN = 0x018A,
            LB_GETCOUNT = 0x018B,
            LB_SELECTSTRING = 0x018C,
            LB_DIR = 0x018D,
            LB_GETTOPINDEX = 0x018E,
            LB_FINDSTRING = 0x018F,
            LB_GETSELCOUNT = 0x0190,
            LB_GETSELITEMS = 0x0191,
            LB_SETTABSTOPS = 0x0192,
            LB_GETHORIZONTALEXTENT = 0x0193,
            LB_SETHORIZONTALEXTENT = 0x0194,
            LB_SETCOLUMNWIDTH = 0x0195,
            LB_ADDFILE = 0x0196,
            LB_SETTOPINDEX = 0x0197,
            LB_GETITEMRECT = 0x0198,
            LB_GETITEMDATA = 0x0199,
            LB_SETITEMDATA = 0x019A,
            LB_SELITEMRANGE = 0x019B,
            LB_SETANCHORINDEX = 0x019C,
            LB_GETANCHORINDEX = 0x019D,
            LB_SETCARETINDEX = 0x019E,
            LB_GETCARETINDEX = 0x019F,
            LB_SETITEMHEIGHT = 0x01A0,
            LB_GETITEMHEIGHT = 0x01A1,
            LB_FINDSTRINGEXACT = 0x01A2,
            LB_SETLOCALE = 0x01A5,
            LB_GETLOCALE = 0x01A6,
            LB_SETCOUNT = 0x01A7,
        }


        //message for combo box.
        public enum COMBOBOXMSG : int
        {
            CB_OKAY = 0,
            CB_ERR = (-1),
            CB_ERRSPACE = (-2),
            CBN_ERRSPACE = (-1),
            CBN_SELCHANGE = 1,
            CBN_DBLCLK = 2,
            CBN_SETFOCUS = 3,
            CBN_KILLFOCUS = 4,
            CBN_EDITCHANGE = 5,
            CBN_EDITUPDATE = 6,
            CBN_DROPDOWN = 7,
            CBN_CLOSEUP = 8,
            CBN_SELENDOK = 9,
            CBN_SELENDCANCEL = 10,
            CBS_SIMPLE = 0x0001,
            CBS_DROPDOWN = 0x0002,
            CBS_DROPDOWNLIST = 0x0003,
            CBS_OWNERDRAWFIXED = 0x0010,
            CBS_OWNERDRAWVARIABLE = 0x0020,
            CBS_AUTOHSCROLL = 0x0040,
            CBS_OEMCONVERT = 0x0080,
            CBS_SORT = 0x0100,
            CBS_HASSTRINGS = 0x0200,
            CBS_NOINTEGRALHEIGHT = 0x0400,
            CBS_DISABLENOSCROLL = 0x0800,
            CBS_UPPERCASE = 0x2000,
            CBS_LOWERCASE = 0x4000,
            CB_GETEDITSEL = 0x0140,
            CB_LIMITTEXT = 0x0141,
            CB_SETEDITSEL = 0x0142,
            CB_ADDSTRING = 0x0143,
            CB_DELETESTRING = 0x0144,
            CB_DIR = 0x0145,
            CB_GETCOUNT = 0x0146,
            CB_GETCURSEL = 0x0147,
            CB_GETLBTEXT = 0x0148,
            CB_GETLBTEXTLEN = 0x0149,
            CB_INSERTSTRING = 0x014A,
            CB_RESETCONTENT = 0x014B,
            CB_FINDSTRING = 0x014C,
            CB_SELECTSTRING = 0x014D,
            CB_SETCURSEL = 0x014E,
            CB_SHOWDROPDOWN = 0x014F,
            CB_GETITEMDATA = 0x0150,
            CB_SETITEMDATA = 0x0151,
            CB_GETDROPPEDCONTROLRECT = 0x0152,
            CB_SETITEMHEIGHT = 0x0153,
            CB_GETITEMHEIGHT = 0x0154,
            CB_SETEXTENDEDUI = 0x0155,
            CB_GETEXTENDEDUI = 0x0156,
            CB_GETDROPPEDSTATE = 0x0157,
            CB_FINDSTRINGEXACT = 0x0158,
            CB_SETLOCALE = 0x0159,
            CB_GETLOCALE = 0x015A,
            CB_GETTOPINDEX = 0x015b,
            CB_SETTOPINDEX = 0x015c,
            CB_GETHORIZONTALEXTENT = 0x015d,
            CB_SETHORIZONTALEXTENT = 0x015e,
            CB_GETDROPPEDWIDTH = 0x015f,
            CB_SETDROPPEDWIDTH = 0x0160,
            CB_INITSTORAGE = 0x0161,
            CB_MSGMAX = 0x0162,
        }

        public enum WM_KEYBOARD : int
        {
            /// <summary>
            /// 非系统按键按下
            /// </summary>
            WM_KEYDOWN = 0x100,

            /// <summary>
            /// 非系统按键释放
            /// </summary>
            WM_KEYUP = 0x101,

            /// <summary>
            /// 系统按键按下
            /// </summary>
            WM_SYSKEYDOWN = 0x104,

            /// <summary>
            /// 系统按键释放
            /// </summary>
            WM_SYSKEYUP = 0x105
        }
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true, EntryPoint = "SetWindowsHookEx")]
        public static extern IntPtr SetWindowsHookEx(HookTypes hookType, HookProc hookProc, IntPtr hInstance, int nThreadId);

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hookHandle, int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int RegisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int DeregisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ToAscii(UInt32 uVirtKey, UInt32 uScanCode,
            byte[] lpbKeyState, byte[] lpwTransKey, UInt32 fuState);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        #endregion Hooks

        #region System - Kernel32.dll

        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 GetLastError();

        #endregion

        #region Windows - user32.dll

        public const int GWL_HWNDPARENT = (-8);
        public const int GWL_EXSTYLE = (-20);
        public const int GWL_STYLE = (-16);
        public const int GCL_HICON = (-14);
        public const int GCL_HICONSM = (-34);
        public const int WM_QUERYDRAGICON = 0x37;
        public const int WM_GETICON = 0x7F;
        public const int WM_SETICON = 0x80;
        public const int ICON_SMALL = 0;
        public const int ICON_BIG = 1;
        public const int SMTO_ABORTIFHUNG = 0x2;
        public const int TRUE = 1;
        public const int FALSE = 0;

        public const int WHITE_BRUSH = 0;
        public const int LTGRAY_BRUSH = 1;
        public const int GRAY_BRUSH = 2;
        public const int DKGRAY_BRUSH = 3;
        public const int BLACK_BRUSH = 4;
        public const int NULL_BRUSH = 5;
        public const int HOLLOW_BRUSH = NULL_BRUSH;
        public const int WHITE_PEN = 6;
        public const int BLACK_PEN = 7;
        public const int NULL_PEN = 8;
        public const int OEM_FIXED_FONT = 10;
        public const int ANSI_FIXED_FONT = 11;
        public const int ANSI_VAR_FONT = 12;
        public const int SYSTEM_FONT = 13;
        public const int DEVICE_DEFAULT_FONT = 14;
        public const int DEFAULT_PALETTE = 15;
        public const int SYSTEM_FIXED_FONT = 16;


        public const int RDW_INVALIDATE = 0x0001;
        public const int RDW_INTERNALPAINT = 0x0002;
        public const int RDW_ERASE = 0x0004;

        public const int RDW_VALIDATE = 0x0008;
        public const int RDW_NOINTERNALPAINT = 0x0010;
        public const int RDW_NOERASE = 0x0020;

        public const int RDW_NOCHILDREN = 0x0040;
        public const int RDW_ALLCHILDREN = 0x0080;

        public const int RDW_UPDATENOW = 0x0100;
        public const int RDW_ERASENOW = 0x0200;

        public const int RDW_FRAME = 0x0400;
        public const int RDW_NOFRAME = 0x0800;



        public enum ShowWindowCmds
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        public const int HIDE_WINDOW = 0;
        public const int SHOW_OPENWINDOW = 1;
        public const int SHOW_ICONWINDOW = 2;
        public const int SHOW_FULLSCREEN = 3;
        public const int SHOW_OPENNOACTIVATE = 4;
        public const int SW_PARENTCLOSING = 1;
        public const int SW_OTHERZOOM = 2;
        public const int SW_PARENTOPENING = 3;
        public const int SW_OTHERUNZOOM = 4;

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_NOREDRAW = 0x0008;
        public const int SWP_NOACTIVATE = 0x0010;
        public const int SWP_FRAMECHANGED = 0x0020; /* The frame changed: send WM_NCCALCSIZE */
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_HIDEWINDOW = 0x0080;
        public const int SWP_NOCOPYBITS = 0x0100;
        public const int SWP_NOOWNERZORDER = 0x0200; /* Don't do owner Z ordering */
        public const int SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */
        public const int SWP_DRAWFRAME = SWP_FRAMECHANGED;
        public const int SWP_NOREPOSITION = SWP_NOOWNERZORDER;
        public const int SWP_DEFERERASE = 0x2000;
        public const int SWP_ASYNCWINDOWPOS = 0x4000;

        public const int HWND_TOP = 0;
        public const int HWND_BOTTOM = 1;
        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;

        public enum PeekMessageFlags
        {
            PM_NOREMOVE = 0,
            PM_REMOVE = 1,
            PM_NOYIELD = 2
        }

        public enum MouseEventFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            Wheel = 0x0800,
            Absolute = 0x8000
        }

        public enum WindowMessages
        {
            WM_NULL = 0x0000,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_MOVE = 0x0003,
            WM_SIZE = 0x0005,
            WM_ACTIVATE = 0x0006,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_ENABLE = 0x000A,
            WM_SETREDRAW = 0x000B,
            WM_SETTEXT = 0x000C,
            WM_GETTEXT = 0x000D,
            WM_GETTEXTLENGTH = 0x000E,
            WM_PAINT = 0x000F,
            WM_CLOSE = 0x0010,
            WM_QUERYENDSESSION = 0x0011,
            WM_QUIT = 0x0012,
            WM_QUERYOPEN = 0x0013,
            WM_ERASEBKGND = 0x0014,
            WM_SYSCOLORCHANGE = 0x0015,
            WM_ENDSESSION = 0x0016,
            WM_SHOWWINDOW = 0x0018,
            WM_CTLCOLOR = 0x0019,
            WM_WININICHANGE = 0x001A,
            WM_SETTINGCHANGE = 0x001A,
            WM_DEVMODECHANGE = 0x001B,
            WM_ACTIVATEAPP = 0x001C,
            WM_FONTCHANGE = 0x001D,
            WM_TIMECHANGE = 0x001E,
            WM_CANCELMODE = 0x001F,
            WM_SETCURSOR = 0x0020,
            WM_MOUSEACTIVATE = 0x0021,
            WM_CHILDACTIVATE = 0x0022,
            WM_QUEUESYNC = 0x0023,
            WM_GETMINMAXINFO = 0x0024,
            WM_PAINTICON = 0x0026,
            WM_ICONERASEBKGND = 0x0027,
            WM_NEXTDLGCTL = 0x0028,
            WM_SPOOLERSTATUS = 0x002A,
            WM_DRAWITEM = 0x002B,
            WM_MEASUREITEM = 0x002C,
            WM_DELETEITEM = 0x002D,
            WM_VKEYTOITEM = 0x002E,
            WM_CHARTOITEM = 0x002F,
            WM_SETFONT = 0x0030,
            WM_GETFONT = 0x0031,
            WM_SETHOTKEY = 0x0032,
            WM_GETHOTKEY = 0x0033,
            WM_QUERYDRAGICON = 0x0037,
            WM_COMPAREITEM = 0x0039,
            WM_GETOBJECT = 0x003D,
            WM_COMPACTING = 0x0041,
            WM_COMMNOTIFY = 0x0044,
            WM_WINDOWPOSCHANGING = 0x0046,
            WM_WINDOWPOSCHANGED = 0x0047,
            WM_POWER = 0x0048,
            WM_COPYDATA = 0x004A,
            WM_CANCELJOURNAL = 0x004B,
            WM_NOTIFY = 0x004E,
            WM_INPUTLANGCHANGEREQUEST = 0x0050,
            WM_INPUTLANGCHANGE = 0x0051,
            WM_TCARD = 0x0052,
            WM_HELP = 0x0053,
            WM_USERCHANGED = 0x0054,
            WM_NOTIFYFORMAT = 0x0055,
            WM_CONTEXTMENU = 0x007B,
            WM_STYLECHANGING = 0x007C,
            WM_STYLECHANGED = 0x007D,
            WM_DISPLAYCHANGE = 0x007E,
            WM_GETICON = 0x007F,
            WM_SETICON = 0x0080,
            WM_NCCREATE = 0x0081,
            WM_NCDESTROY = 0x0082,
            WM_NCCALCSIZE = 0x0083,
            WM_NCHITTEST = 0x0084,
            WM_NCPAINT = 0x0085,
            WM_NCACTIVATE = 0x0086,
            WM_GETDLGCODE = 0x0087,
            WM_SYNCPAINT = 0x0088,
            WM_NCMOUSEMOVE = 0x00A0,
            WM_NCLBUTTONDOWN = 0x00A1,
            WM_NCLBUTTONUP = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN = 0x00A4,
            WM_NCRBUTTONUP = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN = 0x00A7,
            WM_NCMBUTTONUP = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_DEADCHAR = 0x0103,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
            WM_SYSDEADCHAR = 0x0107,
            WM_KEYLAST = 0x0108,
            WM_IME_STARTCOMPOSITION = 0x010D,
            WM_IME_ENDCOMPOSITION = 0x010E,
            WM_IME_COMPOSITION = 0x010F,
            WM_IME_KEYLAST = 0x010F,
            WM_INITDIALOG = 0x0110,
            WM_COMMAND = 0x0111,
            WM_SYSCOMMAND = 0x0112,
            WM_TIMER = 0x0113,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
            WM_INITMENU = 0x0116,
            WM_INITMENUPOPUP = 0x0117,
            WM_MENUSELECT = 0x011F,
            WM_MENUCHAR = 0x0120,
            WM_ENTERIDLE = 0x0121,
            WM_MENURBUTTONUP = 0x0122,
            WM_MENUDRAG = 0x0123,
            WM_MENUGETOBJECT = 0x0124,
            WM_UNINITMENUPOPUP = 0x0125,
            WM_MENUCOMMAND = 0x0126,
            WM_CTLCOLORMSGBOX = 0x0132,
            WM_CTLCOLOREDIT = 0x0133,
            WM_CTLCOLORLISTBOX = 0x0134,
            WM_CTLCOLORBTN = 0x0135,
            WM_CTLCOLORDLG = 0x0136,
            WM_CTLCOLORSCROLLBAR = 0x0137,
            WM_CTLCOLORSTATIC = 0x0138,
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_RBUTTONDBLCLK = 0x0206,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208,
            WM_MBUTTONDBLCLK = 0x0209,
            WM_MOUSEWHEEL = 0x020A,
            WM_PARENTNOTIFY = 0x0210,
            WM_ENTERMENULOOP = 0x0211,
            WM_EXITMENULOOP = 0x0212,
            WM_NEXTMENU = 0x0213,
            WM_SIZING = 0x0214,
            WM_CAPTURECHANGED = 0x0215,
            WM_MOVING = 0x0216,
            WM_DEVICECHANGE = 0x0219,
            WM_MDICREATE = 0x0220,
            WM_MDIDESTROY = 0x0221,
            WM_MDIACTIVATE = 0x0222,
            WM_MDIRESTORE = 0x0223,
            WM_MDINEXT = 0x0224,
            WM_MDIMAXIMIZE = 0x0225,
            WM_MDITILE = 0x0226,
            WM_MDICASCADE = 0x0227,
            WM_MDIICONARRANGE = 0x0228,
            WM_MDIGETACTIVE = 0x0229,
            WM_MDISETMENU = 0x0230,
            WM_ENTERSIZEMOVE = 0x0231,
            WM_EXITSIZEMOVE = 0x0232,
            WM_DROPFILES = 0x0233,
            WM_MDIREFRESHMENU = 0x0234,
            WM_IME_SETCONTEXT = 0x0281,
            WM_IME_NOTIFY = 0x0282,
            WM_IME_CONTROL = 0x0283,
            WM_IME_COMPOSITIONFULL = 0x0284,
            WM_IME_SELECT = 0x0285,
            WM_IME_CHAR = 0x0286,
            WM_IME_REQUEST = 0x0288,
            WM_IME_KEYDOWN = 0x0290,
            WM_IME_KEYUP = 0x0291,
            WM_MOUSEHOVER = 0x02A1,
            WM_MOUSELEAVE = 0x02A3,
            WM_CUT = 0x0300,
            WM_COPY = 0x0301,
            WM_PASTE = 0x0302,
            WM_CLEAR = 0x0303,
            WM_UNDO = 0x0304,
            WM_RENDERFORMAT = 0x0305,
            WM_RENDERALLFORMATS = 0x0306,
            WM_DESTROYCLIPBOARD = 0x0307,
            WM_DRAWCLIPBOARD = 0x0308,
            WM_PAINTCLIPBOARD = 0x0309,
            WM_VSCROLLCLIPBOARD = 0x030A,
            WM_SIZECLIPBOARD = 0x030B,
            WM_ASKCBFORMATNAME = 0x030C,
            WM_CHANGECBCHAIN = 0x030D,
            WM_HSCROLLCLIPBOARD = 0x030E,
            WM_QUERYNEWPALETTE = 0x030F,
            WM_PALETTEISCHANGING = 0x0310,
            WM_PALETTECHANGED = 0x0311,
            WM_HOTKEY = 0x0312,
            WM_PRINT = 0x0317,
            WM_PRINTCLIENT = 0x0318,
            WM_HANDHELDFIRST = 0x0358,
            WM_HANDHELDLAST = 0x035F,
            WM_AFXFIRST = 0x0360,
            WM_AFXLAST = 0x037F,
            WM_PENWINFIRST = 0x0380,
            WM_PENWINLAST = 0x038F,
            WM_APP = 0x8000,
            WM_USER = 0x0400,
            WM_REFLECT = WM_USER + 0x1c00
        }

        public enum WindowMenuMessage : int
        {
            SC_SIZE = 0xF000,
            SC_MOVE = 0xF010,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_NEXTWINDOW = 0xF040,
            SC_PREVWINDOW = 0xF050,
            SC_CLOSE = 0xF060,
            SC_VSCROLL = 0xF070,
            SC_HSCROLL = 0xF080,
            SC_MOUSEMENU = 0xF090,
            SC_KEYMENU = 0xF100,
            SC_ARRANGE = 0xF110,
            SC_RESTORE = 0xF120,
            SC_TASKLIST = 0xF130,
            SC_SCREENSAVE = 0xF140,
            SC_HOTKEY = 0xF150,
            SC_DEFAULT = 0xF160,
            SC_MONITORPOWER = 0xF170,
            SC_CONTEXTHELP = 0xF180,
            SC_SEPARATOR = 0xF00F
        }

        /// <summary>
        /// Defines the style bits that a window can have
        /// </summary>
        public enum WindowStyles : uint
        {
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xC00000,//               '  WS_BORDER Or WS_DLGFRAME
            WS_CHILD = 0x40000000,
            WS_CHILDWINDOW = (WS_CHILD),
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_EX_ACCEPTFILES = 0x10,
            WS_EX_DLGMODALFRAME = 0x1,
            WS_EX_NOPARENTNOTIFY = 0x4,
            WS_EX_TOPMOST = 0x8,
            WS_EX_TRANSPARENT = 0x20,
            WS_EX_TOOLWINDOW = 0x80,
            WS_EX_APPWINDOW = 0x40000,
            WS_GROUP = 0x20000,
            WS_HSCROLL = 0x100000,
            WS_MAXIMIZE = 0x1000000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x20000,
            WS_OVERLAPPED = 0x0,
            WS_POPUP = 0x80000000,
            WS_SYSMENU = 0x80000,
            WS_TABSTOP = 0x10000,
            WS_THICKFRAME = 0x40000,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x200000,
            WS_ICONIC = WS_MINIMIZE,
            WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX)
        }

        public struct WindowInfo
        {
            public int size;
            public Rectangle window;
            public Rectangle client;
            public int style;
            public int exStyle;
            public int windowStatus;
            public uint xWindowBorders;
            public uint yWindowBorders;
            public short atomWindowtype;
            public short creatorVersion;
        }

        public struct WindowPlacement
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point minPosition;
            public Point maxPosition;
            public Rectangle normalPosition;
        }

        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public int Width
            {
                get
                {
                    return right - left;
                }
            }

            public int Height
            {
                get
                {
                    return bottom - top;
                }
            }
        }

        public delegate bool EnumWindowEventHandler(IntPtr hWnd, Int32 lParam);

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static unsafe extern int GetCursorPos(POINT* lpPoint);

        [DllImport("User32.dll")]
        public extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);


        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("oleacc.dll")]
        public static extern IntPtr AccessibleObjectFromPoint(POINT pt, [Out, MarshalAs(UnmanagedType.Interface)] out IAccessible accObj, [Out] out object ChildID);

        [DllImport("oleacc.dll", ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public static extern object AccessibleObjectFromWindow(
            IntPtr hwnd,
            Int32 dwObjectID,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
         ref IAccessible ppvObject);

        [DllImport("user32.dll")]
        public static extern int GetWindowModuleFileName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr window, ref WindowPlacement position);

        [DllImport("User32.dll")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("User32.dll")]
        public static extern void EnumWindows(EnumWindowEventHandler callback, Int32 lParam);

        //		[DllImport("User32.dll")]
        //		public static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

        [DllImport("User32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern Int32 GetWindow(IntPtr hWnd, Int32 wCmd);

        //		[DllImport("User32.dll")]
        //		public static extern WindowStyles GetWindowLong(IntPtr hWnd, Int32 index);

        [DllImport("User32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern WindowStyles GetWindowLong(IntPtr hWnd, int index);

        [DllImport("User32.dll")]
        public static extern int SendMessageTimeout(IntPtr hWnd, int uMsg, int wParam, int lParam, int fuFlags, int uTimeout, out int lpdwResult);

        [DllImport("User32.dll")]
        public static extern int GetClassLong(IntPtr hWnd, int index);

        [DllImport("User32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("User32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, INPUT pInputs, int cbSize);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        [DllImport("User32.dll")]
        public static extern int PostMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        [DllImport("User32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, int altTabActivated);

        [DllImport("User32.dll")]
        public static extern int ShowWindowAsync(IntPtr hWnd, int command);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr window);

        [DllImport("user32.dll")]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WindowInfo info);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, ref Rect rectangle);

        //[DllImport("user32.dll")]
        //public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDc, IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern IntPtr GetStockObject(int nObject);

        [DllImport("user32.dll")]
        public static extern int InvalidateRect(IntPtr hWnd, IntPtr lpRect, int bErase);

        [DllImport("user32.dll")]
        public static extern int UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        #endregion Windows

        #region GDI - gdi32.dll

        public enum BinaryRasterOperations
        {
            R2_BLACK = 1,   /*  0       */
            R2_NOTMERGEPEN = 2,   /* DPon     */
            R2_MASKNOTPEN = 3,   /* DPna     */
            R2_NOTCOPYPEN = 4,   /* PN       */
            R2_MASKPENNOT = 5,   /* PDna     */
            R2_NOT = 6,   /* Dn       */
            R2_XORPEN = 7,   /* DPx      */
            R2_NOTMASKPEN = 8,   /* DPan     */
            R2_MASKPEN = 9,   /* DPa      */
            R2_NOTXORPEN = 10,  /* DPxn     */
            R2_NOP = 11,  /* D        */
            R2_MERGENOTPEN = 12,  /* DPno     */
            R2_COPYPEN = 13,  /* P        */
            R2_MERGEPENNOT = 14,  /* PDno     */
            R2_MERGEPEN = 15,  /* DPo      */
            R2_WHITE = 16,  /*  1       */
            R2_LAST = 16
        }

        public enum TernaryRasterOperations
        {
            SRCCOPY = 0x00CC0020, /* dest = source                   */
            SRCPAINT = 0x00EE0086, /* dest = source OR dest           */
            SRCAND = 0x008800C6, /* dest = source AND dest          */
            SRCINVERT = 0x00660046, /* dest = source XOR dest          */
            SRCERASE = 0x00440328, /* dest = source AND (NOT dest )   */
            NOTSRCCOPY = 0x00330008, /* dest = (NOT source)             */
            NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
            MERGECOPY = 0x00C000CA, /* dest = (source AND pattern)     */
            MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest     */
            PATCOPY = 0x00F00021, /* dest = pattern                  */
            PATPAINT = 0x00FB0A09, /* dest = DPSnoo                   */
            PATINVERT = 0x005A0049, /* dest = pattern XOR dest         */
            DSTINVERT = 0x00550009, /* dest = (NOT dest)               */
            BLACKNESS = 0x00000042, /* dest = BLACK                    */
            WHITENESS = 0x00FF0062 /* dest = WHITE                    */

        }

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDst, int xDst, int yDst, int cx, int cy, IntPtr hdcSrc, int xSrc, int ySrc, uint ulRop);

        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hdcDst, int xDst, int yDst, int cx, int cy, IntPtr hdcSrc, int xSrc, int ySrc, int cxSrc, int cySrc, uint ulRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(IntPtr lpszDriver, string lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hdc);

        #endregion

        #region Shlwapi.dll

        [DllImport("Shlwapi.dll")]
        public static extern string PathGetArgs(string path);

        public static string SafePathGetArgs(string path)
        {
            try
            {
                return Win32API.PathGetArgs(path);
            }
            catch (System.Exception) { }
            return string.Empty;
        }

        [DllImport("Shlwapi.dll")]
        public static extern int PathCompactPathEx(
            System.Text.StringBuilder pszOut, /* Address of the string that has been altered */
            System.Text.StringBuilder pszSrc, /* Pointer to a null-terminated string of max length (MAX_PATH) that contains the path to be altered */
            uint cchMax,					  /* Maximum number of chars to be contained in the new string, including the null character. Example: cchMax = 8, then 7 chars will be returned, the last for the null character. */
            uint dwFlags);					  /* Reserved */

        public static string PathCompactPathEx(string source, uint maxChars)
        {
            StringBuilder pszOut = new StringBuilder((int)Win32API.MAX_PATH);
            StringBuilder pszSrc = new StringBuilder(source);

            int result = Win32API.PathCompactPathEx(pszOut, pszSrc, maxChars, (uint)0);
            if (result == 1)
                return pszOut.ToString();
            else
            {
                System.Diagnostics.Debug.WriteLine("Win32.PathCompactPathEx failed to compact the path '" + source + "' down to '" + maxChars + "' characters.");
                return string.Empty;
            }
        }

        #endregion

        #region Hotkeys

        [Flags()]
        public enum HotkeyModifiers
        {
            MOD_ALT = 0x0001,
            MOD_CONTROL = 0x0002,
            MOD_SHIFT = 0x0004,
            MOD_WIN = 0x0008
        }

        [DllImport("User32")]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint modifiers, uint virtualkeyCode);

        [DllImport("User32")]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("Kernel32")]
        public static extern short GlobalAddAtom(string atomName);

        [DllImport("Kernel32")]
        public static extern short GlobalDeleteAtom(short atom);

        #endregion

        [DllImport("User32")]
        public static extern int LockWindowUpdate(IntPtr windowHandle);

        public static short MAKEWORD(byte a, byte b)
        {
            return ((short)(((byte)(a & 0xff)) | ((short)((byte)(b & 0xff))) << 8));
        }

        public static byte LOBYTE(short a)
        {
            return ((byte)(a & 0xff));
        }

        public static byte HIBYTE(short a)
        {
            return ((byte)(a >> 8));
        }

        public static int MAKELONG(short a, short b)
        {
            return (((int)(a & 0xffff)) | (((int)(b & 0xffff)) << 16));
        }

        public static short HIWORD(int a)
        {
            return ((short)(a >> 16));
        }

        public static short LOWORD(int a)
        {
            return ((short)(a & 0xffff));
        }

        [DllImport("Kernel32")]
        public static extern int CopyFile(string source, string destination, int failIfExists);
    }

};