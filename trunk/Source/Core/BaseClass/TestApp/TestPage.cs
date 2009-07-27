using System;
using System.Collections.Generic;
using System.Text;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.Core
{
    public class TestPage : ITestPage
    {
        #region fields

        protected TestBrowser _browser;
        protected IHTMLDocument _rootDocument;

        protected TestObjectMap _objMap;
        protected String _title;
        protected String _url;

        #endregion

        #region properties

        public virtual String Title
        {
            get
            {
                if (String.IsNullOrEmpty(_title))
                {
                    if (_rootDocument != null)
                    {
                        IHTMLDocument2 doc2 = _rootDocument as IHTMLDocument2;
                        if (doc2 != null)
                        {
                            _title = doc2.title;
                        }
                    }
                }

                return _title;
            }
        }

        public virtual String URL
        {
            get
            {
                if (String.IsNullOrEmpty(_url))
                {
                    if (_rootDocument != null)
                    {
                        IHTMLDocument2 doc2 = _rootDocument as IHTMLDocument2;
                        if (doc2 != null)
                        {
                            _url = doc2.url;
                        }
                    }
                }

                return _url;
            }
        }

        public virtual ITestObjectMap Objects
        {
            get
            {
                if (_objMap == null)
                {
                    ITestObjectPool pool = GetObjectPool();
                    _objMap = new TestObjectMap(pool);
                }

                return _objMap;
            }
        }

        public virtual ITestBrowser Browser
        {
            get
            {
                return _browser;
            }
        }

        public virtual IHTMLDocument Document
        {
            get
            {
                return _rootDocument;
            }
        }

        #endregion

        #region methods

        #region ctor

        public TestPage(TestBrowser browser, IHTMLDocument rootDoc)
        {
            if (browser == null || rootDoc == null)
            {
                throw new CannotGetTestPageException("Browser and Document can not be null.");
            }
            this._browser = browser;
            this._rootDocument = rootDoc;
        }

        #endregion

        public virtual IHTMLDocument GetDocument()
        {
            return _rootDocument;
        }

        //return all documents, include frames.
        public virtual IHTMLDocument[] GetAllDocuments()
        {
            if (_rootDocument != null)
            {
                try
                {
                    List<IHTMLDocument> res = new List<IHTMLDocument>();
                    res.Add(_rootDocument);

                    IHTMLDocument[] frames = COMUtil.GetFrames(_rootDocument);
                    if (frames != null)
                    {
                        res.AddRange(frames);
                    }

                    return res.ToArray();
                }
                catch
                {
                }
            }

            return null;
        }

        /* string GetHTML()
      * return the HTML code of current page.
      */
        public virtual string GetAllHTML()
        {
            IHTMLDocument[] allDocs = GetAllDocuments();
            StringBuilder sb = new StringBuilder();
            foreach (HTMLDocument doc in allDocs)
            {
                try
                {
                    if (doc.body != null && doc.body.innerHTML != null)
                    {
                        sb.Append(doc.body.innerHTML);
                    }
                }
                catch
                {
                    continue;
                }
            }
            return sb.ToString();
        }

        public virtual ITestObjectPool GetObjectPool()
        {
            throw new TestObjectPoolExcpetion("Plugin must implement GetObjectPool");
        }

        #endregion
    }
}
