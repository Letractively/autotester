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
*              <label>text</label>,<span>text</span> and <font></font>
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
    public class HTMLTestLabel : HTMLTestGUIObject, IText
    {

        #region fields

        //<label>
        protected IHTMLLabelElement _labelElement;
        //<span>
        protected IHTMLSpanElement _spanElement;
        //<font>
        protected IHTMLFontElement _fontElement;

        protected string _styleStr;
        #endregion

        #region properties

        #endregion

        #region methods

        #region ctor

        public HTMLTestLabel(IHTMLElement element)
            : base(element, null)
        {
        }

        public HTMLTestLabel(IHTMLElement element, HTMLTestBrowser browser)
            : base(element, browser)
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
                else if (element.tagName == "FONT")
                {
                    _fontElement = (IHTMLFontElement)element;
                }

            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not convert to Label element: " + ex.ToString());
            }

            //get text of the label.
            if (!HTMLTestObject.TryGetProperty(element, "innerText", out _label))
            {
                throw new CannotBuildObjectException("Can not get text of label.");
            }
        }

        #endregion

        #region public methods

        #region IText Members

        public string GetText()
        {
            return _label;
        }

        public string GetFontFamily()
        {
            try
            {
                string fontFamily = GetStyleProperty("font-family");

                if (String.IsNullOrEmpty(fontFamily) && _fontElement != null)
                {
                    if (!HTMLTestObject.TryGetProperty(this._sourceElement, "face", out fontFamily))
                    {
                        throw new PropertyNotFoundException("Can not get font family");
                    }
                }

                return fontFamily;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get font family: " + ex.ToString());
            }
        }

        public string GetFontSize()
        {
            try
            {
                string fontSize = GetStyleProperty("font-size");
                if (String.IsNullOrEmpty(fontSize) && _fontElement != null)
                {
                    if (!HTMLTestObject.TryGetProperty(this._sourceElement, "size", out fontSize))
                    {
                        throw new PropertyNotFoundException("Can not get font size");
                    }
                }

                return fontSize;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get font size: " + ex.ToString());
            }
        }

        public string GetFontColor()
        {
            try
            {
                string fontColor = GetStyleProperty("font-color");

                if (String.IsNullOrEmpty(fontColor) && _fontElement != null)
                {
                    if (!HTMLTestObject.TryGetProperty(this._sourceElement, "color", out fontColor))
                    {
                        throw new PropertyNotFoundException("Can not get font color");
                    }
                }

                return fontColor;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get font color: " + ex.ToString());
            }
        }

        #endregion

        #endregion

        #region private methods

        /* String GetStyle()
         * return the "style" property.
         */
        protected virtual String GetStyle()
        {
            if (String.IsNullOrEmpty(_styleStr))
            {
                if (HTMLTestObject.TryGetProperty(this._sourceElement, "style", out _styleStr))
                {
                    if (String.Compare(_styleStr, "System.__ComObject", true) != 0)
                    {
                        return _styleStr;
                    }
                }

                return null;
            }
            else
            {
                return _styleStr;
            }
        }

        protected virtual String GetStyleProperty(string property)
        {
            if (String.IsNullOrEmpty(property))
            {
                throw new PropertyNotFoundException("Property name can not be empty.");
            }

            string style = GetStyle();

            if (!String.IsNullOrEmpty(style))
            {
                int startPos = style.IndexOf(property, StringComparison.CurrentCultureIgnoreCase);
                if (startPos >= 0)
                {
                    int endPos = style.IndexOf(';', startPos);
                    if (endPos <= 0)
                    {
                        endPos = style.Length;
                    }
                    if (endPos > startPos)
                    {
                        return style.Substring(startPos, endPos - startPos).Split(':')[1];
                    }
                }
            }

            return null;
        }

        #endregion

        #endregion
    }
}
