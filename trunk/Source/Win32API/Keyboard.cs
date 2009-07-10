/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: KeyboardOp.cs
*
* Description: This class defines the actions of keyborad.
*              you can input any keys. 
*              for special keys like tab, ctrl, you need to use {}.
*              like {tab} etc.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Shrinerain.AutoTester.Win32
{
    public sealed class KeyboardOp
    {
        #region fields

        private static bool _sendMessageOnly = false;

        public static bool SendMessageOnly
        {
            get { return KeyboardOp._sendMessageOnly; }
            set { KeyboardOp._sendMessageOnly = value; }
        }

        // private static Regex _specialKeysReg = new Regex(@"{[a-zA-Z]+}", RegexOptions.Compiled);

        #endregion

        #region ctor

        //can Not create instance
        private KeyboardOp()
        {

        }

        #endregion

        #region public methods

        /* void SendChars(string str)
         * send normal characters to system. 
         * except special keys like tab, ctrl etc.
         * NOTICE: why not use SendWait directly? 
         *         because I found sometimes SendWait can NOT send the character correctly.
         */
        public unsafe static void SendChars(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return;
            }

            //struct for keyboard event.
            Win32API.INPUT* structInput = stackalloc Win32API.INPUT[2];
            structInput[0].type = structInput[1].type = 1; //keyboard
            structInput[0].ki.wVk = structInput[1].ki.wVk = 0;
            structInput[0].ki.time = structInput[1].ki.time = 0;
            structInput[0].ki.dwFlags = structInput[1].ki.dwFlags = 4; //unicode

            for (int i = 0; i < str.Length; i++)
            {
                //key down
                structInput[0].ki.wScan = structInput[1].ki.wScan = str[i];
                //key up
                structInput[1].ki.dwFlags = 2;
                //send the character
                Win32API.SendInput(2, structInput, Marshal.SizeOf(*structInput));
            }
        }

        public static void SendChars(IntPtr handle, string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return;
            }

            if (_sendMessageOnly && handle != IntPtr.Zero)
            {
                Win32API.SendMessage(handle, (int)Win32API.WindowMessages.WM_SETTEXT, IntPtr.Zero, str);
            }
            else
            {
                SendChars(str);
            }
        }

        /* void SendKey(string keys)
         * send special keys to system. should use {}.
         * for example, if you want to send tab to system. uses {tab}.
         */
        public static void SendKey(string keys)
        {
            if (!String.IsNullOrEmpty(keys))
            {
                SendKeys.SendWait(keys);
            }
        }


        #region some common special keys

        public static void PressEnter()
        {
            PressKey(Win32API.VKCodes.VK_RETURN);
        }

        public static void PressSpace()
        {
            PressKey(Win32API.VKCodes.VK_SPACE);
        }

        public static void PressWindow()
        {
            PressKey(Win32API.VKCodes.VK_LWIN);
        }

        public static void PressCap()
        {
            PressKey(Win32API.VKCodes.VK_CAPITAL);
        }

        public static void PressShift()
        {
            PressKey(Win32API.VKCodes.VK_SHIFT);
        }

        public static void PressCtrl()
        {
            PressKey(Win32API.VKCodes.VK_CONTROL);
        }

        public static void PressAlt()
        {
            PressKey(Win32API.VKCodes.VK_MENU);
        }

        public static void PressEsc()
        {
            PressKey(Win32API.VKCodes.VK_ESCAPE);
        }

        public static void PressF1()
        {
            PressKey(Win32API.VKCodes.VK_F1);
        }

        public static void PressF2()
        {
            PressKey(Win32API.VKCodes.VK_F2);
        }

        public static void PressF3()
        {
            PressKey(Win32API.VKCodes.VK_F3);
        }

        public static void PressF4()
        {
            PressKey(Win32API.VKCodes.VK_F4);
        }

        public static void PressF5()
        {
            PressKey(Win32API.VKCodes.VK_F5);
        }

        public static void PressF6()
        {
            PressKey(Win32API.VKCodes.VK_F6);
        }

        public static void PressF7()
        {
            PressKey(Win32API.VKCodes.VK_F7);
        }

        public static void PressF8()
        {
            PressKey(Win32API.VKCodes.VK_F8);
        }

        public static void PressF9()
        {
            PressKey(Win32API.VKCodes.VK_F9);
        }

        public static void PressF10()
        {
            PressKey(Win32API.VKCodes.VK_F10);
        }

        public static void PressF11()
        {
            PressKey(Win32API.VKCodes.VK_F11);
        }

        public static void PressF12()
        {
            PressKey(Win32API.VKCodes.VK_F12);
        }

        public static void PressDel()
        {
            PressKey(Win32API.VKCodes.VK_DELETE);
        }

        public static void PressBackspace()
        {
            PressKey(Win32API.VKCodes.VK_BACK);
        }

        public static void PressInsert()
        {
            PressKey(Win32API.VKCodes.VK_INSERT);
        }

        public static void PressHome()
        {
            PressKey(Win32API.VKCodes.VK_HOME);
        }

        public static void PressEnd()
        {
            PressKey(Win32API.VKCodes.VK_END);
        }

        public static void PressPageUp()
        {
            PressKey(Win32API.VKCodes.VK_PRIOR);
        }

        public static void PressPageDown()
        {
            PressKey(Win32API.VKCodes.VK_NEXT);
        }

        public static void PressPrint()
        {
            PressKey(Win32API.VKCodes.VK_PRINT);
        }

        public static void PressUp()
        {
            PressKey(Win32API.VKCodes.VK_UP);
        }

        public static void PressRight()
        {
            PressKey(Win32API.VKCodes.VK_RIGHT);
        }

        public static void PressLeft()
        {
            PressKey(Win32API.VKCodes.VK_LEFT);
        }

        public static void PressDown()
        {
            PressKey(Win32API.VKCodes.VK_DOWN);
        }

        public static void PressNumLock()
        {
            PressKey(Win32API.VKCodes.VK_NUMLOCK);
        }

        /* PressKeys(Win32API.VKCodes[] vkCodes)
         * press combine keys, like alt+F4.
         */
        public unsafe static void PressKeys(Win32API.VKCodes[] vkCodes)
        {
            if (vkCodes == null || vkCodes.Length == 0)
            {
                return;
            }

            int len = vkCodes.Length;

            Win32API.INPUT* structInput = stackalloc Win32API.INPUT[len * 2];
            structInput[0].type = structInput[1].type = 1; //keyboard
            structInput[0].ki.time = structInput[1].ki.time = 0;
            structInput[0].ki.dwFlags = structInput[1].ki.dwFlags = 4; //unicode

            for (int i = 0; i < len; i++)
            {
                structInput[i].ki.wVk = structInput[len * 2 - i - 1].ki.wVk = (ushort)vkCodes[i];

                //key up
                structInput[len * 2 - i - 1].ki.dwFlags = 2;
            }

            //send the key
            Win32API.SendInput((uint)(len * 2), structInput, Marshal.SizeOf(*structInput));
        }

        public static void PressKeys(int[] vkCodes)
        {
            if (vkCodes.Length > 0)
            {
                Win32API.VKCodes[] codes = new Win32API.VKCodes[vkCodes.Length];

                for (int i = 0; i < vkCodes.Length; i++)
                {
                    codes[i] = (Win32API.VKCodes)vkCodes[i];
                }

                PressKeys(codes);
            }

        }

        /* void PressKey(Win32API.VKCodes vkCode)
         * Press a single key.
         */
        public unsafe static void PressKey(Win32API.VKCodes vkCode)
        {
            Win32API.INPUT* structInput = stackalloc Win32API.INPUT[2];
            structInput[0].type = structInput[1].type = 1; //keyboard
            structInput[0].ki.wVk = structInput[1].ki.wVk = (ushort)vkCode;
            structInput[0].ki.time = structInput[1].ki.time = 0;
            structInput[0].ki.dwFlags = structInput[1].ki.dwFlags = 4; //unicode

            //key up
            structInput[1].ki.dwFlags = 2;

            //send the key
            Win32API.SendInput(2, structInput, Marshal.SizeOf(*structInput));
        }

        public static void PressKey(int vkCode)
        {
            PressKey((Win32API.VKCodes)vkCode);
        }

        #endregion

        #endregion

        #region private methods



        #endregion
    }
}