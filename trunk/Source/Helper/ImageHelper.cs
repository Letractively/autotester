/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ImageHelper.cs
*
* Description: This class provide functions to handle image.
*
* History: 2008/01/23 wan,yu Init version.
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Helper
{
    public sealed class ImageHelper
    {

        #region fields

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        private ImageHelper()
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
                throw new Exception("Can not save control print: " + ex.Message);
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
                throw new Exception("Can not save screen area: " + ex.Message);
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
                throw new Exception("Can not save screen print file: " + fileName + "," + ex.Message);
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

                // get te hDC of the target window
                IntPtr hdcSrc = Win32API.GetWindowDC(handle);
                // get the size
                Win32API.Rect windowRect = new Win32API.Rect();
                Win32API.GetWindowRect(handle, ref windowRect);
                int width = windowRect.right - windowRect.left;
                int height = windowRect.bottom - windowRect.top;
                // create a device context we can copy to
                IntPtr hdcDest = Win32API.CreateCompatibleDC(hdcSrc);
                // create a bitmap we can copy it to,
                // using GetDeviceCaps to get the width/height
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

                Graphics g = Graphics.FromImage(rectImg);

                g.CopyFromScreen(left, top, 0, 0, controlRect.Size);

                g.Dispose();

                return rectImg;
            }
            catch (Exception ex)
            {
                throw new Exception("Can not capture rect: " + ex.Message);
            }
        }

        public static Image CaptureScreenArea(Rectangle rect)
        {
            return CaptureScreenArea(rect.Left, rect.Top, rect.Width, rect.Height);
        }


        /* bool CompareImages(Image sourceImg, Image desImg)
         * compare two images pixel by pixel, return true if they are the same.
         */
        public static bool CompareImages(Image sourceImg, Image desImg)
        {
            return false;
        }

        #endregion

        #region private methods

        #endregion

        #endregion

    }
}
