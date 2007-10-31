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
            this.GetRect();
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
        public virtual Rectangle GetRect()
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


        #region gui actions

        #endregion

        #endregion



        #region private methods

        // if the object is out of page view, scroll it to make it visible.
        protected void ScrollIntoView(bool toTop)
        {
            int right = this._rect.X + this._rect.Width;
            int buttom = this._rect.Y + this._rect.Height;

            int currentWidth = HTMLTestBrowser.ClientLeft + HTMLTestBrowser.ClientWidth;
            int currentHeight = HTMLTestBrowser.ClientTop + HTMLTestBrowser.ClientHeight;

            if (right > currentWidth || buttom > currentHeight)
            {
                this._sourceElement.scrollIntoView(toTop);
                this.Rect = GetRect();
            }

        }

        #endregion

        #endregion

    }
}
