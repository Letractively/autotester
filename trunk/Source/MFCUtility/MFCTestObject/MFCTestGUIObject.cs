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

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.MFCUtility
{
    public class MFCTestGUIObject : MFCTestObject, IVisible
    {

        #region fields


        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods
        #region IVisible Members

        public Point GetCenterPoint()
        {
            throw new NotImplementedException();
        }

        public Rectangle GetRectOnScreen()
        {
            throw new NotImplementedException();
        }

        public Bitmap GetControlPrint()
        {
            throw new NotImplementedException();
        }

        public void Hover()
        {
            throw new NotImplementedException();
        }

        #endregion
        #endregion

        #region private methods


        #endregion

        #endregion


    }
}
