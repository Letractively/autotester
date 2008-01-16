/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MFCTestGUIObject.cs
*
* Description: This is the base class of MFC GUI object.
*              Important methods include "GetRectOnScreen()" and "Hover()" 
*
* History: 2008/01/14 wan,yu Init version.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.MFCUtility
{
    public class MFCTestGUIObject : MFCTestObject, IVisible
    {

        #region fields

        //rect on the screen.
        protected Rectangle _rect;
        protected Point _centerPoint;

        //sync event to control action 
        protected static AutoResetEvent _actionFinished = new AutoResetEvent(true);

        #endregion

        #region properties

        public Rectangle Rect
        {
            get { return _rect; }
        }

        public Point CenterPoint
        {
            get { return _centerPoint; }
        }

        #endregion

        #region methods

        #region ctor

        public MFCTestGUIObject()
            : base()
        {

        }

        public MFCTestGUIObject(IntPtr handle)
            : base(handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Handle can not be 0.");
            }

            try
            {
                GetRectOnScreen();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get rect on screen: " + ex.Message);
            }

        }

        #endregion

        #region public methods
        #region IVisible Members

        public virtual Point GetCenterPoint()
        {
            return this._centerPoint;
        }

        public virtual Rectangle GetRectOnScreen()
        {
            try
            {
                if (this._handle == IntPtr.Zero)
                {
                    throw new CannotGetObjectPositionException("Handle can not be 0.");
                }

                Win32API.Rect rect = new Win32API.Rect();
                Win32API.GetWindowRect(this._handle, ref rect);

                if (rect.Width <= 0 || rect.Height <= 0)
                {
                    throw new CannotGetObjectPositionException("Can not get width/height of object.");
                }

                this._rect = new Rectangle(rect.left, rect.top, rect.Width, rect.Height);

                this._centerPoint = CalCenterPoint();

                return this._rect;

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get rect on screen: " + ex.Message);
            }

        }

        public virtual Bitmap GetControlPrint()
        {
            throw new NotImplementedException();
        }

        public virtual void Hover()
        {
            try
            {
                MouseOp.MoveTo(this._centerPoint);

                //sleep for 0.2s, make it looks like human action.
                Thread.Sleep(200 * 1);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not move mouse to this object: " + ex.Message);
            }
        }

        #endregion
        #endregion

        #region private methods

        /*  Point CalCenterPoint()
         *  Calculate the center point of this object.
         */
        protected virtual Point CalCenterPoint()
        {
            if (this._rect.Width <= 0 || this._rect.Height <= 0)
            {
                throw new CannotGetObjectPositionException("Can not get center point.");
            }

            try
            {
                Point p = new Point();

                p.X = this._rect.Left + this._rect.Width / 2;
                p.Y = this._rect.Top + this._rect.Height / 2;

                return p;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get center point: " + ex.Message);
            }
        }

        #endregion

        #endregion


    }
}
