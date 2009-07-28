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
    public class TestIEPage : ITestPage
    {
        #region fields

        protected TestInternetExplorer _browser;
        protected ITestDocument _rootDocument;

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
                        _title = _rootDocument.Title;
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
                        _url = _rootDocument.URL;
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

        public virtual ITestDocument Document
        {
            get
            {
                return _rootDocument;
            }
        }

        #endregion

        #region methods

        #region ctor

        public TestIEPage(TestInternetExplorer browser, ITestDocument rootDoc)
        {
            if (browser == null || rootDoc == null)
            {
                throw new CannotGetTestPageException("Browser and Document can not be null.");
            }
            this._browser = browser;
            this._rootDocument = rootDoc;
        }

        #endregion

        public virtual ITestDocument GetDocument()
        {
            return _rootDocument;
        }

        //return all documents, include frames.
        public virtual ITestDocument[] GetAllDocuments()
        {
            if (_rootDocument != null)
            {
                try
                {
                    List<ITestDocument> res = new List<ITestDocument>();
                    res.Add(_rootDocument);

                    ITestDocument[] frames = _rootDocument.GetFrames();
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
            ITestDocument[] allDocs = GetAllDocuments();
            StringBuilder sb = new StringBuilder();
            foreach (ITestDocument doc in allDocs)
            {
                try
                {
                    String content = doc.GetHTMLContent();
                    if (!String.IsNullOrEmpty(content))
                    {
                        sb.Append(content);
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
