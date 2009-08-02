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
        private HTMLTestObjectMap _htmlMap;

        #endregion

        #region properties

        public new HTMLTestObjectMap Objects
        {
            get
            {
                return _htmlMap;
            }
        }

        public new HTMLTestDocument Document
        {
            get
            {
                return _htmlDoc;
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

        public new IHTMLElement GetElementByID(String id)
        {
            return base.GetElementByID(id) as IHTMLElement;
        }

        public new IHTMLElement GetElementByPoint(int x, int y)
        {
            return base.GetElementByPoint(x, y) as IHTMLElement;
        }

        public new IHTMLElement[] GetElementsByName(string name)
        {
            return base.GetElementsByName(name) as IHTMLElement[];
        }

        public new IHTMLElement[] GetElementsByTagName(String tag)
        {
            return base.GetElementsByTagName(tag) as IHTMLElement[];
        }

        public new IHTMLElement[] GetAllElements()
        {
            return base.GetAllElements() as IHTMLElement[];
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
            if (_htmlMap == null)
            {
                HTMLTestObjectPool pool = GetObjectPool() as HTMLTestObjectPool;
                pool.SetTestPage(this);
                _htmlMap = new HTMLTestObjectMap(pool);
            }

            return _htmlMap;
        }

        #endregion
    }
}
