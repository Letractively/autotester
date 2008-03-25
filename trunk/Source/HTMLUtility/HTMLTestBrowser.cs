/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestBrowser.cs
*
* Description: This class defines the the actions to support HTML test.
*              we use HTML DOM to get the object from Internet Explorer.
*
* History: 2007/09/04 wan,yu Init version
*          2008/01/15 wan,yu update, modify some static memebers to instance 
*
*********************************************************************/

using System;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestBrowser : TestBrowser
    {
        #region Fileds

        #endregion

        #region Properties

        #endregion

        #region Methods

        public HTMLTestBrowser()
        {
            _browserName = "Internet Explorer";
        }

        ~HTMLTestBrowser()
        {
            Dispose();
        }

        #region public methods


        #region GetObject methods

        /* IHTMLElementCollection GetAllObjects()
         * return all element of HTML DOM.
         */
        public IHTMLElementCollection GetAllObjects()
        {
            if (_HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return (IHTMLElementCollection)_HTMLDom.body.all;
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not get all objects: " + ex.Message);
            }

        }

        /* IHTMLElement GetObjectByID(string id)
         * return element by .id property.
         */
        public IHTMLElement GetObjectByID(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new PropertyNotFoundException("ID can not be null.");
            }
            if (_HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return _HTMLDom.getElementById(id);
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found test object by id:" + id + ": " + ex.Message);
            }

        }

        /* IHTMLElementCollection GetObjectsByName(string name)
         * return elements by .name property.
         */
        public IHTMLElementCollection GetObjectsByName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new PropertyNotFoundException("Name can not be null.");
            }

            if (_HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }

            try
            {
                return _HTMLDom.getElementsByName(name);
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found test object by name:" + name + ":" + ex.Message);
            }
        }

        /* IHTMLElementCollection GetObjectsByTagName(string name)
         * return elements by tag, eg: <a> will return all link.
         */
        public IHTMLElementCollection GetObjectsByTagName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new PropertyNotFoundException("Tag name can not be null.");
            }

            if (_HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }

            try
            {
                return _HTMLDom.getElementsByTagName(name);
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found test object by tag name:" + name + ":" + ex.Message);
            }
        }

        /* IHTMLElement GetObjectFromPoint(int x, int y)
         * return element at expected point.
         */
        public IHTMLElement GetObjectFromPoint(int x, int y)
        {
            try
            {
                return _HTMLDom.elementFromPoint(x, y);
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found object at point: (" + x.ToString() + "," + y.ToString() + "): " + ex.Message);
            }
        }

        #endregion

        #endregion

        #region private help methods

        /* void OnDocumentLoadComplete(object pDesp, ref object pUrl)
         * when document loaded, tell the htmlobjectpool to reload all objects.
         */
        protected override void OnDocumentLoadComplete(object pDesp, ref object pUrl)
        {

            //when document loaded, tell the htmlobjectpool to reload all objects.

            HTMLTestObjectPool.DocumentRefreshed();

            base.OnDocumentLoadComplete(pDesp, ref pUrl);
        }

        #endregion

        #endregion
    }
}
