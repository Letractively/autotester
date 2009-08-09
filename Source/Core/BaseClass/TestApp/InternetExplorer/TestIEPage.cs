using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

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
            if (_objMap == null)
            {
                ITestObjectPool pool = GetObjectPool();
                pool.SetTestPage(this);
                _objMap = new TestObjectMap(pool);
            }

            return _objMap;
        }

        public Object[] GetAllElements()
        {
            if (_rootDocument == null)
            {
                throw new BrowserNotFoundException();
            }
            try
            {
                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                ITestDocument[] allDocs = GetAllDocuments();
                foreach (TestIEDocument doc in allDocs)
                {
                    try
                    {
                        IHTMLElement[] elements = doc.GetAllElements() as IHTMLElement[];
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
        public Object GetElementByID(string id)
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
                IHTMLElement element = _rootDocument.GetElementByID(id) as IHTMLElement;
                if (element == null)
                {
                    ITestDocument[] allDocs = GetAllDocuments();
                    foreach (TestIEDocument doc in allDocs)
                    {
                        element = doc.GetElementByID(id) as IHTMLElement;
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
        public Object[] GetElementsByName(string name)
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
                ITestDocument[] allDocs = GetAllDocuments();
                foreach (TestIEDocument doc in allDocs)
                {
                    try
                    {
                        IHTMLElement[] res = doc.GetElementsByName(name) as IHTMLElement[];
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
        public Object[] GetElementsByTagName(string tag)
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
                ITestDocument[] allDocs = GetAllDocuments();
                foreach (TestIEDocument doc in allDocs)
                {
                    try
                    {
                        IHTMLElement[] res = doc.GetElementsByTagName(tag) as IHTMLElement[];
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
        public Object GetElementByPoint(int x, int y)
        {
            try
            {
                IHTMLElement ele = _rootDocument.GetElementByPoint(x, y) as IHTMLElement;
                IHTMLDocument rootDoc = _rootDocument.Document;
                IHTMLDocument frameDoc = null;
                while (ele != null && ele is IHTMLIFrameElement)
                {
                    frameDoc = COMUtil.GetFrameDocument(ele as IHTMLIFrameElement);
                    Rectangle rect = WindowsAsstFunctions.GetOffsetPostion(rootDoc, frameDoc);
                    x = x - rect.Left;
                    y = y - rect.Top;
                    ele = (frameDoc as IHTMLDocument2).elementFromPoint(x, y);
                    rootDoc = frameDoc;
                }

                return ele;
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found object at point: (" + x.ToString() + "," + y.ToString() + "): " + ex.ToString());
            }
        }

        #endregion
    }
}
