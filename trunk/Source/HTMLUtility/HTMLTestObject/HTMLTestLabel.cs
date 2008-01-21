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
*              <label>text</label>,<span>text</span> and <td>text</td> 
*              <DIV>text</DIV> as the Label.
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

        //<label>
        protected IHTMLLabelElement _labelElement;

        //<span>
        protected IHTMLSpanElement _spanElement;

        //<td>
        protected IHTMLTableCell _cellElement;

        //<div>
        protected IHTMLDivElement _divElement;

        protected string _text;

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
                else if (element.tagName == "DIV")
                {
                    _divElement = (IHTMLDivElement)element;
                }

            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not conver to Label element: " + ex.Message);
            }

            //get text of the label.
            if (!HTMLTestObject.TryGetValueByProperty(element, "innerText", out _text))
            {
                throw new CannotBuildObjectException("Can not get text of label.");
            }
        }

        #endregion

        #region public methods

        #region IShowInfo Members

        public string GetText()
        {
            return _text;
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
