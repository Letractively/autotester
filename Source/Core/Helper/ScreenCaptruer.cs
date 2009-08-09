/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ScreenCaptruer.cs
*
* Description: This class provide functions to get screen/window print.
*
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core.Helper
{
    public sealed class ScreenCaptruer
    {

        #region fields
        private const int Hight_Border = 2;
        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        private ScreenCaptruer()
        {
        }

        #endregion

        #region public methods

        /* void SaveControlPrint(IntPtr handle, string fileName)
         * save print of control to a jpg file.
         */
        public static void SaveControlPrint(IntPtr handle, string fileName)
        {
            try
            {
                Image img = CaptureWindow(handle);
                img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw new Exception("Can not save control print: " + ex.ToString());
            }
        }

        /* void SaveScreenArea(int left, int top, int width, int height, string fileName)
         * save the expected area to a jpg file.
         */
        public static void SaveScreenArea(int left, int top, int width, int height, string fileName)
        {
            try
            {
                Image img = CaptureScreenArea(left, top, width, height);
                img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw new Exception("Can not save screen area: " + ex.ToString());
            }
        }

        public static void SaveScreenArea(Rectangle rect, string fileName)
        {
            SaveScreenArea(rect.Left, rect.Top, rect.Width, rect.Height, fileName);
        }

        /* void SaveScreenPrint()
         * save screen print to a jpg file.
         */
        public static void SaveScreenPrint(string fileName)
        {
            try
            {
                Image img = CaptureScreen();
                img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw new Exception("Can not save screen print file: " + fileName + "," + ex.ToString());
            }
        }

        /* Image CaptureScreen()
         * return an image of screen.
         */
        public static Image CaptureScreen()
        {
            return CaptureWindow(Win32API.GetDesktopWindow());
        }

        /* Image CaptureWindow(IntPtr handle)
         * return an image of expected handle.
         */
        public static Image CaptureWindow(IntPtr handle)
        {
            try
            {
                if (handle == IntPtr.Zero)
                {
                    throw new Exception("Handle can not be 0.");
                }

                // get the size
                Win32API.Rect windowRect = new Win32API.Rect();
                Win32API.GetWindowRect(handle, ref windowRect);
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;

                if (width > 0 && height > 0)
                {
                    bool printed = false;

                    //firstly, try PrintWindow to get the image of the window.
                    Bitmap bm = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(bm))
                    {
                        System.IntPtr bmDC = g.GetHdc();
                        printed = Win32API.PrintWindow(handle, bmDC, 0);
                        g.ReleaseHdc(bmDC);
                    }

                    if (printed)
                    {
                        return bm;
                    }
                    else
                    {
                        //not printed, try other way.

                        // get te hDC of the target window
                        IntPtr hdcSrc = Win32API.GetWindowDC(handle);

                        // create a device context we can copy to
                        IntPtr hdcDest = Win32API.CreateCompatibleDC(hdcSrc);
                        // create a bitmap we can copy it to,
                        IntPtr hBitmap = Win32API.CreateCompatibleBitmap(hdcSrc, width, height);
                        // select the bitmap object
                        IntPtr hOld = Win32API.SelectObject(hdcDest, hBitmap);
                        // bitblt over
                        Win32API.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, Win32API.SRCCOPY);
                        // restore selection
                        Win32API.SelectObject(hdcDest, hOld);
                        // clean up 
                        Win32API.DeleteDC(hdcDest);
                        Win32API.ReleaseDC(handle, hdcSrc);

                        // get a .NET image object for it
                        Image img = Image.FromHbitmap(hBitmap);
                        // free up the Bitmap object
                        Win32API.DeleteObject(hBitmap);

                        return img;
                    }
                }
                else
                {
                    throw new Exception("Can not get size infomation of the window.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        /* Image CaptureScreenRect(int left, int top, int width, int height)
         * return an Image object of expected rect.
         */
        public static Image CaptureScreenArea(int left, int top, int width, int height)
        {
            try
            {
                if (width < 1 || height < 1)
                {
                    throw new Exception("Width and Height must larger than 0.");
                }

                Rectangle controlRect = new Rectangle(left, top, width, height);
                Image rectImg = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(rectImg))
                {
                    g.CopyFromScreen(left, top, 0, 0, controlRect.Size);
                }

                return rectImg;
            }
            catch (Exception ex)
            {
                throw new Exception("Can not capture rect: " + ex.ToString());
            }
        }

        public static Image CaptureScreenArea(Rectangle rect)
        {
            return CaptureScreenArea(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static void HighlightWindowRect(IntPtr handle, Rectangle rect, int mseconds)
        {
            int left = rect.Left;
            int top = rect.Top;
            int width = rect.Width;
            int height = rect.Height;
            HighlightWindowRect(handle, left, top, width, height, mseconds);
        }

        public static void HighlightScreenRect(Rectangle rect, int mseconds)
        {
            int left = rect.Left;
            int top = rect.Top;
            int width = rect.Width;
            int height = rect.Height;
            HighlightScreenRect(left, top, width, height, mseconds);
        }

        public static void HighlightScreenRect(int left, int top, int width, int height, int mseconds)
        {
            IntPtr handle = Win32API.WindowFromPoint(left + width / 2, top + height / 2);
            Win32API.Rect windRect = new Win32API.Rect();
            Win32API.GetWindowRect(handle, ref windRect);

            left -= windRect.left;
            top -= windRect.top;
            HighlightWindowRect(handle, left, top, width, height, mseconds);
        }

        public static void HighlightWindowRect(IntPtr handle, int left, int top, int width, int height, int mseconds)
        {
            if (width > 0 && height > 0 && mseconds > 0)
            {
                try
                {
                    IntPtr hDC = Win32API.GetDC(handle);
                    using (Pen pen = new Pen(Color.Red, Hight_Border))
                    {
                        using (Graphics g = Graphics.FromHdc(hDC))
                        {
                            g.DrawRectangle(pen, left, top, width, height);
                        }
                    }
                    Win32API.ReleaseDC(handle, hDC);
                    Thread.Sleep(mseconds);

                    RestoreHighLight(left, top, width, height, handle);
                }
                catch (Exception ex)
                {
                    throw new CannotHighlightObjectException("Can not high light: " + ex.ToString());
                }
            }
        }

        public static void RestoreHighLight(Rectangle rect)
        {
            int left = rect.Left;
            int top = rect.Top;
            int width = rect.Width;
            int height = rect.Height;

            RestoreHighLight(left, top, width, height, IntPtr.Zero);
        }

        public static void RestoreHighLight(int left, int top, int width, int height, IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                handle = Win32API.WindowFromPoint(left + width / 2, top + height / 2);
            }

            Win32API.Rect rect = new Win32API.Rect();
            rect.left = left;
            rect.top = top;
            rect.right = left + width;
            rect.bottom = top + height;

            RestoreHighLight(rect, handle);
        }

        public static void RestoreHighLight(Win32API.Rect rect, IntPtr handle)
        {
            Win32API.Rect invalidRect = new Win32API.Rect();
            invalidRect.left = rect.left - Hight_Border;
            invalidRect.top = rect.top - Hight_Border;
            invalidRect.right = rect.right + Hight_Border;
            invalidRect.bottom = rect.bottom + Hight_Border;
            //refresh the window
            Win32API.InvalidateRect(handle, ref invalidRect, 1);
            //Win32API.UpdateWindow(handle);
        }

        #endregion

        #region private methods

        #endregion

        #endregion
    }
}
