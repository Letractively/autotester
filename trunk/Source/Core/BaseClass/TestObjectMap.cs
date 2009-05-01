using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public class TestObjectMap
    {
        #region fields

        //object pool for this map.
        private ITestObjectPool _objPool;

        private IClickable _button;
        private IInputable _textBox;
        private ICheckable _checkBox;
        private ICheckable _radioBox;
        private ISelectable _comboBox;
        private ISelectable _listBox;
        private IClickable _link;
        private IText _label;
        private IPicture _img;
        private ITable _table;

        //key for cache.
        private const string _keySplitter = "__shrinerainmap__";

        private TestObject _lastObject;

        #endregion

        #region properties

        #endregion

        #region methods

        #region ctor

        public TestObjectMap(ITestObjectPool pool)
        {
            this._objPool = pool;

            //when a new object is found, we will add it to the map.
            //pool.OnObjectFound += new HTMLTestObjectPool.ObjectFoundHandler(AddTypeObjectToMap);
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
                throw new CannotAddMapObjectException("Can not add object[" + name + "] to map: " + ex.Message);
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

        #region html test object

        /* HTMLTestButton Button()
         * return a button object.
         * will get the last object from pool, and convert it to a button.
         */
        public IClickable Button()
        {
            _button = (IClickable)GetNamelessObject("button");
            return _button;
        }

        /* HTMLTestButton Button(string name)
         * return a button by expected name.
         * Try to get a button from the cache, so firstly we need to use Add() function to add a button to the cache.
         */
        public IClickable Button(string name)
        {
            GetMapObject(name, "button");
            return (IClickable)_lastObject;
        }

        public ICheckable CheckBox()
        {
            _checkBox = (ICheckable)GetNamelessObject("checkbox");
            return _checkBox;
        }

        public ICheckable CheckBox(string name)
        {
            GetMapObject(name, "CheckBox");
            return (ICheckable)_lastObject;
        }

        public ISelectable ComboBox()
        {
            _comboBox = (ISelectable)GetNamelessObject("combobox");
            return _comboBox;
        }

        public ISelectable ComboBox(string name)
        {
            GetMapObject(name, "ComboBox");
            return (ISelectable)_lastObject;
        }

        public IPicture Image()
        {
            _img = (IPicture)GetNamelessObject("image");
            return _img;
        }

        public IPicture Image(string name)
        {
            GetMapObject(name, "Image");
            return (IPicture)_lastObject;
        }

        public IText Label()
        {
            _label = (IText)GetNamelessObject("label");
            return _label;
        }

        public IText Label(string name)
        {
            GetMapObject(name, "Label");
            return (IText)_lastObject;
        }

        public IClickable Link()
        {
            _link = (IClickable)GetNamelessObject("link");
            return _link;
        }

        public IClickable Link(string name)
        {
            GetMapObject(name, "Link");
            return (IClickable)_lastObject; ;
        }

        public ISelectable ListBox()
        {
            _listBox = (ISelectable)GetNamelessObject("listbox");
            return _listBox;
        }

        public ISelectable ListBox(string name)
        {
            GetMapObject(name, "ListBox");
            return (ISelectable)_lastObject;
        }

        public ICheckable RadioBox()
        {
            _radioBox = (ICheckable)GetNamelessObject("radiobox");
            return (ICheckable)_lastObject;
        }

        public ICheckable RadioBox(string name)
        {
            GetMapObject(name, "radiobox");
            return (ICheckable)_lastObject;
        }

        public ITable Table()
        {
            _table = (ITable)GetNamelessObject("table");
            return _table;
        }

        public ITable Table(String name)
        {
            GetMapObject(name, "Table");
            return (ITable)_lastObject;
        }

        public IInputable TextBox()
        {
            _textBox = (IInputable)GetNamelessObject("textbox");
            return _textBox;
        }

        public IInputable TextBox(string name)
        {
            GetMapObject(name, "TextBox");
            return (IInputable)_lastObject;
        }

        #endregion

        #endregion

        #region private methods

        /* bool GetMapObject(string name, HTMLTestObjectType type)
         * Get the html test object from cache, convert to expected type.
         */
        private void GetMapObject(string name, string type)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new CannotGetMapObjectException("name can not be empty.");
            }

            try
            {
                bool found = false;

                TestException exception = null;

                if (!String.IsNullOrEmpty(type))
                {
                    string key = BuildKey(type.ToString() + name);

                    if (ObjectCache.TryGetObjectFromCache(key, out _lastObject))
                    {
                        found = true;
                    }
                    else
                    {
                        TestProperty[] properties = null;
                        //parse the description text.
                        //if the format is like "id=1", we think it is a property=value pair. 
                        if (TestProperty.GetProperties(name, out properties))
                        {
                            if (GetObjectFromPool(properties, out _lastObject, out exception))
                            {
                                found = true;
                            }
                        }
                        else
                        {
                            if (GetObjectFromPool(name, type.ToString(), out _lastObject, out exception))
                            {
                                found = true;
                            }
                        }
                    }

                    if (!found)
                    {
                        throw new ObjectNotFoundException(exception.Message);
                    }
                }
                else
                {
                    throw new CannotGetMapObjectException("Unkown object type.");
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetMapObjectException("Can not get object from map: " + ex.Message);
            }
        }

        /* void AddTypeObjectToMap(string methodName, string[] paras, object obj)
         * add object to map automatically.
         */
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

        /* bool GetObjectFromPool(string[] properties, string[] values, out object obj)
         * return HTMLTestGuiObject by a string description.
         */
        private bool GetObjectFromPool(TestProperty[] properties, out TestObject obj, out TestException exception)
        {
            obj = null;
            exception = null;

            if (properties != null)
            {
                try
                {
                    obj = this._objPool.GetObjectsByProperties(properties)[0];
                    return true;
                }
                catch (TestException ex)
                {
                    exception = ex;
                    return false;
                }
            }

            return false;
        }

        private bool GetObjectFromPool(string text, string type, out TestObject obj, out TestException exception)
        {
            obj = null;
            exception = null;

            if (!String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(type))
            {
                try
                {
                    obj = this._objPool.GetObjectsByType(type, new TestProperty[] { new TestProperty(TestObject.VisibleProperty, text) })[0];
                    return true;
                }
                catch (TestException ex)
                {
                    exception = ex;
                    return false;
                }
            }

            return false;
        }

        /* Object GetNamelessObject(string type)
         * return the object with no name specific.
         * firstly, try to get the last object, if failed, search object from pool.
         */
        private TestObject GetNamelessObject(string type)
        {
            if (!String.IsNullOrEmpty(type))
            {
                return this._objPool.GetObjectsByType(type, null)[0];
            }

            throw new ObjectNotFoundException("Type can not be empty.");

        }

        private String BuildKey(string feed)
        {
            return _keySplitter + feed;
        }

        #endregion

        #endregion
    }
}
