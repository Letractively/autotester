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

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestTable : HTMLGuiTestObject, IClickable, IShowInfo, ISelectable, IContainer
    {


        #region fields


        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #endregion

        #region private methods


        #endregion

        #endregion


        #region IClickable Members

        public void Click()
        {
            throw new NotImplementedException();
        }

        public void DoubleClick()
        {
            throw new NotImplementedException();
        }

        public void RightClick()
        {
            throw new NotImplementedException();
        }

        public void MiddleClick()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new NotImplementedException();
        }

        public object GetDefaultAction()
        {
            throw new NotImplementedException();
        }

        public void PerformDefaultAction(object parameter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IShowInfo Members

        public string GetText()
        {
            throw new NotImplementedException();
        }

        public string GetFontStyle()
        {
            throw new NotImplementedException();
        }

        public string GetFontFamily()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISelectable Members

        public void Select(string str)
        {
            throw new NotImplementedException();
        }

        public void SelectByIndex(int index)
        {
            throw new NotImplementedException();
        }

        public string[] GetAllValues()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IContainer Members

        public object[] GetChildren()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
