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
        protected InternetExplorer _internalIE;
        protected TestIEDocument _rootDocument;

        protected TestObjectMap _objMap;
        protected String _title;
        protected String _url;

        #endregion

        #region properties

        public virtual int Left
        {
            get
            {
                return _internalIE.Left;
            }
        }

        public virtual int Top
        {
            get
            {
                return _internalIE.Top;
            }
        }

        public virtual int Width
        {
            get
            {
                return _internalIE.Width;
            }
        }

        public virtual int Height
        {
            get
            {
                return _internalIE.Height;
            }
        }

        public virtual IntPtr Handle
        {
            get
            {
                return (IntPtr)_internalIE.HWND;
            }
        }

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
                return GetObjectMap();
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

        public TestIEPage(TestInternetExplorer browser, InternetExplorer ie)
        {
            if (browser == null || ie == null)
            {
                throw new CannotGetTestPageException("Browser can not be null.");
            }

            try
            {
                this._browser = browser;
                this._internalIE = ie;
                this._rootDocument = new TestIEDocument(ie.Document as IHTMLDocument2);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetTestPageException(ex.ToString());
            }
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
                    return GetFrames(_rootDocument);
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotGetTestDocumentException("Can not get all documents in this page: " + ex.ToString());
                }
            }

            return null;
        }

        protected virtual TestIEDocument[] GetFrames(TestIEDocument root)
        {
            if (root != null)
            {
                List<TestIEDocument> allDocs = new List<TestIEDocument>();
                allDocs.Add(root);

                TestIEDocument[] frames = root.GetFrames() as TestIEDocument[];
                if (frames != null && frames.Length > 0)
                {
                    foreach (TestIEDocument frame in frames)
                    {
                        try
                        {
                            TestIEDocument[] subFrames = GetFrames(frame);
                            allDocs.AddRange(subFrames);
                        }
                        catch
                        {
                        }
                    }
                }

                return allDocs.ToArray();
            }

            return null;
        }

        /* string GetHTML()
      * return the HTML code of current page.
      */
        public virtual string GetAllHTMLContent()
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

        public virtual ITestObjectMap GetObjectMap()
        {
            throw new TestObjectMapException("Plugin must implement GetObjectMap");
        }

        #endregion
    }
}
