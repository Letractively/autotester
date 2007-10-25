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
            SendKeys.SendWait(str);
        }
        #endregion

        #region private methods
        //  private void SendChar
        #endregion
    }
}