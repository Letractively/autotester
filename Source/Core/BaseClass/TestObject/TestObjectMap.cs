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
        protected TestObject[] _lastObjects;
        protected int _timeout = 15;

        #endregion

        #region properties

        public ITestObjectPool ObjectPool
        {
            get { return _objPool; }
            set { _objPool = value; }
        }

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
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

        #region check object

        //check if an object exist or not.
        public virtual bool IsExist(String name)
        {
            return IsExist(null, name);
        }

        public virtual bool IsExist(String type, String name)
        {
            TestObject[] tmps;
            return TryGetObjects(type, name, out tmps);
        }

        public virtual bool IsExist(TestProperty[] properties)
        {
            TestObject[] tmps;
            return TryGetObjects(null, properties, out tmps);
        }

        public virtual bool IsExist(String type, TestProperty[] properties)
        {
            TestObject[] tmps;
            return TryGetObjects(type, properties, out tmps);
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

        public virtual bool TryGetObjects(TestProperty[] properties, out TestObject[] objects)
        {
            objects = null;
            try
            {
                objects = GetMapObjects(null, properties);
                return objects != null;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool TryGetObjects(String type, TestProperty[] properties, out TestObject[] objects)
        {
            objects = null;
            try
            {
                objects = GetMapObjects(type, properties);
                return objects != null;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region test object

        #region button

        public virtual IClickable Button()
        {
            return Button(String.Empty);
        }

        public virtual IClickable Button(string name)
        {
            return Buttons(name)[0];
        }

        public virtual IClickable Button(TestProperty[] properties)
        {
            return Buttons(properties)[0];
        }

        public virtual IClickable[] Buttons()
        {
            return Buttons(String.Empty);
        }

        public virtual IClickable[] Buttons(string name)
        {
            GetMapObjects(TestObjectType.Button, name);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IClickable[] Buttons(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.Button, properties);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region textbox

        public virtual IInputable TextBox()
        {
            return TextBox(String.Empty);
        }

        public virtual IInputable TextBox(string name)
        {
            return TextBoxs(name)[0];
        }

        public virtual IInputable TextBox(TestProperty[] properties)
        {
            return TextBoxs(properties)[0];
        }

        public virtual IInputable[] TextBoxs()
        {
            return TextBoxs(String.Empty);
        }

        public virtual IInputable[] TextBoxs(string name)
        {
            GetMapObjects(TestObjectType.TextBox, name);
            IInputable[] tmp = new IInputable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IInputable[] TextBoxs(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.TextBox, properties);
            IInputable[] tmp = new IInputable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region checkbox

        public virtual ICheckable CheckBox()
        {
            return CheckBox(String.Empty);
        }

        public virtual ICheckable CheckBox(string name)
        {
            return CheckBoxs(name)[0];
        }

        public virtual ICheckable CheckBox(TestProperty[] propertiese)
        {
            return CheckBoxs(propertiese)[0];
        }

        public virtual ICheckable[] CheckBoxs()
        {
            return CheckBoxs(String.Empty);
        }

        public virtual ICheckable[] CheckBoxs(string name)
        {
            GetMapObjects(TestObjectType.CheckBox, name);
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual ICheckable[] CheckBoxs(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.CheckBox, properties);
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region droplist

        public virtual ISelectable DropList()
        {
            return DropList(String.Empty);
        }

        public virtual ISelectable DropList(TestProperty[] properties)
        {
            return DropLists(properties)[0];
        }

        public virtual ISelectable DropList(string name)
        {
            return DropLists(name)[0];
        }

        public virtual ISelectable[] DropLists()
        {
            return DropLists(String.Empty);
        }

        public virtual ISelectable[] DropLists(string name)
        {
            GetMapObjects(TestObjectType.DropList, name);
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual ISelectable[] DropLists(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.DropList, properties);
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region picture

        public virtual IPicture Image()
        {
            return Image(String.Empty);
        }

        public virtual IPicture Image(string name)
        {
            return Images(name)[0];
        }

        public virtual IPicture Image(TestProperty[] properties)
        {
            return Images(properties)[0];
        }

        public virtual IPicture[] Images()
        {
            return Images(String.Empty);
        }

        public virtual IPicture[] Images(string name)
        {
            GetMapObjects(TestObjectType.Image, name);
            IPicture[] tmp = new IPicture[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IPicture[] Images(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.Image, properties);
            IPicture[] tmp = new IPicture[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region label

        public virtual IText Label()
        {
            return Label(String.Empty);
        }

        public virtual IText Label(string name)
        {
            return Labels(name)[0];
        }

        public virtual IText Label(TestProperty[] properties)
        {
            return Labels(properties)[0];
        }

        public virtual IText[] Labels()
        {
            return Labels(String.Empty);
        }

        public virtual IText[] Labels(string name)
        {
            GetMapObjects(TestObjectType.Label, name);
            IText[] tmp = new IText[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IText[] Labels(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.Label, properties);
            IText[] tmp = new IText[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region link

        public virtual IClickable Link()
        {
            return Link(String.Empty);
        }

        public virtual IClickable Link(string name)
        {
            return Links(name)[0];
        }

        public virtual IClickable Link(TestProperty[] properties)
        {
            return Links(properties)[0];
        }

        public virtual IClickable[] Links()
        {
            return Links(String.Empty);
        }

        public virtual IClickable[] Links(string name)
        {
            GetMapObjects(TestObjectType.Link, name);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IClickable[] Links(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.Link, properties);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region listbox

        public virtual ISelectable ListBox()
        {
            return ListBox(String.Empty);
        }

        public virtual ISelectable ListBox(TestProperty[] properties)
        {
            return ListBoxs(properties)[0];
        }

        public virtual ISelectable ListBox(string name)
        {
            return ListBoxs(name)[0];
        }

        public virtual ISelectable[] ListBoxs()
        {
            return ListBoxs(String.Empty);
        }

        public virtual ISelectable[] ListBoxs(string name)
        {
            GetMapObjects(TestObjectType.ListBox, name);
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual ISelectable[] ListBoxs(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.ListBox, properties);
            ISelectable[] tmp = new ISelectable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region radiobox

        public virtual ICheckable RadioBox()
        {
            return RadioBox(String.Empty);
        }

        public virtual ICheckable RadioBox(TestProperty[] properties)
        {
            return RadioBoxs(properties)[0];
        }

        public virtual ICheckable RadioBox(string name)
        {
            return RadioBoxs(name)[0];
        }

        public virtual ICheckable[] RadioBoxs()
        {
            return RadioBoxs(String.Empty);
        }

        public virtual ICheckable[] RadioBoxs(string name)
        {
            GetMapObjects(TestObjectType.RadioBox, name);
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual ICheckable[] RadioBoxs(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.RadioBox, properties);
            ICheckable[] tmp = new ICheckable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region table

        public virtual ITable Table()
        {
            return Table(String.Empty);
        }

        public virtual ITable Table(TestProperty[] properties)
        {
            return Tables(properties)[0];
        }

        public virtual ITable Table(String name)
        {
            return Tables(name)[0];
        }

        public virtual ITable[] Tables()
        {
            return Tables(String.Empty);
        }

        public virtual ITable[] Tables(String name)
        {
            GetMapObjects(TestObjectType.Table, name);
            ITable[] tmp = new ITable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual ITable[] Tables(TestProperty[] properties)
        {
            GetMapObjects(TestObjectType.Table, properties);
            ITable[] tmp = new ITable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region any type

        public virtual IVisible AnyObject()
        {
            return AnyObject(String.Empty);
        }

        public virtual IVisible AnyObject(String name)
        {
            return AnyObject(String.Empty, name);
        }

        public virtual IVisible AnyObject(String type, String name)
        {
            return AnyObjects(type, name)[0];
        }

        public virtual IVisible AnyObject(TestProperty[] properties)
        {
            return AnyObjects(properties)[0];
        }

        public virtual IVisible AnyObject(string type, TestProperty[] properties)
        {
            return AnyObjects(type, properties)[0];
        }

        public virtual IVisible[] AnyObjects()
        {
            return AnyObjects(String.Empty);
        }

        public virtual IVisible[] AnyObjects(String name)
        {
            return AnyObjects(String.Empty, name);
        }

        public virtual IVisible[] AnyObjects(String type, String name)
        {
            GetMapObjects(type, name);
            IVisible[] tmp = new IVisible[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IVisible[] AnyObjects(TestProperty[] properties)
        {
            return AnyObjects(String.Empty, properties);
        }

        public virtual IVisible[] AnyObjects(string type, TestProperty[] properties)
        {
            GetMapObjects(type, properties);
            IVisible[] tmp = new IVisible[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion


        #endregion

        #endregion

        #region private methods

        protected virtual TestObject[] GetMapObjects(string type, String description)
        {
            TestProperty[] properties = TestProperty.GetProperties(description);
            return GetMapObjects(type, properties);
        }

        protected virtual TestObject[] GetMapObjects(string type, TestProperty[] properties)
        {
            try
            {
                TestException exception = null;
                if (!TryGetObjectsFromPool(type, properties, out _lastObjects, out exception))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (TestProperty tp in properties)
                    {
                        sb.Append("{" + tp.ToString() + "},");
                    }

                    throw new ObjectNotFoundException(String.Format("Can not get object {0} with error: {1} ", sb.ToString(), exception.ToString()));
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (TestProperty tp in properties)
                {
                    sb.Append("{" + tp.ToString() + "},");
                }

                throw new ObjectNotFoundException(String.Format("Can not get object {0} with error: {1} ", sb.ToString(), ex.ToString()));
            }

            return _lastObjects;
        }


        private bool TryGetObjectsFromPool(string type, TestProperty[] properties, out TestObject[] obj, out TestException exception)
        {
            obj = null;
            exception = null;

            int currentTimeout = _timeout;
            int oriTimeout = this._objPool.SearchTimeout;
            this._objPool.SearchTimeout = currentTimeout;

            try
            {
                if (String.IsNullOrEmpty(type))
                {
                    if (properties == null || properties.Length == 0)
                    {
                        obj = this._objPool.GetAllObjects();
                    }
                    else
                    {
                        obj = this._objPool.GetObjectsByProperties(properties);
                    }
                }
                else
                {
                    obj = this._objPool.GetObjectsByType(type, properties);
                }

                return obj != null;
            }
            catch (TestException ex)
            {
                exception = ex;
                return false;
            }
            finally
            {
                this._objPool.SearchTimeout = oriTimeout;
            }
        }

        #endregion

        #endregion

    }
}
