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
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestLink : HTMLGuiTestObject, IClickable, IContainer
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

        public IHTMLImgElement LinkImg
        {
            get { return _linkImgElement; }
        }

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
            try
            {
                _acnchorElement = (HTMLAnchorElement)element;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not build test link object: " + e.Message);
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
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get the image of link: " + e.Message);
            }

            try
            {
                //get the url of the link.
                _href = _acnchorElement.href;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get href of link: " + e.Message);
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
                _actionFinished.WaitOne();

                Hover();
                MouseOp.Click();

                _actionFinished.Set();

            }
            catch
            {
                throw new CannotPerformActionException("Can not perform click action of link.");
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

        }

        public virtual object GetDefaultAction()
        {
            return "Click";
        }

        public virtual void PerformDefaultAction(object para)
        {
            Click();
        }

        public virtual Object[] GetChildren()
        {
            if (_childeren != null)
            {
                return _childeren;
            }

            return null;
        }

        #endregion

        #region private methods

        /* HTMLTestObject[] GetLinkChildren()
         * Return the children object of link
         */
        protected virtual HTMLTestObject[] GetLinkChildren()
        {
            return null;
        }

        /* bool IsImage()
         * Return true if the link is an image link.
         */
        protected virtual bool IsImage()
        {
            if (String.IsNullOrEmpty(_linkText))
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        #endregion

        #endregion

    }
}
