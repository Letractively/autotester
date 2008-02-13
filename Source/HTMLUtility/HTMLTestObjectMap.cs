/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestObjectMap.cs
*
* Description: This class provide the the object map for HTML testing.
*              Like other automation tools, we have a map to store objects.
*              We can get/insert an object to this map. 
*
* History: 2008/01/29 wan,yu Init version 
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Helper;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestObjectMap
    {

        #region fields

        //HTML test object type.
        private HTMLTestButton _button;
        private HTMLTestCheckBox _checkBox;
        private HTMLTestRadioButton _radioBtn;
        private HTMLTestComboBox _combobox;
        private HTMLTestFileDialog _fileDialog;
        private HTMLTestImage _img;
        private HTMLTestLabel _label;
        private HTMLTestLink _link;
        private HTMLTestListBox _listbox;
        private HTMLTestMsgBox _msgBox;
        private HTMLTestTable _table;
        private HTMLTestTextBox _textBox;
        private HTMLTestActiveXObject _activeXObj;

        //object pool for this map.
        private HTMLTestObjectPool _objPool;

        //key for cache.
        private const string _keySplitter = "__htmlmap__";


        #endregion

        #region properties

        public HTMLTestObjectPool HTMLTestObjectPool
        {
            get { return _objPool; }
            set
            {
                if (value != null)
                {
                    _objPool = value;
                    _objPool.OnNewObjectFound += new HTMLTestObjectPool._newObjectDelegate(AddTypeObjectToMap);
                }

            }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestObjectMap()
        {

        }

        public HTMLTestObjectMap(HTMLTestObjectPool pool)
        {
            this._objPool = pool;

            //when a new object is found, we will add it to the map.
            pool.OnNewObjectFound += new HTMLTestObjectPool._newObjectDelegate(AddTypeObjectToMap);
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

            HTMLTestGUIObject obj = (HTMLTestGUIObject)this._objPool.GetLastObject();

            Add(name, obj);
        }

        public void Add(string name, HTMLTestGUIObject obj)
        {
            try
            {
                if (obj != null)
                {
                    string key = null;

                    if (obj.Type != HTMLTestObjectType.Unknow)
                    {
                        key = obj.Type.ToString() + _keySplitter + name;

                        ObjectCache.InsertObjectToCache(key, obj);
                    }
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

        #region html test object

        /* HTMLTestButton Button()
         * return a button object.
         * will get the last object from pool, and convert it to a button.
         */
        public HTMLTestButton Button()
        {
            _button = (HTMLTestButton)this._objPool.GetLastObject();

            return _button;
        }

        /* HTMLTestButton Button(string name)
         * return a button by expected name.
         * Try to get a button from the cache, so firstly we need to use Add() function to add a button to the cache.
         */
        public HTMLTestButton Button(string name)
        {
            GetMapObject(name, HTMLTestObjectType.Button);

            return _button;
        }

        public HTMLTestCheckBox CheckBox()
        {
            _checkBox = (HTMLTestCheckBox)this._objPool.GetLastObject();

            return _checkBox;
        }

        public HTMLTestCheckBox CheckBox(string name)
        {
            GetMapObject(name, HTMLTestObjectType.CheckBox);

            return _checkBox;
        }

        public HTMLTestComboBox ComboBox()
        {
            _combobox = (HTMLTestComboBox)this._objPool.GetLastObject();

            return _combobox;
        }

        public HTMLTestComboBox ComboBox(string name)
        {
            GetMapObject(name, HTMLTestObjectType.ComboBox);

            return _combobox;
        }

        public HTMLTestFileDialog FileDialog()
        {
            _fileDialog = (HTMLTestFileDialog)this._objPool.GetLastObject();

            return _fileDialog;
        }

        public HTMLTestFileDialog FileDialog(string name)
        {
            GetMapObject(name, HTMLTestObjectType.FileDialog);

            return _fileDialog;
        }

        public HTMLTestImage Image()
        {
            _img = (HTMLTestImage)this._objPool.GetLastObject();

            return _img;
        }

        public HTMLTestImage Image(string name)
        {
            GetMapObject(name, HTMLTestObjectType.Image);

            return _img; ;
        }

        public HTMLTestLabel Label()
        {
            _label = (HTMLTestLabel)this._objPool.GetLastObject();

            return _label;
        }

        public HTMLTestLabel Label(string name)
        {
            GetMapObject(name, HTMLTestObjectType.Label);

            return _label;
        }

        public HTMLTestLink Link()
        {
            _link = (HTMLTestLink)this._objPool.GetLastObject();

            return _link;
        }

        public HTMLTestLink Link(string name)
        {
            GetMapObject(name, HTMLTestObjectType.Link);

            return _link;
        }

        public HTMLTestListBox ListBox()
        {
            _listbox = (HTMLTestListBox)this._objPool.GetLastObject();

            return _listbox;
        }

        public HTMLTestListBox ListBox(string name)
        {
            GetMapObject(name, HTMLTestObjectType.ListBox);

            return _listbox;
        }

        public HTMLTestMsgBox MsgBox()
        {
            _msgBox = (HTMLTestMsgBox)this._objPool.GetLastObject();

            return _msgBox;
        }

        public HTMLTestMsgBox MsgBox(string name)
        {
            GetMapObject(name, HTMLTestObjectType.MsgBox);

            return _msgBox;
        }

        public HTMLTestRadioButton RadioBtn()
        {
            _radioBtn = (HTMLTestRadioButton)this._objPool.GetLastObject();

            return _radioBtn;
        }

        public HTMLTestRadioButton RadioBtn(string name)
        {
            GetMapObject(name, HTMLTestObjectType.RadioButton);

            return _radioBtn;
        }

        public HTMLTestTable Table()
        {
            _table = (HTMLTestTable)this._objPool.GetLastObject();

            return _table;
        }

        public HTMLTestTable Table(String name)
        {
            GetMapObject(name, HTMLTestObjectType.Table);

            return _table;
        }

        public HTMLTestTextBox TextBox()
        {
            _textBox = (HTMLTestTextBox)this._objPool.GetLastObject();

            return _textBox;
        }

        public HTMLTestTextBox TextBox(string name)
        {
            GetMapObject(name, HTMLTestObjectType.TextBox);

            return _textBox;
        }

        public HTMLTestActiveXObject ActiveXObject()
        {
            _activeXObj = (HTMLTestActiveXObject)this._objPool.GetLastObject();

            return _activeXObj;
        }

        public HTMLTestActiveXObject ActiveXObject(string name)
        {
            GetMapObject(name, HTMLTestObjectType.ActiveX);

            return _activeXObj;
        }
        #endregion

        #endregion

        #region private methods

        /* void GetMapObject(string name, HTMLTestObjectType type)
         * Get the html test object from cache, convert to expected type.
         */
        private void GetMapObject(string name, HTMLTestObjectType type)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new CannotGetMapObjectException("Name can not be empty.");
            }

            try
            {
                if (type != HTMLTestObjectType.Unknow)
                {
                    string key = type.ToString() + _keySplitter + name;

                    object tmp = null;

                    if (ObjectCache.TryGetObjectFromCache(key, out tmp))
                    {
                        //build objects.
                        switch (type)
                        {
                            case HTMLTestObjectType.Button:
                                _button = (HTMLTestButton)tmp;
                                break;
                            case HTMLTestObjectType.CheckBox:
                                _checkBox = (HTMLTestCheckBox)tmp;
                                break;
                            case HTMLTestObjectType.ComboBox:
                                _combobox = (HTMLTestComboBox)tmp;
                                break;
                            case HTMLTestObjectType.FileDialog:
                                _fileDialog = (HTMLTestFileDialog)tmp;
                                break;
                            case HTMLTestObjectType.Image:
                                _img = (HTMLTestImage)tmp;
                                break;
                            case HTMLTestObjectType.Label:
                                _label = (HTMLTestLabel)tmp;
                                break;
                            case HTMLTestObjectType.Link:
                                _link = (HTMLTestLink)tmp;
                                break;
                            case HTMLTestObjectType.ListBox:
                                _listbox = (HTMLTestListBox)tmp;
                                break;
                            case HTMLTestObjectType.MsgBox:
                                _msgBox = (HTMLTestMsgBox)tmp;
                                break;
                            case HTMLTestObjectType.RadioButton:
                                _radioBtn = (HTMLTestRadioButton)tmp;
                                break;
                            case HTMLTestObjectType.Table:
                                _table = (HTMLTestTable)tmp;
                                break;
                            case HTMLTestObjectType.TextBox:
                                _textBox = (HTMLTestTextBox)tmp;
                                break;
                            case HTMLTestObjectType.ActiveX:
                                _activeXObj = (HTMLTestActiveXObject)tmp;
                                break;
                            default:
                                break;
                        }

                    }
                    else
                    {
                        throw new CannotGetMapObjectException("Can not get map object by: " + name);
                    }
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetMapObjectException("Can not get map object [" + name + "]: " + ex.Message);
            }
        }

        /* void AddTypeObjectToMap(string methodName, string[] paras, TestObject obj)
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
                        Add(name, (HTMLTestGUIObject)obj);
                    }
                }
            }
        }

        #endregion

        #endregion

    }

}
