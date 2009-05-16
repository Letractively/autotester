/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestLink.cs
*
* Description: This class defines the actions provide by Link.
*              The important actions is "Click". 
*
* History: 2007/09/04 wan,yu Init version
*          2008/01/12 wan,yu update, remove HTMLTestObject[] GetLinkChildren()
* 
*********************************************************************/

using System;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestLink : HTMLTestGUIObject, IClickable, IText, IStatus
    {

        #region fields

        //the url of this link
        protected string _href;

        //the text of the link if it is a text link.
        protected string _linkText;

        //the image of the link if it is a image link.
        protected IHTMLImgElement _linkImgElement;

        //for link, we can have child image.
        protected object[] _childeren;

        // the HTML element of link.
        protected HTMLAnchorElement _acnchorElement;

        #endregion

        #region properties

        public string LinkText
        {
            get { return _linkText; }
        }

        public string Href
        {
            get { return _href; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestLink(IHTMLElement element)
            : base(element)
        {

            this._type = HTMLTestObjectType.Link;

            try
            {
                _acnchorElement = (HTMLAnchorElement)element;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not convert to HTMLAnchorElement: " + ex.ToString());
            }

            try
            {
                //get the link text
                _linkText = _acnchorElement.innerText;
            }
            catch
            {
                _linkText = "";
            }

            try
            {
                // if the text is null, it maybe a image link, try to get the image.
                if (String.IsNullOrEmpty(_linkText))
                {
                    _linkImgElement = (IHTMLImgElement)_acnchorElement.firstChild;
                }
            }
            catch
            {
                //throw new CannotBuildObjectException("Can not get the image of link: " + ex.ToString());
            }

            try
            {
                //get the url of the link.
                _href = _acnchorElement.href;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get href of link: " + ex.ToString());
            }
        }

        #endregion

        #region public methods

        /* void Click()
         * Click on link
         */
        public virtual void Click()
        {
            try
            {
                if (!IsReady() || !_isEnable || !_isVisible)
                {
                    throw new CannotPerformActionException("Link is not enabled.");
                }

                _actionFinished.WaitOne();

                Hover();
                if (_sendMsgOnly)
                {
                    _acnchorElement.click();
                }
                else
                {
                    MouseOp.Click();
                }

                if (_isDelayAfterAction)
                {
                    System.Threading.Thread.Sleep(_delayTime * 1000);
                }

                _actionFinished.Set();

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform click action of link: " + ex.ToString());
            }
        }

        public virtual void DoubleClick()
        {

        }

        public virtual void RightClick()
        {

        }

        public virtual void MiddleClick()
        {

        }

        public virtual void Focus()
        {
            try
            {
                if (this._acnchorElement != null)
                {
                    this._acnchorElement.focus();
                }
                else
                {
                    throw new CannotPerformActionException("Can not focus: Element can not be null.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not focus: " + ex.ToString());
            }

        }

        public virtual string GetAction()
        {
            return "Click";
        }

        public virtual void DoAction(object para)
        {
            Click();
        }

        #region IText Members

        public string GetText()
        {
            return this._linkText;
        }

        public override string GetLabel()
        {
            return GetText();
        }

        public string GetFontFamily()
        {
            throw new NotImplementedException();
        }

        public string GetFontSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontColor()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods

        /* bool IsImage()
         * Return true if the link is an image link.
         */
        protected virtual bool IsImage()
        {
            return String.IsNullOrEmpty(_linkText);
        }

        #endregion

        #endregion

    }
}
