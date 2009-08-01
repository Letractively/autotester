using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.Core
{
    [ComVisible(true)]
    public class TestIEDocument : ITestDocument
    {
        #region fields

        protected IHTMLDocument2 _document;
        protected TestInternetExplorer _browser;
        protected TestIEPage _page;

        protected String _url;
        protected String _title;

        public delegate void DocumentChangeHandler(IHTMLDocument2 doc);
        public event DocumentChangeHandler OnDocumentChange;

        //cache.
        protected IHTMLElement[] _allElements;
        protected Dictionary<String, IHTMLElement[]> _tagElements = new Dictionary<string, IHTMLElement[]>(199);
        protected Dictionary<String, IHTMLElement[]> _nameElements = new Dictionary<string, IHTMLElement[]>(199);

        #endregion

        #region property

        public IHTMLDocument Document
        {
            get { return _document; }
        }

        public TestInternetExplorer Browser
        {
            get { return this._browser; }
        }

        public String URL
        {
            get
            {
                if (_document != null)
                {
                    _url = _document.url;
                }

                return _url;
            }
        }

        public String Title
        {
            get
            {
                if (_document != null)
                {
                    _title = _document.title;
                }

                return _title;
            }
        }

        public ITestPage Page
        {
            get { return _page; }
        }

        #endregion

        #region methods

        #region ctor

        public TestIEDocument(IHTMLDocument2 doc)
            : this(doc, null)
        {
        }

        public TestIEDocument(IHTMLDocument2 doc, TestInternetExplorer browser)
        {
            if (doc == null)
            {
                throw new CannotBuildObjectException("Document can not be null.");
            }

            this._document = doc;
            this._browser = browser;
            RegisterEvents();
        }

        #endregion

        #region public

        public Object GetElementByID(String id)
        {
            if (!String.IsNullOrEmpty(id) && this._document != null)
            {
                try
                {
                    IHTMLDocument3 doc3 = this._document as IHTMLDocument3;
                    if (doc3 != null)
                    {
                        return doc3.getElementById(id);
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        public Object GetElementByPoint(int x, int y)
        {
            if (this._document != null)
            {
                try
                {
                    return this._document.elementFromPoint(x, y);
                }
                catch
                {
                }
            }

            return null;
        }

        public Object[] GetElementsByName(string name)
        {
            IHTMLElement[] result = null;
            if (!String.IsNullOrEmpty(name) && this._document != null)
            {
                try
                {
                    name = name.ToUpper().Trim();
                    IHTMLDocument3 doc3 = this._document as IHTMLDocument3;
                    if (doc3 != null)
                    {
                        if (!_nameElements.TryGetValue(name, out result))
                        {
                            IHTMLElementCollection elements = doc3.getElementsByName(name);
                            result = new IHTMLElement[elements.length];
                            for (int i = 0; i < elements.length; i++)
                            {
                                try
                                {
                                    result[i] = elements.item(i, i) as IHTMLElement;
                                }
                                catch
                                {
                                }
                            }
                            _nameElements.Add(name, result);
                        }
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        public Object[] GetElementsByTagName(String tag)
        {
            IHTMLElement[] result = null;
            if (!String.IsNullOrEmpty(tag) && this._document != null)
            {
                try
                {
                    tag = tag.ToUpper().Trim();
                    IHTMLDocument3 doc3 = this._document as IHTMLDocument3;
                    if (doc3 != null)
                    {
                        if (!_tagElements.TryGetValue(tag, out result))
                        {
                            IHTMLElementCollection elements = doc3.getElementsByTagName(tag);
                            result = new IHTMLElement[elements.length];
                            for (int i = 0; i < elements.length; i++)
                            {
                                try
                                {
                                    result[i] = elements.item(i, i) as IHTMLElement;
                                }
                                catch
                                {
                                }
                            }
                            _tagElements.Add(tag, result);
                        }
                    }
                }
                catch
                {
                }
            }

            return result;
        }

        public Object[] GetAllElements()
        {
            if (_allElements == null && _document != null)
            {
                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                foreach (IHTMLElement ele in (IHTMLElementCollection)_document.body.all)
                {
                    try
                    {
                        allObjectList.Add(ele);
                    }
                    catch
                    {
                        continue;
                    }
                }
                _allElements = allObjectList.ToArray();
            }

            return _allElements;
        }

        public ITestDocument GetParent()
        {
            if (this._document != null)
            {
                try
                {
                    IHTMLDocument3 doc3 = this._document as IHTMLDocument3;
                    TestIEDocument parent = new TestIEDocument(doc3.parentDocument);
                    return parent;
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotGetTestDocumentException("Can not get parent document: " + ex.ToString());
                }
            }

            return null;
        }

        public ITestDocument[] GetFrames()
        {
            if (this._document != null)
            {
                return GetFrames(this);
            }

            return null;
        }

        public String GetHTMLContent()
        {
            if (this._document != null)
            {
                try
                {
                    return this._document.body.outerHTML;
                }
                catch
                {
                }
            }

            return null;
        }

        protected virtual TestIEDocument[] GetFrames(TestIEDocument root)
        {
            if (root != null)
            {
                IHTMLDocument doc = root.Document;
                if (doc != null)
                {
                    IHTMLDocument[] frames = COMUtil.GetFrames(doc);
                    if (frames != null && frames.Length > 0)
                    {
                        List<TestIEDocument> framesDocs = new List<TestIEDocument>();
                        foreach (IHTMLDocument frame in frames)
                        {
                            try
                            {
                                TestIEDocument frameDoc = new TestIEDocument(frame as IHTMLDocument2);
                                framesDocs.Add(frameDoc);
                            }
                            catch
                            {
                            }
                        }

                        return framesDocs.ToArray();
                    }
                }
            }

            return null;
        }


        #endregion

        #region private

        protected virtual void RegisterEvents()
        {
            IHTMLDocument3 doc3 = this._document as IHTMLDocument3;
            if (doc3 != null)
            {
                doc3.attachEvent("onpropertychange", this);
            }
        }

        [DispId(0)]
        public void HandleEvent(IHTMLEventObj pEvtObj)
        {
            if (pEvtObj.type == "onpropertychange")
            {
                IHTMLEventObj2 e2 = pEvtObj as IHTMLEventObj2;
                if (String.Compare("innerHTML", e2.propertyName, false) == 0 ||
                    String.Compare("outerHTML", e2.propertyName, false) == 0)
                {
                    Notify(null);
                }
            }
        }

        private void Notify(IHTMLDOMNode node)
        {
            _tagElements.Clear();
            _nameElements.Clear();
            _allElements = null;

            if (OnDocumentChange != null)
            {
                OnDocumentChange(_document);
            }
        }

        #endregion

        #endregion
    }
}
