/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLGuiTestObject.cs
*
* Description: This class defines the methods for HTML GUI testing.
*              It will calculate the screen position for GUI object. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

using mshtml;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLGuiTestObject : HTMLTestObject, IVisible
    {

        #region fields

        //the rectangle on screen of the object.
        protected Rectangle _rect;

        //sync event to ensure action is finished before next step.
        protected static AutoResetEvent _actionFinished = new AutoResetEvent(true);


        #endregion

        #region properties

        public Rectangle Rect
        {
            get { return _rect; }
            set
            {
                if (value != null)
                {
                    _rect = value;
                }
            }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLGuiTestObject()
            : base()
        {
            GetRectOnScreen();
        }

        public HTMLGuiTestObject(IHTMLElement element)
            : base(element)
        {
            //when init, get the position information.
            GetRectOnScreen();
        }

        ~HTMLGuiTestObject()
        {
            Dispose();
        }

        #endregion

        #region public methods

        /* void Dispose()
         * When GC, close AutoResetEvent
         */
        public override void Dispose()
        {
            try
            {
                base.Dispose();
                if (_actionFinished != null)
                {
                    _actionFinished.Close();
                    _actionFinished = null;
                }
                GC.SuppressFinalize(this);
            }
            catch
            {

            }


        }

        /* Point GetCenterPoint()
         * Get the center point of the object.
         * Some actions like click, we need to find the center point, and move mouse to the point.
         */
        public virtual Point GetCenterPoint()
        {
            Point tmp = new Point();

            tmp.X = Rect.Left + Rect.Width / 2;
            tmp.Y = Rect.Top + Rect.Height / 2;

            return tmp;
        }


        /* Rectangle GetRectOnScreen()
         * Get the rectangle on screen of the object.
         * we use HTML dom to calculate the rect.
         */
        public virtual Rectangle GetRectOnScreen()
        {
            // get it's position offset to it's parent object.
            int top = _sourceElement.offsetTop;
            int left = _sourceElement.offsetLeft;
            int width = _sourceElement.offsetWidth;
            int height = _sourceElement.offsetHeight;

            //find parent object, calculate 
            IHTMLElement parent = _sourceElement.offsetParent;
            while (parent != null)
            {
                top += parent.offsetTop;
                left += parent.offsetLeft;
                parent = parent.offsetParent;
            }

            //get the browser information, get the real position on screen.
            top += HTMLTestBrowser.ClientTop;
            left += HTMLTestBrowser.ClientLeft;
            top -= HTMLTestBrowser.ScrollTop;
            left -= HTMLTestBrowser.ScrollLeft;

            Rect = new Rectangle(left, top, width, height);

            return Rect;
        }

        /* Bitmap GetControlPrint()
         * return the image of the object.
         */
        public virtual Bitmap GetControlPrint()
        {
            return null;
        }

        /* void Hover()
         * Move the mouse to the object.
         * This is a very important methods.
         * All the GUI actions like click, input, we need to move the mouse to the object first.
         */
        public virtual void Hover()
        {
            try
            {
                //if the object is not visible, then move it.
                ScrollIntoView(false);

                //get the center point of the object, and move mouse to it.
                Point tmp = this.GetCenterPoint();
                MouseOp.MoveTo(tmp.X, tmp.Y);
            }
            catch
            {
                throw new CanNotPerformActionException("Can not perform Hover action.");
            }
        }

        /*  void HighLight()
         *  Highlight the object, we will see a red rect around the object.
         */
        public override void HighLight()
        {
            try
            {
                _actionFinished.WaitOne();

                ThreadPool.QueueUserWorkItem(HighLightRectCallback, null);

            }
            catch (CanNotHighlightObjectException)
            {
                throw;
            }

        }

        #region gui actions

        #endregion

        #endregion



        #region private methods

        /* void ScrollIntoView(bool toTop)
         * if the object is out of page view, scroll it to make it is visible.
         * Like some very long page, we can not see everything, we need to move the scrollbar to see
         * the bottom object.
         */
        protected virtual void ScrollIntoView(bool toTop)
        {
            //first, get the current position.
            int right = this._rect.X + this._rect.Width;
            int buttom = this._rect.Y + this._rect.Height;

            int currentWidth = HTMLTestBrowser.ClientLeft + HTMLTestBrowser.ClientWidth;
            int currentHeight = HTMLTestBrowser.ClientTop + HTMLTestBrowser.ClientHeight;

            //if the object is not visible, then move the scrollbar.
            if (right > currentWidth || buttom > currentHeight)
            {
                this._sourceElement.scrollIntoView(toTop);

                //re-calculate the position, because we had move it.
                this.Rect = GetRectOnScreen();
            }

        }

        /* void HighLightRectCallback(Object obj)
         * callback function to highlight the object.
         */
        protected virtual void HighLightRectCallback(Object obj)
        {
            HighLightRect(false);
        }

        /* void HighLightRect(bool isWindowsControl)
         * Highlight the object.
         * is it is not a windows control, we need to minus the browser position.
         */
        protected virtual void HighLightRect(bool isWindowsControl)
        {
            try
            {

                int left = this._rect.Left;
                int top = this._rect.Top;
                int width = this._rect.Width;
                int height = this._rect.Height;

                IntPtr handle = Win32API.WindowFromPoint(left + 1, top + 1);

                //if the control is not a windows standard control,we need to minus the browser top and left.
                //because if it is NOT a windows control, then we consider it is a HTML control, when we get the handle,
                //the handle is belonged to "Internet Explorer_Server", it not include the menu bar...
                //so we need to minus the menu bar height and top to get the actual position.

                if (!isWindowsControl)
                {
                    left -= HTMLTestBrowser.ClientLeft;
                    top -= HTMLTestBrowser.ClientTop;
                }

                IntPtr hDC = Win32API.GetWindowDC(handle);
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    using (Graphics g = Graphics.FromHdc(hDC))
                    {
                        g.DrawRectangle(pen, left, top, width, height);
                    }
                }
                Win32API.ReleaseDC(handle, hDC);

                Thread.Sleep(500 * 1); // the red rect last for 0.2 seconds.

                Win32API.InvalidateRect(handle, IntPtr.Zero, 1 /* TRUE */);
                Win32API.UpdateWindow(handle);
                Win32API.RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, Win32API.RDW_FRAME | Win32API.RDW_INVALIDATE | Win32API.RDW_UPDATENOW | Win32API.RDW_ALLCHILDREN);

                _actionFinished.Set();
            }
            catch
            {
                throw new CanNotHighlightObjectException();
            }
        }

        #endregion

        #endregion

    }
}
