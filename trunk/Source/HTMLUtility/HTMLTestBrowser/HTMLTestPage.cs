using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestPage : TestPage
    {
        #region fields

        private HTMLTestDocument _htmlDoc;
        private HTMLTestObjectPool _objPool;

        #endregion

        #region properties

        public new HTMLTestObjectMap Objects
        {
            get { return _objMap as HTMLTestObjectMap; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestPage(HTMLTestBrowser browser, IHTMLDocument document)
            : base(browser, document)
        {
            this._htmlDoc = new HTMLTestDocument(document as IHTMLDocument2);
            HTMLTestObjectPool pool = new HTMLTestObjectPool(this);
            this._objMap = new HTMLTestObjectMap(pool);
        }

        #endregion

        public override IHTMLDocument[] GetAllDocuments()
        {
            IHTMLDocument[] allDocs = base.GetAllDocuments();
            HTMLTestDocument[] allDocuments = new HTMLTestDocument[allDocs.Length];
            for (int i = 0; i < allDocs.Length; i++)
            {
                try
                {
                    IHTMLDocument doc = allDocs[i];
                    HTMLTestDocument testDoc = new HTMLTestDocument(doc as IHTMLDocument2);
                    allDocuments[i] = testDoc;
                }
                catch
                {
                }
            }

            return allDocuments;
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
                IHTMLDocument[] allDocs = GetAllDocuments();
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
                IHTMLElement element = _htmlDoc.GetElementByID(id);
                if (element == null)
                {
                    IHTMLDocument[] allDocs = GetAllDocuments();
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
                IHTMLDocument[] allDocs = GetAllDocuments();
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
                IHTMLDocument[] allDocs = GetAllDocuments();
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
                return _htmlDoc.GetElementByPoint(x, y);
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

        #endregion
    }
}
