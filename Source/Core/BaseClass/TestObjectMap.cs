using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestObjectMap : ITestObjectMap
    {
        #region fields

        //object pool for this map.
        protected ITestObjectPool _objPool;
        protected ITestApp _app;
        protected ITestBrowser _browser;

        //key for cache.
        protected const string _keySplitter = "__shrinerainmap__";
        protected TestObject[] _lastObjects;

        protected bool _useCache = false;
        protected const int Timeout = 5;

        #endregion

        #region properties

        #endregion

        #region methods

        #region ctor

        public TestObjectMap()
        {
        }

        public TestObjectMap(ITestApp testApp)
        {
            if (testApp != null)
            {
                _app = testApp;
                _objPool = _app.GetObjectPool();
            }
        }

        public TestObjectMap(ITestBrowser testBrowser)
        {
            if (testBrowser != null)
            {
                _browser = testBrowser;
                _objPool = testBrowser.GetObjectPool();
            }
        }

        #endregion

        #region public methods

        /* void Add(String name)
         * Add the last object to map, then we can use name to access it.
         * 
         */
        public void Add(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new CannotAddMapObjectException("Name can not be empty.");
            }

            TestObject obj = (TestObject)this._objPool.GetLastObject();
            Add(name, obj);
        }

        public void Add(string name, TestObject obj)
        {
            try
            {
                if (obj != null)
                {
                    string key = BuildKey(obj.Domain + name);
                    ObjectCache.InsertObjectToCache(key, obj);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAddMapObjectException("Can not add object[" + name + "] to map: " + ex.ToString());
            }
        }

        /* void Delete(string name)
         * delete an object from map.
         */
        public void Delete(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                try
                {
                    //check all keys.
                    string[] allKeys = ObjectCache.GetKeys();
                    foreach (string k in allKeys)
                    {
                        if (k.IndexOf(_keySplitter) > 0 && k.IndexOf(name) > 0)
                        {
                            //remove the object.
                            ObjectCache.Remove(k);
                            return;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        #region test object

        #region page and window

        public TestPage Page(int index)
        {
            if (index >= 0)
            {
                ITestBrowser browser = _browser.GetPage(index);
                if (browser != null)
                {
                    return new TestPage(browser);
                }
            }

            throw new BrowserNotFoundException("Can not get page by index: " + index);
        }

        public TestPage Page(string title, string url)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(url))
            {
                ITestBrowser browser = _browser.GetPage(title, url);
                if (browser != null)
                {
                    return new TestPage(browser);
                }
            }

            throw new BrowserNotFoundException("Can not get page by title: " + title + ", url: " + url);
        }

        public TestPage NewPage()
        {
            ITestBrowser browser = _browser.GetMostRecentPage();
            if (browser != null)
            {
                return new TestPage(browser);
            }

            throw new BrowserNotFoundException("Can not get new page.");
        }

        public TestWindow Window(int index)
        {
            if (index >= 0)
            {
                ITestApp app = _app.GetWindow(index);
                if (app != null)
                {
                    return new TestWindow(app);
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by index: " + index);
        }

        public TestWindow Window(string title, string className)
        {
            if (!String.IsNullOrEmpty(title) || !String.IsNullOrEmpty(className))
            {
                ITestApp app = _app.GetWindow(title, className);
                if (app != null)
                {
                    return new TestWindow(app);
                }
            }

            throw new AppNotFoundExpcetion("Can not find window by title: " + title + ", className: " + className);
        }

        public TestWindow NewWindow()
        {
            ITestApp app = _app.GetMostRecentWindow();
            if (app != null)
            {
                return new TestWindow(app);
            }

            throw new AppNotFoundExpcetion("Can not find new window.");
        }

        #endregion

        public IClickable Button()
        {
            return Buttons()[0];
        }

        public IClickable[] Buttons()
        {
            return Buttons(null);
        }

        public IClickable Button(string name)
        {
            return Buttons(name)[0];
        }

        public IClickable[] Buttons(string name)
        {
            GetMapObjects(name, "button");
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public IInputable TextBox()
        {
            return TextBoxs()[0];
        }

        public IInputable[] TextBoxs()
        {
            return TextBoxs(null);
        }

        public IInputable TextBox(string name)
        {
            return TextBoxs(name)[0];
        }

        public IInputable[] TextBoxs(string name)
        {
            GetMapObjects(name, "TextBox");
            IInputable[] tmp = new IInputable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public ICheckable CheckBox()
        {
            return CheckBoxs()[0];
        }

        public ICheckable[] CheckBoxs()
        {
            return CheckBoxs(null);
        }

        public ICheckable CheckBox(string name)
        {
            return CheckBoxs(name)[0];
        }

        public ICheckable[] CheckBoxs(string name)
        {
            GetMapObjects(name, "CheckBox");
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public ISelectable ComboBox()
        {
            return ComboBoxs()[0];
        }

        public ISelectable[] ComboBoxs()
        {
            return ComboBoxs(null);
        }

        public ISelectable ComboBox(string name)
        {
            return ComboBoxs(name)[0];
        }

        public ISelectable[] ComboBoxs(string name)
        {
            GetMapObjects(name, "ComboBox");
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public IPicture Image()
        {
            return Images()[0];
        }

        public IPicture[] Images()
        {
            return Images(null);
        }

        public IPicture Image(string name)
        {
            return Images(name)[0];
        }

        public IPicture[] Images(string name)
        {
            GetMapObjects(name, "Image");
            IPicture[] tmp = new IPicture[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public IText Label()
        {
            return Labels()[0];
        }

        public IText[] Labels()
        {
            return Labels(null);
        }

        public IText Label(string name)
        {
            return Labels(name)[0];
        }

        public IText[] Labels(string name)
        {
            GetMapObjects(name, "Label");
            IText[] tmp = new IText[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public IClickable Link()
        {
            return Links()[0];
        }

        public IClickable[] Links()
        {
            return Links(null);
        }

        public IClickable Link(string name)
        {
            return Links(name)[0];
        }

        public IClickable[] Links(string name)
        {
            GetMapObjects(name, "Link");
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public ISelectable ListBox()
        {
            return ListBoxs()[0];
        }

        public ISelectable[] ListBoxs()
        {
            return ListBoxs(null);
        }

        public ISelectable ListBox(string name)
        {
            return ListBoxs(name)[0];
        }

        public ISelectable[] ListBoxs(string name)
        {
            GetMapObjects(name, "ListBox");
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public ICheckable RadioBox()
        {
            return RadioBoxs()[0];
        }

        public ICheckable[] RadioBoxs()
        {
            return RadioBoxs(null);
        }

        public ICheckable RadioBox(string name)
        {
            return RadioBoxs(name)[0];
        }

        public ICheckable[] RadioBoxs(string name)
        {
            GetMapObjects(name, "radiobox");
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public ITable Table()
        {
            return Tables()[0];
        }

        public ITable[] Tables()
        {
            return Tables(null);
        }

        public ITable Table(String name)
        {
            return Tables(name)[0];
        }

        public ITable[] Tables(String name)
        {
            GetMapObjects(name, "Table");
            ITable[] tmp = new ITable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public IVisible AnyObject()
        {
            return AnyObjects()[0];
        }

        public IVisible[] AnyObjects()
        {
            return AnyObjects(null);
        }

        public IVisible AnyObject(String name)
        {
            return AnyObjects(name)[0];
        }

        public IVisible[] AnyObjects(String name)
        {
            GetMapObjects(name, null);
            IVisible[] tmp = new IVisible[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public IClickable Menu()
        {
            throw new NotImplementedException();
        }

        public IClickable[] Menus()
        {
            throw new NotImplementedException();
        }

        public IClickable Menu(string name)
        {
            throw new NotImplementedException();
        }

        public IClickable[] Menus(string name)
        {
            throw new NotImplementedException();
        }

        public IClickable Tab()
        {
            throw new NotImplementedException();
        }

        public IClickable[] Tabs()
        {
            throw new NotImplementedException();
        }

        public IClickable Tab(string name)
        {
            throw new NotImplementedException();
        }

        public IClickable[] Tabs(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private methods

        private void GetMapObjects(string name, string type)
        {
            try
            {
                bool found = false;
                TestException exception = null;

                type = (type == null ? "" : type);
                string key = BuildKey(type + name);
                TestObject cacheObj;
                if (_useCache && !String.IsNullOrEmpty(name) && ObjectCache.TryGetObjectFromCache(key, out cacheObj))
                {
                    _lastObjects = new TestObject[] { cacheObj };
                    found = true;
                }
                else
                {
                    //parse the description text.
                    //if the format is like "id=1", we think it is a property=value pair. 
                    TestProperty[] properties = null;
                    if (!TestProperty.TryGetProperties(name, out properties) && !String.IsNullOrEmpty(name))
                    {
                        properties = new TestProperty[] { new TestProperty(TestObject.VisibleProperty, name) };
                    }

                    if (TryGetObjectsFromPool(type, properties, out _lastObjects, out exception))
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    if (_useCache && _lastObjects[0].IsExist())
                    {
                        ObjectCache.InsertObjectToCache(key, _lastObjects[0]);
                    }
                }
                else
                {
                    throw new ObjectNotFoundException(exception.Message);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetMapObjectException("Can not get object from map: " + ex.ToString());
            }
        }

        private bool TryGetObjectsFromPool(string type, TestProperty[] properties, out TestObject[] obj, out TestException exception)
        {
            obj = null;
            exception = null;

            int oriTimeout = this._objPool.GetTimeout();
            this._objPool.SetTimeout(Timeout);
            try
            {
                if (String.IsNullOrEmpty(type) && (properties == null || properties.Length == 0))
                {
                    obj = this._objPool.GetAllObjects();
                }
                else
                {
                    if (String.IsNullOrEmpty(type))
                    {
                        obj = this._objPool.GetObjectsByProperties(properties);
                    }
                    else
                    {
                        obj = this._objPool.GetObjectsByType(type, properties);
                    }
                }

                return true;
            }
            catch (ObjectNotFoundException ex)
            {
                obj = new TestObject[] { new TestFakeObject() };
                exception = ex;
                return true;
            }
            catch (TestException ex)
            {
                exception = ex;
                return false;
            }
            finally
            {
                this._objPool.SetTimeout(oriTimeout);
            }
        }

        private void AddTypeObjectToMap(string methodName, string[] paras, TestObject obj)
        {
            if (!String.IsNullOrEmpty(methodName) && paras != null && obj != null)
            {
                if (paras.Length == 2)
                {
                    string name = paras[1];

                    if (!String.IsNullOrEmpty(name))
                    {
                        Add(name, obj);
                    }
                }
            }
        }

        private static String BuildKey(string feed)
        {
            return _keySplitter + feed;
        }

        #endregion

        #endregion
    }

    public class TestPage : TestObjectMap
    {
        public TestPage(ITestBrowser testBrowser)
        {
            if (testBrowser != null)
            {
                _browser = testBrowser;
                _objPool = testBrowser.GetObjectPool();
            }
        }
    }

    public class TestWindow : TestObjectMap
    {
        public TestWindow(ITestApp testApp)
        {
            if (testApp != null)
            {
                _app = testApp;
                _objPool = testApp.GetObjectPool();
            }
        }
    }
}
