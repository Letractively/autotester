/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestListBox.cs
*
* Description: This class defines the actions provide by Listbox.
*              the important actions include "Select" and "SelectByIndex"
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using mshtml;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestListBox : HTMLGuiTestObject, ISelectable, IWindows
    {
        #region fields

        //the item is selected.
        protected string _selectedValue;
        //all values of the listbox
        protected string[] _allValues;

        //size of listbox. if it has more items than the size, we can see scroll bar.
        protected int _itemCountPerPage;

        //height of each item.
        protected int _itemHeight;

        //handle for listbox, listbox is a windows control
        protected IntPtr _handle;

        protected HTMLSelectElementClass _htmlSelectClass;

        #endregion

        #region properties

        public int ItemCountPerPage
        {
            get { return _itemCountPerPage; }

        }

        public string SelectedValue
        {
            get { return _selectedValue; }

        }

        protected int ItemHeight
        {
            get { return _itemHeight; }
        }


        #endregion

        #region methods

        #region ctor

        public HTMLTestListBox(IHTMLElement element)
            : base(element)
        {
            try
            {
                _htmlSelectClass = (HTMLSelectElementClass)element;
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
                this._itemCountPerPage = int.Parse(element.getAttribute("size", 0).ToString());
            }
            catch
            {
                throw new CanNotBuildObjectException("Can not get size of list box.");
            }
            try
            {

                //get the windows handle
                IntPtr listboxHandle = Win32API.FindWindowEx(TestBrowser.IEServerHandle, IntPtr.Zero, "Internet Explorer_TridentLstBox", null);
                while (listboxHandle != IntPtr.Zero)
                {
                    // get the rect of this control
                    Win32API.Rect tmpRect = new Win32API.Rect();
                    Win32API.GetWindowRect(listboxHandle, ref tmpRect);
                    int centerX = (tmpRect.right - tmpRect.left) / 2 + tmpRect.left;
                    int centerY = (tmpRect.bottom - tmpRect.top) / 2 + tmpRect.top;

                    //if the control has the same position with our HTML test object, that means we find it.
                    if ((centerX > this.Rect.Left && centerX < this.Rect.Left + this.Rect.Width) && (centerY > this.Rect.Top && centerY < this.Rect.Top + this.Rect.Height))
                    {
                        this._handle = listboxHandle;

                        break;
                    }
                    else
                    {
                        //else, go to next listbox
                        listboxHandle = Win32API.FindWindowEx(TestBrowser.IEServerHandle, listboxHandle, "Internet Explorer_TridentLstBox", null);
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
                this._itemHeight = Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.LISTBOXMSG.LB_GETITEMHEIGHT), 0, 0);
            }
            catch
            {
                if (this.ItemCountPerPage < 1)
                {
                    this._itemHeight = this.Rect.Height;
                }
                else
                {
                    this._itemHeight = this.Rect.Height / this.ItemCountPerPage;
                }
            }
        }

        #endregion

        #region public methods
        public virtual void Select(string value)
        {
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
            if (this._allValues == null)
            {
                this._allValues = GetAllValues();
            }

            if (index < 0)
            {
                index = 0;
            }
            else if (index > this._allValues.Length)
            {
                index = this._allValues.Length - 1;
            }

            try
            {
                _actionFinished.WaitOne();

                Hover();

                Point itemPosition = GetItemPosition(index);

                MouseOp.Click(itemPosition.X, itemPosition.Y);

                this._selectedValue = this._allValues[index];

                _actionFinished.Set();

            }
            catch (ItemNotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CanNotPerformActionException(e.ToString());
            }

        }

        public String[] GetAllValues()
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

        public virtual IntPtr GetHandle()
        {
            return this._handle;
        }
        public virtual string GetClass()
        {
            return "Internet Explorer_TridentLstBox";
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

        #endregion

        #region private methods

        //protected virtual int GetSelectedIndex()
        //{
        //    return _htmlSelectClass.selectedIndex;
        //}

        protected string GetSelectedValue()
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

        protected virtual Point GetItemPosition(int index)
        {

            int startIndex = GetTopIndex();

            if (startIndex < 0 || startIndex >= this._allValues.Length)
            {
                throw new ItemNotFoundException("Can not get the position of item at index: " + ToString());
            }

            int positionFlag = -1; //0 for visible, 1 for smaller than TopIndex,2 for larger than LastIndex.

            if (index < startIndex)
            {
                positionFlag = 1;
            }
            else
            {
                if (index > startIndex + this._itemCountPerPage)
                {
                    positionFlag = 2;
                }
                else
                {
                    positionFlag = 0;
                }
            }

            if (positionFlag == -1)
            {
                throw new ItemNotFoundException("Can not get item: " + index.ToString());
            }

            int itemX = this.Rect.Width / 3 + this.Rect.Left;
            int itemY = this.Rect.Top + this.ItemHeight / 2;

            if (positionFlag == 0)
            {
                itemY += (index - startIndex) * this.ItemHeight;
            }
            else if (positionFlag == 1)
            {
                Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.LISTBOXMSG.LB_SETTOPINDEX), index, 0);
            }
            else if (positionFlag == 2)
            {
                int expectedStartIndex = index - this._itemCountPerPage + 1;
                Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.LISTBOXMSG.LB_SETTOPINDEX), expectedStartIndex, 0);

                itemY += (this._itemCountPerPage - 1) * this.ItemHeight;
            }

            return new Point(itemX, itemY);

        }

        protected virtual int GetTopIndex()
        {
            try
            {
                return Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.LISTBOXMSG.LB_GETTOPINDEX), 0, 0);
            }
            catch
            {
                throw new PropertyNotFoundException("Can not get the first visible item.");
            }

        }

        //protected virtual int GetItemCountPerPage()
        //{
        //    return this._itemCountPerPage;
        //}

        protected virtual int GetIndexByString(string value)
        {
            if (this._allValues == null)
            {
                this._allValues = GetAllValues();
            }


            for (int i = 0; i < this._allValues.Length; i++)
            {
                if (String.Compare(_allValues[i], value, true) == 0)
                {
                    return i;
                }
            }

            throw new ItemNotFoundException("Can not find item: " + value);

        }

        protected override void HighLightRectCallback(object obj)
        {
            base.HighLightRect(true);
        }

        #endregion

        #endregion



    }

}
