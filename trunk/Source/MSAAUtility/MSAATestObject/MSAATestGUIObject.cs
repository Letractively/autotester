/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MSAATestGUIObject.cs
*
* Description: This class define the GUI test object for MSAA.
*
* History: 2008/03/19 wan,yu init version.
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestGUIObject : MSAATestObject, IVisible
    {

        #region fields

        protected Rectangle _rect;
        protected Point _centerPoint;

        protected bool _isVisible = true;
        protected bool _isEnable = true;
        protected bool _isReadonly = false;

        //sync event to ensure action is finished before next step.
        protected static AutoResetEvent _actionFinished = new AutoResetEvent(true);

        //if set the flag to ture, we will not control the actual mouse and keyboard, just send windows message.
        //then we will not see the mouse move.
        protected bool _sendMsgOnly = false;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public MSAATestGUIObject(IAccessible iAcc)
            : this(iAcc, 0)
        {
        }

        public MSAATestGUIObject(IAccessible parentAcc, int childID)
            : base(parentAcc, childID)
        {
            this._isEnable = IsEnable();
            this._isVisible = IsVisible();
            this._isReadonly = IsReadonly();
            GetRectOnScreen();
        }

        #endregion

        #region public methods

        #region IVisible Members

        public virtual Point GetCenterPoint()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual Rectangle GetRectOnScreen()
        {
            if (this._iAcc != null)
            {
                int left, top, width, height;
                this._iAcc.accLocation(out left, out top, out width, out height, _selfID);

                if (width < 1 || height < 1)
                {
                    throw new CannotGetObjectPositionException("Can not get width and height information.");
                }
                else
                {
                    CalCenterPoint(left, top, width, height);

                    return new Rectangle(left, top, width, height);
                }
            }
            else
            {
                throw new CannotGetObjectPositionException("Can not get rect on screen of screen.");
            }
        }

        public virtual Bitmap GetControlPrint()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void Hover()
        {
            MouseOp.MoveTo(_centerPoint);
        }

        public virtual bool IsVisible()
        {
            string state = GetState(this._iAcc, Convert.ToInt32(this._selfID));

            if (String.IsNullOrEmpty(state))
            {
                return true;
            }
            else
            {
                return state.IndexOf("invisible") < 0;
            }
        }

        public virtual bool IsEnable()
        {
            string state = GetState(this._iAcc, Convert.ToInt32(this._selfID));

            if (String.IsNullOrEmpty(state))
            {
                return true;
            }
            else
            {
                return state.IndexOf("unavailable") < 0;
            }
        }

        public virtual bool IsReadonly()
        {
            string state = GetState(this._iAcc, Convert.ToInt32(this._selfID));

            return true;
        }

        public virtual void HighLight()
        {

        }

        #endregion

        #endregion

        #region private methods

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
