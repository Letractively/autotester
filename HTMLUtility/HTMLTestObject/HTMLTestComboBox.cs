using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using mshtml;

using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestComboBox : HTMLGuiTestObject, ISelectable, IWindows
    {

        #region fields

        protected IntPtr _handle;

        protected string[] _allValues;
        protected string _selectedValue;
        protected int _itemCountPerPage = 30;
        protected int _itemHeight;

        protected HTMLSelectElementClass _htmlSelectClass;

        #endregion

        #region properties
        public int ItemHeight
        {
            get { return _itemHeight; }
        }
        public int ItemCountPerPage
        {
            get { return _itemCountPerPage; }
        }
        public string SelectedValue
        {
            get { return _selectedValue; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestComboBox(IHTMLElement element)
            : base(element)
        {
            try
            {
                _htmlSelectClass = (HTMLSelectElementClass)this._sourceElement;
            }
            catch
            {
                throw new CanNotBuildObjectException("HTML source element can not be null.");
            }
            try
            {
                this._allValues = this.GetAllValues();
            }
            catch
            {
                throw new CanNotBuildObjectException("Can not get the list values.");
            }
            try
            {
                this._selectedValue = this.GetSelectedValue();
            }
            catch
            {
                this._selectedValue = "";
            }
            try
            {
                IntPtr comboboxHandle = Win32API.FindWindowEx(TestBrowser.IEServerHandle, IntPtr.Zero, "Internet Explorer_TridentCmboBx", null);
                while (comboboxHandle != IntPtr.Zero)
                {
                    Win32API.Rect tmpRect = new Win32API.Rect();
                    Win32API.GetWindowRect(comboboxHandle, ref tmpRect);
                    int centerX = (tmpRect.right - tmpRect.left) / 2 + tmpRect.left;
                    int centerY = (tmpRect.bottom - tmpRect.top) / 2 + tmpRect.top;

                    if ((centerX > this.Rect.Left && centerX < this.Rect.Left + this.Rect.Width) && (centerY > this.Rect.Top && centerY < this.Rect.Top + this.Rect.Height))
                    {
                        this._handle = comboboxHandle;
                        break;
                    }
                    else
                    {
                        comboboxHandle = Win32API.FindWindowEx(TestBrowser.IEServerHandle, comboboxHandle, "Internet Explorer_TridentLstBox", null);
                    }
                }

                if (this._handle == IntPtr.Zero)
                {
                    throw new CanNotBuildObjectException("Can not get windows handle of list box.");
                }
            }
            catch (CanNotBuildObjectException)
            {
                throw;
            }
            catch
            {
                throw new CanNotBuildObjectException("Can not get windows handle of list box.");
            }

            try
            {
                this._itemHeight = Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.COMBOBOXMSG.CB_GETITEMHEIGHT), 0, 0);
            }
            catch
            {
                this._itemHeight = this.Rect.Height;
            }

        }

        #endregion

        #region public methods

        public virtual void Select(string value)
        {

            //the item index to select. -1 means invalid.
            int index;
            if (String.IsNullOrEmpty(value))
            {
                index = 0;
            }
            else
            {
                index = GetIndexByString(value);
            }

            SelectByIndex(index);

        }

        public virtual void SelectByIndex(int index)
        {
            if (index < 0 || index > _allValues.Length)
            {
                throw new CanNotPerformActionException("Invalid item index: " + index.ToString());
            }



        }

        public virtual void Focus()
        {
            Hover();
            MouseOp.Click();
        }

        public virtual object GetDefaultAction()
        {
            return "Select";
        }

        public virtual void PerformDefaultAction()
        {
            SelectByIndex(0);
        }

        public virtual string GetText()
        {
            return this._selectedValue;
        }

        public virtual string GetFontStyle()
        {
            return null;
        }

        public virtual string GetFontFamily()
        {
            return null;
        }

        public virtual IntPtr GetHandle()
        {
            return this._handle;
        }
        public virtual string GetClass()
        {
            return "Internet Explorer_TridentCmboBx";
        }

        public virtual String[] GetAllValues()
        {
            string[] values = new string[_htmlSelectClass.length];
            try
            {
                HTMLOptionElementClass optionClass;
                for (int i = 0; i < _htmlSelectClass.length; i++)
                {
                    optionClass = (HTMLOptionElementClass)_htmlSelectClass.item(i, i);
                    values[i] = optionClass.innerText;
                }
                return values;
            }
            catch
            {
                throw;
            }

        }

        #endregion

        #region private methods

        protected virtual string GetSelectedValue()
        {
            if (this._allValues == null)
            {
                this._allValues = GetAllValues();
            }
            try
            {
                return this._allValues[_htmlSelectClass.selectedIndex];
            }
            catch
            {
                return "";
            }
        }

        protected virtual int GetIndexByString(string value)
        {
            string tmp = value.ToUpper();

            for (int i = 0; i < this._allValues.Length; i++)
            {
                if (_allValues[i].ToUpper() == tmp)
                {
                    return i;
                }
            }

            throw new ItemNotFoundException("Can not get item:" + value);
        }

        protected virtual Point GetItemPosition(int index)
        {

            int topIndex = GetTopIndex();

            if (topIndex < 0 || topIndex >= this._allValues.Length)
            {
                throw new ItemNotFoundException("Can not find the position of item at index:" + index.ToString());
            }



            int itemX = 0;
            int itemY = 0;


            return new Point(itemX, itemY);
        }

        protected virtual int GetTopIndex()
        {
            try
            {
                return Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.COMBOBOXMSG.CB_GETTOPINDEX), 0, 0);
            }
            catch
            {
                throw new PropertyNotFoundException("Can not get the first visible item.");
            }
        }
        #endregion

        #endregion


    }
}
