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

        //key for cache.
        protected const string _keySplitter = "__shrinerainmap__";
        protected TestObject[] _lastObjects;

        protected bool _useCache = false;
        protected static int _timeout = 5;

        #endregion

        #region properties

        public ITestObjectPool ObjectPool
        {
            get { return _objPool; }
            set { _objPool = value; }
        }

        public int Timeout
        {
            get { return TestObjectMap._timeout; }
            set { TestObjectMap._timeout = value; }
        }

        #endregion

        #region methods

        #region ctor

        public TestObjectMap(ITestObjectPool pool)
        {
            this._objPool = pool;
        }

        #endregion

        #region public methods

        //check if an object exit or not.
        public virtual bool IsExist(String name)
        {
            return IsExist(null, name);
        }

        public virtual bool IsExist(String type, String name)
        {
            TestObject[] tmps;
            return TryGetObjects(type, name, out tmps);
        }

        public virtual bool TryGetObjects(String name, out TestObject[] objects)
        {
            objects = null;
            return TryGetObjects(null, name, out objects);
        }

        public virtual bool TryGetObjects(String type, String name, out TestObject[] objects)
        {
            objects = null;
            try
            {
                objects = GetMapObjects(type, name);
                return objects != null;
            }
            catch
            {
                return false;
            }
        }

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

        public virtual IClickable Button()
        {
            return Buttons()[0];
        }

        public virtual IClickable[] Buttons()
        {
            return Buttons(null);
        }

        public virtual IClickable Button(string name)
        {
            return Buttons(name)[0];
        }

        public virtual IClickable[] Buttons(string name)
        {
            GetMapObjects("button", name);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public virtual IInputable TextBox()
        {
            return TextBoxs()[0];
        }

        public virtual IInputable[] TextBoxs()
        {
            return TextBoxs(null);
        }

        public virtual IInputable TextBox(string name)
        {
            return TextBoxs(name)[0];
        }

        public virtual IInputable[] TextBoxs(string name)
        {
            GetMapObjects("TextBox", name);
            IInputable[] tmp = new IInputable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public virtual ICheckable CheckBox()
        {
            return CheckBoxs()[0];
        }

        public virtual ICheckable[] CheckBoxs()
        {
            return CheckBoxs(null);
        }

        public virtual ICheckable CheckBox(string name)
        {
            return CheckBoxs(name)[0];
        }

        public virtual ICheckable[] CheckBoxs(string name)
        {
            GetMapObjects("CheckBox", name);
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public virtual ISelectable ComboBox()
        {
            return ComboBoxs()[0];
        }

        public virtual ISelectable[] ComboBoxs()
        {
            return ComboBoxs(null);
        }

        public virtual ISelectable ComboBox(string name)
        {
            return ComboBoxs(name)[0];
        }

        public virtual ISelectable[] ComboBoxs(string name)
        {
            GetMapObjects("ComboBox", name);
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public virtual IPicture Image()
        {
            return Images()[0];
        }

        public virtual IPicture[] Images()
        {
            return Images(null);
        }

        public virtual IPicture Image(string name)
        {
            return Images(name)[0];
        }

        public virtual IPicture[] Images(string name)
        {
            GetMapObjects("Image", name);
            IPicture[] tmp = new IPicture[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IText Label()
        {
            return Labels()[0];
        }

        public virtual IText[] Labels()
        {
            return Labels(null);
        }

        public virtual IText Label(string name)
        {
            return Labels(name)[0];
        }

        public virtual IText[] Labels(string name)
        {
            GetMapObjects("Label", name);
            IText[] tmp = new IText[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IClickable Link()
        {
            return Links()[0];
        }

        public virtual IClickable[] Links()
        {
            return Links(null);
        }

        public virtual IClickable Link(string name)
        {
            return Links(name)[0];
        }

        public virtual IClickable[] Links(string name)
        {
            GetMapObjects("Link", name);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual ISelectable ListBox()
        {
            return ListBoxs()[0];
        }

        public virtual ISelectable[] ListBoxs()
        {
            return ListBoxs(null);
        }

        public virtual ISelectable ListBox(string name)
        {
            return ListBoxs(name)[0];
        }

        public virtual ISelectable[] ListBoxs(string name)
        {
            GetMapObjects("ListBox", name);
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }


        public virtual ICheckable RadioBox()
        {
            return RadioBoxs()[0];
        }

        public virtual ICheckable[] RadioBoxs()
        {
            return RadioBoxs(null);
        }

        public virtual ICheckable RadioBox(string name)
        {
            return RadioBoxs(name)[0];
        }

        public virtual ICheckable[] RadioBoxs(string name)
        {
            GetMapObjects("radiobox", name);
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual ITable Table()
        {
            return Tables()[0];
        }

        public virtual ITable[] Tables()
        {
            return Tables(null);
        }

        public virtual ITable Table(String name)
        {
            return Tables(name)[0];
        }

        public virtual ITable[] Tables(String name)
        {
            GetMapObjects("Table", name);
            ITable[] tmp = new ITable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IVisible AnyObject()
        {
            return AnyObjects()[0];
        }

        public virtual IVisible[] AnyObjects()
        {
            return AnyObjects(null);
        }

        public virtual IVisible AnyObject(String name)
        {
            return AnyObjects(name)[0];
        }

        public virtual IVisible[] AnyObjects(String name)
        {
            GetMapObjects(null, name);
            IVisible[] tmp = new IVisible[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IClickable Menu()
        {
            throw new NotImplementedException();
        }

        public virtual IClickable[] Menus()
        {
            throw new NotImplementedException();
        }

        public virtual IClickable Menu(string name)
        {
            throw new NotImplementedException();
        }

        public virtual IClickable[] Menus(string name)
        {
            throw new NotImplementedException();
        }

        public virtual IClickable Tab()
        {
            throw new NotImplementedException();
        }

        public virtual IClickable[] Tabs()
        {
            throw new NotImplementedException();
        }

        public virtual IClickable Tab(string name)
        {
            throw new NotImplementedException();
        }

        public virtual IClickable[] Tabs(string name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private methods

        protected virtual TestObject[] GetMapObjects(string type, string name)
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

                    found = TryGetObjectsFromPool(type, properties, out _lastObjects, out exception);
                }

                if (found)
                {
                    if (_useCache)
                    {
                        ObjectCache.InsertObjectToCache(key, _lastObjects[0]);
                    }
                }
                else
                {
                    throw new ObjectNotFoundException(exception.ToString());
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

            return _lastObjects;
        }

        private bool TryGetObjectsFromPool(string type, TestProperty[] properties, out TestObject[] obj, out TestException exception)
        {
            obj = null;
            exception = null;

            int currentTimeout = _timeout;
            int oriTimeout = this._objPool.GetTimeout();
            this._objPool.SetTimeout(currentTimeout);

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
}
