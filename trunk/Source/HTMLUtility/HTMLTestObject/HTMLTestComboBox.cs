/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestComboBox.cs
*
* Description: This class define the actions provide by Combobox. 
*              the important actions include "Select" and "SelectByIndex"
* 
* History: 2007/09/04 wan,yu Init version
*          2007/01/09 wan,yu bug fix, change the _htmlSelectClass to
*                                                _htmlSelectElement 
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestComboBox : HTMLGuiTestObject, ISelectable, IWindows
    {

        #region fields

        //HTML combobox is NOT a HTML control, it is a standard windows control, so we can  get it's handle.
        protected IntPtr _handle;

        //all items.
        protected string[] _allValues;

        //current selected value.
        protected string _selectedValue;

        //how many items per page, means you can see these items without move the scrollbar
        protected int _itemCountPerPage = 30;

        //height of each item.
        protected int _itemHeight;

        //HTML element.
        protected IHTMLSelectElement _htmlSelectElement;
        //protected HTMLSelectElementClass _htmlSelectClass;

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
                _htmlSelectElement = (IHTMLSelectElement)element;
                //_htmlSelectClass = (HTMLSelectElementClass)element;
            }
            catch
            {
                throw new CannotBuildObjectException("HTML source element can not be null.");
            }
            try
            {
                this._allValues = this.GetAllValues();
            }
            catch
            {
                throw new CannotBuildObjectException("Can not get the list values.");
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
                this._class = "Internet Explorer_TridentCmboBx";

                //find the windows handle of combo box, it's class name is "Internet Explorer_TridentCmboBx".
                IntPtr comboboxHandle = Win32API.FindWindowEx(TestBrowser.IEServerHandle, IntPtr.Zero, this._class, null);
                while (comboboxHandle != IntPtr.Zero)
                {
                    //get the position of the control
                    Win32API.Rect tmpRect = new Win32API.Rect();
                    Win32API.GetWindowRect(comboboxHandle, ref tmpRect);
                    int centerX = (tmpRect.right - tmpRect.left) / 2 + tmpRect.left;
                    int centerY = (tmpRect.bottom - tmpRect.top) / 2 + tmpRect.top;

                    //we compare the position of the Windows control and HTML object. if they have same position, that means we find it.
                    if ((centerX > this.Rect.Left && centerX < this.Rect.Left + this.Rect.Width) && (centerY > this.Rect.Top && centerY < this.Rect.Top + this.Rect.Height))
                    {
                        this._handle = comboboxHandle;
                        break;
                    }
                    else
                    {
                        //not found, go to next control
                        comboboxHandle = Win32API.FindWindowEx(TestBrowser.IEServerHandle, comboboxHandle, this._class, null);
                    }
                }
            }
            catch
            {

            }

            //get the height of each item.
            this._itemHeight = Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.COMBOBOXMSG.CB_GETITEMHEIGHT), 0, 0);

            if (this._itemHeight == 0)
            {
                //the default item height
                this._itemHeight = 13;
            }

        }

        #endregion

        #region public methods

        /* void Select(string value)
         * select an item by text.
         */
        public virtual void Select(string value)
        {

            //the item index to select. -1 means invalid.
            int index;

            //if nothing input, then select the 1st item.
            if (String.IsNullOrEmpty(value))
            {
                index = 0;
            }
            else
            {
                // get the index of the text.
                index = GetIndexByString(value);
            }

            SelectByIndex(index);

        }

        /* void SelectByIndex(int index)
         * Select the item by it's index.
         */
        public virtual void SelectByIndex(int index)
        {
            //if the index is less than 0 or larger than the item count, invalid.
            if (index < 0 || index > _allValues.Length)
            {
                throw new CannotPerformActionException("Invalid item index: " + index.ToString());
            }

            try
            {


                //get the position on the screen 
                Point itemPosition = GetItemPosition(index);

                //click the object.
                Click();

                //sleep for 0.5 second, make sure the user can see this action.
                //if no sleep, the the action may too fast.
                System.Threading.Thread.Sleep(500 * 1);

                _actionFinished.WaitOne();

                //click on the actual item.
                MouseOp.Click(itemPosition.X, itemPosition.Y);

                //refresh the selected value.
                this._selectedValue = _allValues[index];

                _actionFinished.Set();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not perform select action on Combobox: " + e.ToString());
            }

        }

        #region IInteractive methods

        public virtual void Focus()
        {
            Hover();
            MouseOp.Click();
        }

        public virtual object GetDefaultAction()
        {
            return "Select";
        }

        public virtual void PerformDefaultAction(object para)
        {
            int index;
            if (int.TryParse(para.ToString(), out index))
            {
                SelectByIndex(index);
            }
            else
            {
                Select(para.ToString());
            }
        }

        #endregion


        #region IShowInfo methods

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

        #region IWindows methods

        public virtual IntPtr GetHandle()
        {
            return this._handle;
        }
        public virtual string GetClass()
        {
            return this._class;
        }

        #endregion

        #region IContainer methods

        /*  public String[] GetAllValues()
         *  get all values of combo box, the value means the text we seen.
         */
        public String[] GetAllValues()
        {
            string[] values = new string[_htmlSelectElement.length];
            IHTMLOptionElement optionElement;

            try
            {
                for (int i = 0; i < _htmlSelectElement.length; i++)
                {
                    optionElement = (IHTMLOptionElement)_htmlSelectElement.item(i, i);
                    values[i] = optionElement.text;
                }

                return values;
            }
            catch
            {
                throw;
            }

        }

        #endregion

        #endregion

        #region private methods

        /* string GetSelectedValue()
         * return the current selected value.
         */
        protected string GetSelectedValue()
        {
            if (this._allValues == null)
            {
                this._allValues = GetAllValues();
            }
            try
            {

                return this._allValues[_htmlSelectElement.selectedIndex];
            }
            catch
            {
                return "";
            }
        }

        /* int GetIndexByString(string value)
         * return the index of an expected string.
         */
        protected virtual int GetIndexByString(string value)
        {

            for (int i = 0; i < this._allValues.Length; i++)
            {
                if (String.Compare(value, _allValues[i], true) == 0)
                {
                    return i;
                }
            }

            throw new ItemNotFoundException("Can not get item:" + value);
        }

        /* Point GetItemPosition(int index)
         * Get the position on screen of an expected item.
         */
        protected virtual Point GetItemPosition(int index)
        {

            //get current top index, top index means the first item you can see currently.
            int topIndex = GetTopIndex();

            if (topIndex < 0 || topIndex >= this._allValues.Length)
            {
                throw new ItemNotFoundException("Can not find the position of item at index:" + index.ToString());
            }


            //find the center point of the first item.
            int itemX = this._rect.Left + this._rect.Width / 2;

            int itemY = this._rect.Top;

            //var to indicate if scroll bar exist.
            bool isScrollBar = false;

            //if the combo box contains more than 30 items, we can see scroll bar.
            if (this._allValues.Length > 30)
            {
                //isScrollBar = true;
            }

            bool isDownward = true;

            int listHeight = 0;

            if (this._allValues.Length > 30)
            {
                listHeight = this._itemHeight * 30;
            }
            else
            {
                listHeight = this._itemHeight * this._allValues.Length;
            }

            if (this._rect.Top + listHeight > TestBrowser.ClientHeight)
            {
                isDownward = false;
            }

            //if scrollbar not exist.
            if (!isScrollBar)
            {
                //we don't need to move the scroll bar.
                if (isDownward)
                {
                    itemY += this._rect.Height + this._itemHeight / 2;
                    itemY += this._itemHeight * index;
                }
                else
                {
                    itemY -= listHeight;
                    itemY += this._itemHeight / 2;
                    itemY += this._itemHeight * (index - topIndex);
                }
            }
            else
            {
                //we need to move the scroll bar.

            }


            return new Point(itemX, itemY);
        }

        /* int GetTopIndex()
         * get the index of the first item we can see currently.
         */
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

        /* void Click()
         * Click the combo box.
         * For a combo box, we don't click it's center point.
         * we click on it's right side to make it pop up the drop down list.
         */
        protected virtual void Click()
        {
            _actionFinished.WaitOne();

            Hover();

            //get the right point
            int right = this._rect.Left + this._rect.Width;
            int height = this._rect.Height;

            int x = right - height / 2;
            int y = this.Rect.Top + height / 2;

            MouseOp.Click(x, y);

            _actionFinished.Set();
        }

        /*  void HighLightRectCallback(object obj)
         *  Combo box is a standard windows control, so we need to override this function.
         */
        protected override void HighLightRectCallback(object obj)
        {
            base.HighLightRect(true);
        }

        #endregion

        #endregion


    }
}
