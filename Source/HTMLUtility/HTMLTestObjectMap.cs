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
            set { _objPool = value; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestObjectMap(HTMLTestObjectPool pool)
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

            try
            {
                HTMLTestGUIObject obj = (HTMLTestGUIObject)this._objPool.GetLastObject();

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
        public HTMLTestButton Button()
        {
            try
            {
                _button = (HTMLTestButton)this._objPool.GetLastObject();

                return _button;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestButton Button(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.Button);

                return _button;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestCheckBox CheckBox()
        {
            try
            {
                _checkBox = (HTMLTestCheckBox)this._objPool.GetLastObject();
                return _checkBox;
            }
            catch
            {
                return null;
            }

        }

        public HTMLTestCheckBox CheckBox(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.CheckBox);

                return _checkBox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestComboBox ComboBox()
        {
            try
            {
                _combobox = (HTMLTestComboBox)this._objPool.GetLastObject();
                return _combobox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestComboBox ComboBox(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.ComboBox);

                return _combobox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestFileDialog FileDialog()
        {
            try
            {
                _fileDialog = (HTMLTestFileDialog)this._objPool.GetLastObject();

                return _fileDialog;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestFileDialog FileDialog(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.FileDialog);

                return _fileDialog;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestImage Image()
        {
            try
            {
                _img = (HTMLTestImage)this._objPool.GetLastObject();
                return _img;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestImage Image(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.Image);

                return _img; ;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestLabel Label()
        {
            try
            {
                _label = (HTMLTestLabel)this._objPool.GetLastObject();
                return _label;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestLabel Label(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.Label);

                return _label;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestLink Link()
        {
            try
            {
                _link = (HTMLTestLink)this._objPool.GetLastObject();
                return _link;
            }
            catch
            {
                return null;
            }

        }

        public HTMLTestLink Link(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.Link);

                return _link;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestListBox ListBox()
        {
            try
            {
                _listbox = (HTMLTestListBox)this._objPool.GetLastObject();
                return _listbox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestListBox ListBox(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.ListBox);

                return _listbox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestMsgBox MsgBox()
        {
            try
            {
                _msgBox = (HTMLTestMsgBox)this._objPool.GetLastObject();
                return _msgBox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestMsgBox MsgBox(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.MsgBox);

                return _msgBox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestRadioButton RadioBtn()
        {
            try
            {
                _radioBtn = (HTMLTestRadioButton)this._objPool.GetLastObject();
                return _radioBtn;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestRadioButton RadioBtn(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.RadioButton);

                return _radioBtn;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestTable Table()
        {
            try
            {
                _table = (HTMLTestTable)this._objPool.GetLastObject();
                return _table;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestTable Table(String name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.Table);

                return _table;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestTextBox TextBox()
        {
            try
            {
                _textBox = (HTMLTestTextBox)this._objPool.GetLastObject();
                return _textBox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestTextBox TextBox(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.TextBox);

                return _textBox;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestActiveXObject ActiveXObject()
        {
            try
            {
                _activeXObj = (HTMLTestActiveXObject)this._objPool.GetLastObject();
                return _activeXObj;
            }
            catch
            {
                return null;
            }
        }

        public HTMLTestActiveXObject ActiveXObject(string name)
        {
            try
            {
                GetMapObject(name, HTMLTestObjectType.ActiveX);

                return _activeXObj;
            }
            catch
            {
                return null;
            }
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
                throw new CannotGetMapObjectException("Can not get map object by: " + name);
            }
        }

        #endregion

        #endregion

    }

}
