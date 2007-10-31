using System;
using System.Collections.Generic;
using System.Text;
using mshtml;

using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestLink : HTMLGuiTestObject, IClickable, IContainer
    {

        #region fields

        protected string _href;
        protected string _linkText;
        protected IHTMLImgElement _linkImgElement;

        protected object[] _childeren;

        protected HTMLLinkElement _linkElement;
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
                // _linkElement = (HTMLLinkElement)element;
            }
            catch (Exception e)
            {
                throw new CanNotBuildObjectException("Can not build test link object: " + e.Message);
            }

            try
            {
                _linkText = _acnchorElement.innerText; //_linkElement.innerText;
            }
            catch
            {
                _linkText = "";
            }
            try
            {
                if (String.IsNullOrEmpty(_linkText))
                {
                    _linkImgElement = (IHTMLImgElement)_linkElement.firstChild;
                }
            }
            catch
            {

            }
            try
            {
                // _childeren=_linkElement.ch
            }
            catch
            {

            }
            try
            {
                _href = _acnchorElement.href; //_linkElement.href;
            }
            catch
            {
                throw new CanNotBuildObjectException("Can not get href of link.");
            }

        }

        #endregion

        #region public methods

        public virtual void Click()
        {
            try
            {
                _actionFinished.WaitOne();

                Hover();
                MouseOp.Click();

            }
            catch
            {
                throw new CanNotPerformActionException("Can not perform click action of link.");
            }
            finally
            {
                _actionFinished.Set();
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

        public virtual void PerformDefaultAction()
        {
            Click();
        }

        public virtual Object[] GetChildren()
        {
            if (_childeren == null)
            {

            }

            return _childeren;
        }

        #endregion

        #region private methods

        protected virtual HTMLTestObject[] GetLinkChildren()
        {
            return null;
        }

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
