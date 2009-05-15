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

        //key for cache.
        private const string _keySplitter = "__shrinerainmap__";
        private TestObject[] _lastObjects;

        private bool _useCache = false;
        private const int Timeout = 5;
        #endregion

        #region properties

        #endregion

        #region methods

        #region ctor

        public TestObjectMap(ITestObjectPool pool)
        {
            this._objPool = pool;
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
                if (String.IsNullOrEmpty(type))
                {
                    obj = this._objPool.GetObjectsByProperties(properties);
                }
                else
                {
                    obj = this._objPool.GetObjectsByType(type, properties);
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

        private String BuildKey(string feed)
        {
            return _keySplitter + feed;
        }

        #endregion

        #endregion
    }
}
