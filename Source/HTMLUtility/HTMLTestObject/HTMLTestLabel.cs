/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestLabel.cs
*
* Description: This class defines the actions provide by label.
*              Actually, there isn't a "Label" control in html, I treat              
*              <label>text</label>,<span>text</span> and <td>text</td> as the Label.
*               
* History: 2008/01/21 wan,yu Init version.       
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestLabel : HTMLTestGUIObject, IShowInfo
    {

        #region fields

        protected IHTMLLabelElement _labelElement;
        protected IHTMLSpanElement _spanElement;
        protected IHTMLTableCell _cellElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestLabel(IHTMLElement element)
        {
            this._type = HTMLTestObjectType.Label;

            try
            {
                if (element.tagName == "LABEL")
                {
                    _labelElement = (IHTMLLabelElement)element;
                }
                else if (element.tagName == "SPAN")
                {
                    _spanElement = (IHTMLSpanElement)element;
                }
                else if (element.tagName == "TD")
                {
                    _cellElement = (IHTMLTableCell)element;
                }
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not conver to Label element: " + ex.Message);
            }

        }

        #endregion

        #region public methods

        #region IShowInfo Members

        public string GetText()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontStyle()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontFamily()
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
