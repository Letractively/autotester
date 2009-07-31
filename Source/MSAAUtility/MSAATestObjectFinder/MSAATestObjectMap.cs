using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestObjectMap : TestObjectMap
    {
        #region fields

        public override ITestObjectType ObjectType
        {
            get
            {
                if (_objType == null)
                {
                    _objType = new MSAATestObjectType();
                }

                return _objType;
            }
        }

        #endregion

        #region methods

        #region ctor

        public MSAATestObjectMap(MSAATestObjectPool pool)
            : base(pool)
        {
        }

        #endregion


        #region menu

        public virtual IClickable Menu()
        {
            return Menu(String.Empty);
        }

        public virtual IClickable Menu(string name)
        {
            return Menus(name)[0];
        }

        public virtual IClickable Menu(TestProperty[] properties)
        {
            return Menus(properties)[0];
        }

        public virtual IClickable[] Menus()
        {
            return Menus(String.Empty);
        }

        public virtual IClickable[] Menus(string name)
        {
            GetMapObjects(MSAATestObjectType.MenuBar, name);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IClickable[] Menus(TestProperty[] properties)
        {
            GetMapObjects(MSAATestObjectType.MenuBar, properties);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region tab

        public virtual IClickable Tab()
        {
            return Tab(String.Empty);
        }

        public virtual IClickable Tab(string name)
        {
            return Tabs(name)[0];
        }

        public virtual IClickable Tab(TestProperty[] properties)
        {
            return Tabs(properties)[0];
        }

        public virtual IClickable[] Tabs()
        {
            return Tabs(String.Empty);
        }

        public virtual IClickable[] Tabs(string name)
        {
            GetMapObjects(MSAATestObjectType.Tab, name);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual IClickable[] Tabs(TestProperty[] properties)
        {
            GetMapObjects(MSAATestObjectType.Tab, properties);
            IClickable[] tmp = new IClickable[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #region ComboBox

        public virtual MSAATestComboBox ComboBox()
        {
            return ComboBox(String.Empty);
        }

        public virtual MSAATestComboBox ComboBox(TestProperty[] propertiese)
        {
            return ComboBoxs(propertiese)[0];
        }

        public virtual MSAATestComboBox ComboBox(string name)
        {
            return ComboBoxs(name)[0];
        }

        public virtual MSAATestComboBox[] ComboBoxs()
        {
            return ComboBoxs(String.Empty);
        }

        public virtual MSAATestComboBox[] ComboBoxs(string name)
        {
            GetMapObjects(MSAATestObjectType.ComboBox, name);
            MSAATestComboBox[] tmp = new MSAATestComboBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        public virtual MSAATestComboBox[] ComboBoxs(TestProperty[] properties)
        {
            GetMapObjects(MSAATestObjectType.ComboBox, properties);
            MSAATestComboBox[] tmp = new MSAATestComboBox[_lastObjects.Length];
            _lastObjects.CopyTo(tmp, 0);
            return tmp;
        }

        #endregion

        #endregion
    }
}
