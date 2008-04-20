/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ScreenWords.cs
*
* Description: Capture words from screen.
*
* History: 2008/03/10 wan,yu Init version.
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Helper
{
    public sealed class ScreenWords
    {
        #region nhw32.dll function

        [DllImport(@"E:\program\cs\AutoTester\Source\Helper\nhw32.dll", CallingConvention = CallingConvention.Winapi)]
        public extern static uint BL_SetFlag32(uint nFlag, IntPtr hNotifyWnd, int MouseX, int MouseY);

        [DllImport(@"E:\program\cs\AutoTester\Source\Helper\nhw32.dll", CallingConvention = CallingConvention.Winapi)]
        public extern static uint BL_GetText32([MarshalAs(UnmanagedType.LPStr)]StringBuilder lpszCurWord, int nBufferSize, ref Rectangle lpWordRect);

        [DllImport(@"E:\program\cs\AutoTester\Source\Helper\nhw32.dll")]
        public extern static bool SetNHW32();

        [DllImport(@"E:\program\cs\AutoTester\Source\Helper\nhw32.dll")]
        public extern static bool ResetNHW32();

        #endregion

        #region fields

        private static String _words;

        private static int _x;
        private static int _y;

        private const int _bufMaxLength = 1024;
        private static StringBuilder _sbBuf = new StringBuilder(_bufMaxLength);
        private static Rectangle _rect = new Rectangle(0, 0, 0, 0);

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        static ScreenWords()
        {
            if (!SetNHW32())
            {
                throw new Exception("Can not Init screen words capture engine.");
            }

            //_eventFrm.OnNewWords += new EventFrm._getWordsDelegate(NewWords);
            //_eventFrm.Show();

        }

        private ScreenWords()
        {

        }

        #endregion

        #region public methods

        public static string GetWords()
        {
            return GetWords(Cursor.Position.X, Cursor.Position.Y);
        }

        public static string GetWords(int x, int y)
        {
            if (x >= 0 && y >= 0)
            {
                _sbBuf.Remove(0, _sbBuf.Length);

                BL_SetFlag32(1000, IntPtr.Zero, x, y);

                Thread.Sleep(2000);

                BL_SetFlag32(1002, IntPtr.Zero, 0, 0);

                BL_GetText32(_sbBuf, _bufMaxLength, ref _rect);

                return _sbBuf.ToString();

            }
            else
            {
                throw new Exception("Width and height must > 0.");
            }
        }


        #endregion

        #region private methods

        private static string WaitWords(int x, int y)
        {
            for (int times = 500; times <= 2000; times += 500)
            {
                BL_SetFlag32(1000, IntPtr.Zero, x, y);

                Thread.Sleep(times);

                BL_SetFlag32(1002, IntPtr.Zero, 0, 0);

                BL_GetText32(_sbBuf, _bufMaxLength, ref _rect);

                if (_sbBuf.Length > 0)
                {
                    return _sbBuf.ToString();
                }
            }

            return null;

        }

        #endregion

        #endregion

    }
}
