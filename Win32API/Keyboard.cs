using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace Shrinerain.AutoTester.Win32
{
    public sealed class KeyboardOp
    {
        #region ctor

        //Not allow to create instance
        private KeyboardOp()
        {

        }

        #endregion


        #region public methods
        public static void SendChars(string str)
        {
            Win32API.INPUT[] structInput = new Win32API.INPUT[1];

            for (int i = 0; i < str.Length; i++)
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

            //SendKeys.Send(str);
            //SendKeys.Flush();
        }
        #endregion

        #region private methods
        //  private void SendChar
        #endregion
    }
}