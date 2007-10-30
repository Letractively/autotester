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

        public HTMLTestBrowser()
        {

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
            catch
            {
                throw new ObjectNotFoundException("Can not get all objects.");
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
            catch
            {
                throw new ObjectNotFoundException("Can not found test object by id:" + id);
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
            catch
            {
                throw new ObjectNotFoundException("Can not found test object by name:" + name);
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
            catch
            {
                throw new ObjectNotFoundException("Can not found test object by tag name:" + name);
            }
        }
        #endregion

        #endregion

        #region private help methods

        #endregion

        #endregion
    }
}
