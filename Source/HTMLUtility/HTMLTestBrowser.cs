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
using System.Runtime.InteropServices;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestBrowser : TestBrowser
    {
        #region Fileds

        private HTMLTestObjectPool _pool;
        private HTMLTestEventDispatcher _dispatcher;
        private HTMLTestObjectMap _objMap;
        private HTMLTestPageMap _pageMap;

        private HTMLTestDocument[] _allDocuments;

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

        public override HTMLDocument[] GetAllDocuments()
        {
            if (this._allDocuments == null)
            {
                HTMLDocument[] allDocs = base.GetAllDocuments();
                _allDocuments = new HTMLTestDocument[allDocs.Length];
                for (int i = 0; i < allDocs.Length; i++)
                {
                    try
                    {
                        HTMLDocument doc = allDocs[i];
                        HTMLTestDocument testDoc = new HTMLTestDocument(doc as IHTMLDocument2);
                        _allDocuments[i] = testDoc;
                    }
                    catch
                    {
                    }
                }
            }

            return _allDocuments;
        }

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
                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                HTMLTestDocument[] allDocs = GetAllDocuments() as HTMLTestDocument[];
                foreach (HTMLTestDocument doc in allDocs)
                {
                    try
                    {
                        IHTMLElement[] elements = doc.GetAllElements();
                        allObjectList.AddRange(elements);
                    }
                    catch
                    {
                    }
                }

                return allObjectList.ToArray();

            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not get all objects: " + ex.ToString());
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
                    HTMLTestDocument[] allDocs = GetAllDocuments() as HTMLTestDocument[];
                    foreach (HTMLTestDocument doc in allDocs)
                    {
                        element = doc.GetElementByID(id);
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
                throw new ObjectNotFoundException("Can not found test object by id:" + id + ": " + ex.ToString());
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
                name = name.ToUpper();
                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                HTMLTestDocument[] allDocs = GetAllDocuments() as HTMLTestDocument[];
                foreach (HTMLTestDocument doc in allDocs)
                {
                    try
                    {
                        IHTMLElement[] res = doc.GetElementsByName(name);
                        allObjectList.AddRange(res);
                    }
                    catch
                    {
                    }
                }
                return allObjectList.ToArray();
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found test object by name:" + name + ":" + ex.ToString());
            }
        }

        /* IHTMLElementCollection GetObjectsByTagName(string name)
         * return elements by tag, eg: <a> will return all link.
         */
        public IHTMLElement[] GetObjectsByTagName(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                throw new PropertyNotFoundException("Tag name can not be null.");
            }

            if (_rootDocument == null)
            {
                throw new BrowserNotFoundException();
            }

            try
            {
                tag = tag.ToUpper();

                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                HTMLTestDocument[] allDocs = GetAllDocuments() as HTMLTestDocument[];
                foreach (HTMLTestDocument doc in allDocs)
                {
                    try
                    {
                        IHTMLElement[] res = doc.GetElementsByTagName(tag);
                        allObjectList.AddRange(res);
                    }
                    catch
                    {
                    }
                }
                return allObjectList.ToArray();
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found test object by tag name:" + tag + ":" + ex.ToString());
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
                throw new ObjectNotFoundException("Can not found object at point: (" + x.ToString() + "," + y.ToString() + "): " + ex.ToString());
            }
        }

        public override ITestObjectMap GetObjectMap()
        {
            if (_objMap == null)
            {
                GetObjectPool();
                _objMap = new HTMLTestObjectMap(_pool);
            }
            return _objMap;
        }

        public override ITestObjectPool GetObjectPool()
        {
            if (_pool == null)
            {
                _pool = new HTMLTestObjectPool(this);
            }
            return _pool;
        }

        public override ITestPageMap GetPageMap()
        {
            if (_pageMap == null)
            {
                _pageMap = new HTMLTestPageMap(this);
            }
            return _pageMap;
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

        protected bool IsDocumentContained(IHTMLDocument doc)
        {
            if (this._allDocuments != null && doc != null)
            {
                IntPtr newDocIUnknown = Marshal.GetIUnknownForObject(doc);
                foreach (HTMLTestDocument existedDoc in this._allDocuments)
                {
                    IntPtr existedDocIUnknown = Marshal.GetIUnknownForObject(existedDoc);
                    if (newDocIUnknown == existedDocIUnknown)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override void RegBrowserEvent(InternetExplorer ie)
        {
            base.RegBrowserEvent(ie);
            if (_dispatcher != null)
            {
                _dispatcher.RegisterEvents(ie.Document as IHTMLDocument2);
            }
        }

        protected override void AllDocumentComplete()
        {
            base.AllDocumentComplete();
            if (_dispatcher != null)
            {
                _dispatcher.RegisterEvents(_rootDocument as IHTMLDocument2);
            }
            NeedRefresh();
        }

        protected override void PageChange(InternetExplorer ie)
        {
            base.PageChange(ie);
            NeedRefresh();
        }

        protected void NeedRefresh()
        {
            _allDocuments = null;
        }

        #endregion

        #endregion
    }
}
