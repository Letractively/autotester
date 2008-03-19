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

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public MSAATestGUIObject(IAccessible iAcc)
            : base(iAcc)
        {
            GetRectOnScreen();
        }

        public MSAATestGUIObject(IAccessible parentAcc, int childID)
            : base(parentAcc, childID)
        {
            GetRectOnScreen();
        }

        #endregion

        #region public methods

        #region IVisible Members

        public Point GetCenterPoint()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Rectangle GetRectOnScreen()
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

        public Bitmap GetControlPrint()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Hover()
        {
            MouseOp.MoveTo(_centerPoint);
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
