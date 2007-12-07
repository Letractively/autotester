/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: 
*
* Description:
*
* History:
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestCheckBox : HTMLGuiTestObject, ICheckable
    {

        #region fields

        protected mshtml.IHTMLInputElement _checkBoxElement;


        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestCheckBox(IHTMLElement element)
            : base(element)
        {
            try
            {
                this._checkBoxElement = (IHTMLInputElement)element;
            }
            catch (System.Exception e)
            {
                throw new CanNotBuildObjectException("Can not get IHTMLInputElement: " + e.Message);
            }

        }

        #endregion

        #region public methods

        #region ICheckable Members

        public void Check()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void UnCheck()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void IsChecked()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IClickable Members

        public void Click()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void DoubleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RightClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void MiddleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetDefaultAction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void PerformDefaultAction()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion


    }
}
