using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.HTMLUtility
{
    [ComVisible(true)]
    public class HTMLTestDocument : HTMLDocumentClass
    {
        #region fields

        protected IHTMLDocument2 _document;
        protected HTMLTestBrowser _browser;

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

        public HTMLTestBrowser Browser
        {
            get { return this._browser; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestDocument(IHTMLDocument2 doc)
            : this(doc, null)
        {
        }

        public HTMLTestDocument(IHTMLDocument2 doc, HTMLTestBrowser browser)
            : base()
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

        public IHTMLElement GetElementByID(String id)
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

        public IHTMLElement GetElementByPoint(int x, int y)
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

        public IHTMLElement[] GetElementsByName(string name)
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

        public IHTMLElement[] GetElementsByTagName(String tag)
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

        public IHTMLElement[] GetAllElements()
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

        #region overide

        //public override IHTMLDOMNode appendChild(IHTMLDOMNode newChild)
        //{
        //    Notify(newChild);
        //    return base.appendChild(newChild);
        //}

        //public override mshtml.IHTMLDOMNode insertBefore(IHTMLDOMNode newChild, object refChild)
        //{
        //    Notify(newChild);
        //    return base.insertBefore(newChild, refChild);
        //}

        //public override void write(params object[] psarray)
        //{
        //    Notify(null);
        //    base.write(psarray);
        //}

        //public override void writeln(params object[] psarray)
        //{
        //    Notify(null);
        //    base.writeln(psarray);
        //}

        #endregion

        #endregion

        #region private

        protected virtual void RegisterEvents()
        {
            IHTMLDocument3 doc3 = this._document as IHTMLDocument3;
            if (doc3 != null)
            {
                doc3.attachEvent("propertychange", this);
            }
        }

        [DispId(0)]
        public void HandleEvent(IHTMLEventObj pEvtObj)
        {
            if (pEvtObj.type == "propertychange")
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
