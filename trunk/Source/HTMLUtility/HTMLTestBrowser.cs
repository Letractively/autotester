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
using System.Collections.Generic;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestBrowser : TestBrowser
    {
        #region Fileds

        private HTMLTestObjectPool _pool;
        private HTMLTestEventDispatcher _dispatcher;

        private IHTMLElement[] _allElements;
        private bool needRefresh = false;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public HTMLTestBrowser()
        {
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
        public IHTMLElement[] GetAllHTMLElements()
        {
            if (_rootDocument == null)
            {
                throw new BrowserNotFoundException();
            }
            try
            {
                if (needRefresh || _allElements == null)
                {
                    needRefresh = false;

                    List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                    HTMLDocument[] allDocs = GetAllDocuments();
                    foreach (HTMLDocument doc in allDocs)
                    {
                        try
                        {
                            foreach (IHTMLElement ele in (IHTMLElementCollection)doc.body.all)
                            {
                                if (ele != null)
                                {
                                    allObjectList.Add(ele);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    _allElements = allObjectList.ToArray();
                }

                return _allElements;
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
            if (_rootDocument == null)
            {
                throw new BrowserNotFoundException();
            }
            try
            {
                IHTMLElement element = _rootDocument.getElementById(id);
                if (element == null)
                {
                    HTMLDocument[] allDocs = GetAllDocuments();
                    foreach (HTMLDocument doc in allDocs)
                    {
                        element = doc.getElementById(id);
                        if (element != null)
                        {
                            return element;
                        }
                    }
                }

                return element;
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found test object by id:" + id + ": " + ex.Message);
            }
        }

        /* IHTMLElementCollection GetObjectsByName(string name)
         * return elements by .name property.
         */
        public IHTMLElement[] GetObjectsByName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new PropertyNotFoundException("Name can not be null.");
            }

            if (_rootDocument == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                HTMLDocument[] allDocs = GetAllDocuments();
                foreach (HTMLDocument doc in allDocs)
                {
                    try
                    {
                        foreach (IHTMLElement ele in doc.getElementsByName(name))
                        {
                            allObjectList.Add(ele);
                        }
                    }
                    catch
                    {
                    }
                }
                return allObjectList.ToArray();
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found test object by name:" + name + ":" + ex.Message);
            }
        }

        /* IHTMLElementCollection GetObjectsByTagName(string name)
         * return elements by tag, eg: <a> will return all link.
         */
        public IHTMLElement[] GetObjectsByTagName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new PropertyNotFoundException("Tag name can not be null.");
            }

            if (_rootDocument == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                HTMLDocument[] allDocs = GetAllDocuments();
                foreach (HTMLDocument doc in allDocs)
                {
                    try
                    {
                        foreach (IHTMLElement ele in doc.getElementsByTagName(name))
                        {
                            allObjectList.Add(ele);
                        }
                    }
                    catch
                    {
                    }
                }
                return allObjectList.ToArray();
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
                return _rootDocument.elementFromPoint(x, y);
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found object at point: (" + x.ToString() + "," + y.ToString() + "): " + ex.Message);
            }
        }

        public override ITestObjectPool GetObjectPool()
        {
            if (_pool == null)
            {
                _pool = new HTMLTestObjectPool(this);
            }

            return _pool;
        }

        public override ITestEventDispatcher GetEventDispatcher()
        {
            if (_dispatcher == null)
            {
                _dispatcher = HTMLTestEventDispatcher.GetInstance();
                _dispatcher.Start(this);
            }

            return _dispatcher;
        }

        #endregion

        #endregion

        #region private help methods

        /* void OnDocumentLoadComplete(object pDesp, ref object pUrl)
         * when document loaded, tell the htmlobjectpool to reload all objects.
         */
        protected override void OnDocumentLoadComplete(object pDesp, ref object pUrl)
        {
            if (_dispatcher != null)
            {
                string url = pUrl.ToString();
                if (!String.IsNullOrEmpty(url) && String.Compare(url, TestConstants.IE_BlankPage_Url, true) != 0)
                {
                    IHTMLDocument2 doc2 = (pDesp as IWebBrowser2).Document as IHTMLDocument2;
                    _dispatcher.RegisterEvents(doc2);
                }
            }
            needRefresh = true;
            base.OnDocumentLoadComplete(pDesp, ref pUrl);
        }

        protected override void OnDownloadComplete()
        {
            needRefresh = true;
            base.OnDownloadComplete();
        }

        #endregion

        #endregion
    }
}
