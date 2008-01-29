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
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Shrinerain.AutoTester.Win32
{
    public sealed class KeyboardOp
    {
        #region ctor

        //can Not to create instance
        private KeyboardOp()
        {

        }

        #endregion

        #region public methods

        /* void SendChars(string str)
         * send normal characters to system. 
         * except special keys like tab, ctrl etc.
         */
        public static void SendChars(string str)
        {

            if (String.IsNullOrEmpty(str))
            {
                return;
            }

            Win32API.INPUT[] structInput = new Win32API.INPUT[1];

            bool specialKeys = false;
            StringBuilder keysBuf = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {


                if (str[i] == '{')
                {
                    if (i == 0 || (i > 0 && str[i - 1] != '\\')) //we didn't escape it.
                    {
                        specialKeys = true;
                        keysBuf = new StringBuilder(10);
                    }
                }

                if (specialKeys)
                {
                    keysBuf.Append(str[i]);

                    if (str[i] == '}')
                    {
                        specialKeys = false;
                        SendKey(keysBuf.ToString());
                    }

                }
                else
                {
                    structInput[0].type = 1; //keyboard
                    structInput[0].ki.wVk = 0;
                    structInput[0].ki.time = 0;
                    structInput[0].ki.dwFlags = 4; //unicode
                    structInput[0].ki.dwExtraInfo = Win32API.GetMessageExtraInfo();

                    //key down
                    structInput[0].ki.wScan = str[i];
                    Win32API.SendInput((uint)structInput.Length, structInput, Marshal.SizeOf(structInput[0]));

                    //key up
                    structInput[0].ki.dwFlags = 2;
                    Win32API.SendInput((uint)structInput.Length, structInput, Marshal.SizeOf(structInput[0]));
                }

                //sleep for 0.05 second, make it looks like human actions
                System.Threading.Thread.Sleep(50 * 1);
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

        #endregion

        #region private methods

        #endregion
    }
}