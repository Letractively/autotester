using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Runtime;
using SHDocVw;
using mshtml;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestBrowser : TestBrowser
    {
        #region Fileds

        #endregion


        #region Properties

        #endregion

        #region Methods


        //for reflecting issue, I keep the constructor as "public"
        public HTMLTestBrowser()
        {
            this._browserName = "Internet Explorer";
        }


        ~HTMLTestBrowser()
        {
            Dispose();
        }

        //singleton
        public new static HTMLTestBrowser GetInstance()
        {
            if (_testBrowser == null)
            {
                _testBrowser = new HTMLTestBrowser();
            }

            return (HTMLTestBrowser)_testBrowser;
        }

        #region public methods


        #region GetObject methods
        public IHTMLElementCollection GetAllObjects()
        {
            if (this._HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._HTMLDom.all;
            }
            catch (Exception e)
            {
                throw new ObjectNotFoundException("Can not get all objects: " + e.Message);
            }

        }
        public IHTMLElement GetObjectByID(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new PropertyNotFoundException("ID can not be null.");
            }
            if (this._HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._HTMLDom.getElementById(id);
            }
            catch (Exception e)
            {
                throw new ObjectNotFoundException("Can not found test object by id:" + id + ": " + e.Message);
            }

        }
        public IHTMLElementCollection GetObjectsByName(string name)
        {
            if (this._HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }
            if (String.IsNullOrEmpty(name))
            {
                throw new PropertyNotFoundException("Name can not be null.");
            }
            try
            {
                return this._HTMLDom.getElementsByName(name);
            }
            catch (Exception e)
            {
                throw new ObjectNotFoundException("Can not found test object by name:" + name + ":" + e.Message);
            }
        }
        public IHTMLElementCollection GetObjectsByTagName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new PropertyNotFoundException("Tag name can not be null.");
            }
            if (this._HTMLDom == null)
            {
                throw new TestBrowserNotFoundException();
            }
            try
            {
                return this._HTMLDom.getElementsByTagName(name);

            }
            catch (Exception e)
            {
                throw new ObjectNotFoundException("Can not found test object by tag name:" + name + ":" + e.Message);
            }
        }

        public IHTMLElement GetObjectFromPoint(int x, int y)
        {
            if (x >= 0 && y >= 0)
            {
                try
                {
                    return _HTMLDom.elementFromPoint(x, y);
                }
                catch (Exception e)
                {
                    throw new ObjectNotFoundException("Can not found object at point: (" + x.ToString() + "," + y.ToString() + "): " + e.Message);
                }
            }
            else
            {
                throw new ObjectNotFoundException("Can not found object at point which less than (0,0)");
            }

        }
        #endregion

        #endregion

        #region private help methods

        protected override void OnDocumentLoadComplete(object pDesp, ref object pUrl)
        {
            //Console.WriteLine("HTMLTestBrowser");

            //when document loaded, tell the htmlobjectpool to reload all objects.

            HTMLTestObjectPool.DocumentRefreshed();

            base.OnDocumentLoadComplete(pDesp, ref pUrl);
        }

        #endregion

        #endregion
    }
}
