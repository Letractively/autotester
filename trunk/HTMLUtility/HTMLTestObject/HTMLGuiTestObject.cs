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

        private Rectangle _rect;

        //sync event to ensure action is finished before next step.
        protected static AutoResetEvent _actionFinished = new AutoResetEvent(true);


        //dthread to high light a rect.
        private static Thread _highLightThread;

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

        public HTMLGuiTestObject(IHTMLElement element)
            : base(element)
        {
            this.GetRectOnScreen();
        }

        ~HTMLGuiTestObject()
        {
            Dispose();
        }

        #endregion

        #region public methods

        public override void Dispose()
        {
            if (_actionFinished != null)
            {
                _actionFinished.Close();
                _actionFinished = null;
            }
            GC.SuppressFinalize(this);

            base.Dispose();

        }

        public virtual Point GetCenterPoint()
        {
            Point tmp = new Point();

            tmp.X = Rect.Left + Rect.Width / 2;
            tmp.Y = Rect.Top + Rect.Height / 2;

            return tmp;
        }


        //get the object rect
        public virtual Rectangle GetRectOnScreen()
        {

            int top = _sourceElement.offsetTop;
            int left = _sourceElement.offsetLeft;
            int width = _sourceElement.offsetWidth;
            int height = _sourceElement.offsetHeight;

            IHTMLElement parent = _sourceElement.offsetParent;
            while (parent != null)
            {
                top += parent.offsetTop;
                left += parent.offsetLeft;
                parent = parent.offsetParent;
            }

            top += HTMLTestBrowser.ClientTop;
            left += HTMLTestBrowser.ClientLeft;
            top -= HTMLTestBrowser.ScrollTop;
            left -= HTMLTestBrowser.ScrollLeft;

            Rect = new Rectangle(left, top, width, height);

            return Rect;
        }

        public virtual Bitmap GetControlPrint()
        {
            return null;
        }

        public virtual void Hover()
        {
            try
            {
                ScrollIntoView(false);
                Point tmp = this.GetCenterPoint();
                MouseOp.MoveTo(tmp.X, tmp.Y);
            }
            catch
            {
                throw new CanNotPerformActionException("Can not perform Hover action.");
            }
        }

        public override void HightLight()
        {
            _highLightThread = new Thread(new ThreadStart(HighLightRect));
            _highLightThread.Start();
        }

        #region gui actions

        #endregion

        #endregion



        #region private methods

        // if the object is out of page view, scroll it to make it visible.
        protected virtual void ScrollIntoView(bool toTop)
        {
            int right = this._rect.X + this._rect.Width;
            int buttom = this._rect.Y + this._rect.Height;

            int currentWidth = HTMLTestBrowser.ClientLeft + HTMLTestBrowser.ClientWidth;
            int currentHeight = HTMLTestBrowser.ClientTop + HTMLTestBrowser.ClientHeight;

            if (right > currentWidth || buttom > currentHeight)
            {
                this._sourceElement.scrollIntoView(toTop);
                this.Rect = GetRectOnScreen();
            }

        }

        protected virtual void HighLightRect()
        {
            HighLightRect(false);
        }

        protected virtual void HighLightRect(bool isWindowsControl)
        {
            try
            {
                int left = this._rect.Left;
                int top = this._rect.Top;
                int width = this._rect.Width;
                int height = this._rect.Height;

                //if the control is not a windows standard control,we need to minus the browser top and left.
                if (!isWindowsControl)
                {
                    left -= HTMLTestBrowser.ClientLeft;
                    top -= HTMLTestBrowser.ClientTop;
                }


                IntPtr handle = Win32API.WindowFromPoint(left + 1, top + 1);
                IntPtr hDC = Win32API.GetWindowDC(handle);
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    using (Graphics g = Graphics.FromHdc(hDC))
                    {
                        g.DrawRectangle(pen, left, top, width, height);
                    }
                }
                Win32API.ReleaseDC(handle, hDC);

                Thread.Sleep(100 * 1); // the red rect last for 0.1 seconds.

                Win32API.InvalidateRect(handle, IntPtr.Zero, 1 /* TRUE */);
                Win32API.UpdateWindow(handle);
                Win32API.RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, Win32API.RDW_FRAME | Win32API.RDW_INVALIDATE | Win32API.RDW_UPDATENOW | Win32API.RDW_ALLCHILDREN);

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
