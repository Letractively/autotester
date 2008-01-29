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

        //object pool for this map.
        private HTMLTestObjectPool _objPool;


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
            return _button;
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
            return _checkBox;
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
            return _combobox;
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
            return _fileDialog;
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
            return _img;
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
            return _label;
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
            return _link;
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
            return _listbox;
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
            return _msgBox;
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
            return _radioBtn;
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
            return _table;
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
            return _textBox;
        }

        #endregion

        #region private methods


        #endregion

        #endregion

    }

}
