/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestGUIObject.cs
*
* Description: This class defines the methods for HTML GUI testing.
*              It will calculate the screen position for GUI object. 
*
* History: 2007/09/04 wan,yu Init version
*          2008/01/10 wan,yu update, move the calculate logic of center point
*                            from GetCenterPoint to  GetRectOnScreen
*          2008/01/14 wan,yu update, modify class name to HTMLTestGUIObject.
*          2008/01/15 wan,yu update, move GetRectOnScreen from creator to
*                                    Browser proeprty.   
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
using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestGUIObject : HTMLTestObject, IVisible
    {

        #region fields

        //the rectangle on screen of the object.
        protected Rectangle _rect;

        //the center point of the object, this is very useful for GUI testing.
        //lot's of our actions we need move the mouse to the center point.
        protected Point _centerPoint;

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

        //when set the html browser, we can start to calculate the position
        public override HTMLTestBrowser Browser
        {
            set
            {
                this._browser = value;
                GetRectOnScreen();
            }
            get
            {
                return this._browser;
            }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestGUIObject()
            : base()
        {
            //when init, get the position information.
            //GetRectOnScreen();
        }

        public HTMLTestGUIObject(IHTMLElement element)
            : base(element)
        {

            //GetRectOnScreen();
        }

        ~HTMLTestGUIObject()
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
                    //_actionFinished.Close();
                    //_actionFinished = null;
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
            return _centerPoint;
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

            //if width or height is 0, error.
            if (width <= 0 || height <= 0)
            {
                throw new CannotGetObjectPositionException("width and height of object can not be 0.");
            }

            //find parent object, calculate 
            //the offsetTop/offsetLeft... is the distance between current object and it's parent object.
            //so we need a loop to get the actual position on the screen.
            IHTMLElement parent = _sourceElement.offsetParent;
            while (parent != null)
            {
                top += parent.offsetTop;
                left += parent.offsetLeft;
                parent = parent.offsetParent;
            }

            //get the browser information, get the real position on screen.
            top += _browser.ClientTop;
            left += _browser.ClientLeft;

            top -= _browser.ScrollTop;
            left -= _browser.ScrollLeft;

            // we will calculate the center point of this object.
            CalCenterPoint(left, top, width, height);

            this._rect = new Rectangle(left, top, width, height);

            return _rect;
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
                MouseOp.MoveTo(_centerPoint);

                //after move mouse to the control, wait for 0.2s, make it looks like human action.
                Thread.Sleep(200 * 1);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform Hover action:" + ex.Message);
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
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotHighlightObjectException("Can not highlight the object: " + ex.Message);
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

            int currentWidth = _browser.ClientLeft + _browser.ClientWidth;
            int currentHeight = _browser.ClientTop + _browser.ClientHeight;

            //if the object is not visible, then move the scrollbar.
            if (right > currentWidth || buttom > currentHeight)
            {
                this._sourceElement.scrollIntoView(toTop);

                Thread.Sleep(500 * 1);

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
                    left -= _browser.ClientLeft;
                    top -= _browser.ClientTop;
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

                // the red rect last for 1 seconds.
                Thread.Sleep(1000 * 1);

                //refresh the window
                Win32API.InvalidateRect(handle, IntPtr.Zero, 1);
                Win32API.UpdateWindow(handle);
                Win32API.RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, Win32API.RDW_FRAME | Win32API.RDW_INVALIDATE | Win32API.RDW_UPDATENOW | Win32API.RDW_ALLCHILDREN);

                _actionFinished.Set();
            }
            catch (Exception ex)
            {
                throw new CannotHighlightObjectException("Can not high light object: " + ex.Message);
            }
        }

        /* CalCenterPoint(int top, int left, int width, int height)
         * get the center point of the control
         */
        protected virtual Point CalCenterPoint(int left, int top, int width, int height)
        {
            _centerPoint.X = left + width / 2;
            _centerPoint.Y = top + height / 2;

            return _centerPoint;
        }

        #endregion

        #endregion

    }
}
