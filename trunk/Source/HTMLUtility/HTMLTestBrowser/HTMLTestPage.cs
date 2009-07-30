using System;
using System.Collections.Generic;
using System.Text;

using mshtml;
using SHDocVw;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestPage : TestIEPage
    {
        #region fields

        private HTMLTestDocument _htmlDoc;
        private HTMLTestObjectPool _objPool;

        #endregion

        #region properties

        public new HTMLTestObjectMap Objects
        {
            get
            {
                return GetObjectMap() as HTMLTestObjectMap;
            }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestPage(HTMLTestBrowser browser, InternetExplorer ie)
            : base(browser, ie)
        {
            try
            {
                this._htmlDoc = new HTMLTestDocument(ie.Document as IHTMLDocument2);
                HTMLTestObjectPool pool = new HTMLTestObjectPool(this);
                this._objMap = new HTMLTestObjectMap(pool);
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

        public override ITestDocument[] GetAllDocuments()
        {
            ITestDocument[] allDocs = base.GetAllDocuments();
            HTMLTestDocument[] allDocuments = new HTMLTestDocument[allDocs.Length];
            for (int i = 0; i < allDocs.Length; i++)
            {
                try
                {
                    IHTMLDocument doc = (allDocs[i] as TestIEDocument).Document;
                    HTMLTestDocument testDoc = new HTMLTestDocument(doc as IHTMLDocument2);
                    allDocuments[i] = testDoc;
                }
                catch
                {
                }
            }

            return allDocuments;
        }

        public IHTMLElement[] GetAllHTMLElements()
        {
            if (_rootDocument == null)
            {
                throw new BrowserNotFoundException();
            }
            try
            {
                List<IHTMLElement> allObjectList = new List<IHTMLElement>();
                ITestDocument[] allDocs = GetAllDocuments();
                foreach (HTMLTestDocument doc in allDocs)
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
        public IHTMLElement GetElementByID(string id)
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
                IHTMLElement element = _htmlDoc.GetElementByID(id) as IHTMLElement;
                if (element == null)
                {
                    ITestDocument[] allDocs = GetAllDocuments();
                    foreach (HTMLTestDocument doc in allDocs)
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
        public IHTMLElement[] GetElementsByName(string name)
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
                foreach (HTMLTestDocument doc in allDocs)
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
        public IHTMLElement[] GetElementsByTagName(string tag)
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
                foreach (HTMLTestDocument doc in allDocs)
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
        public IHTMLElement GetElementByPoint(int x, int y)
        {
            try
            {
                return _htmlDoc.GetElementByPoint(x, y) as IHTMLElement;
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not found object at point: (" + x.ToString() + "," + y.ToString() + "): " + ex.ToString());
            }
        }

        public override ITestObjectPool GetObjectPool()
        {
            if (_objPool == null)
            {
                _objPool = new HTMLTestObjectPool(this);
            }

            return _objPool;
        }

        public override ITestObjectMap GetObjectMap()
        {
            if (_objMap == null)
            {
                HTMLTestObjectPool pool = GetObjectPool() as HTMLTestObjectPool;
                pool.SetTestPage(this);
                _objMap = new HTMLTestObjectMap(pool);
            }

            return _objMap;
        }

        #endregion
    }
}
