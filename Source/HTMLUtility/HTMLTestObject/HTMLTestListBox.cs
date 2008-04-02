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
*          2008/01/10 wan,yu update, change _htmlSelectClass to _htmlSelectElement
*          2008/01/14 wan,yu update, add SelectMulti method to select more than 1 item in listbox. 
*
*********************************************************************/

using System;
using System.Drawing;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    //HTMLTestListBox is NOT a HTML control, it is a standard windows control.
    public class HTMLTestListBox : HTMLTestGUIObject, ISelectable, IWindows
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
        protected string _className;

        protected IHTMLSelectElement _htmlSelectElement;
        //protected HTMLSelectElementClass _htmlSelectClass;

        //if the flag is true, means the listbox can select more than 1 item.
        protected bool _isMultiple = false;


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

        public bool IsMultiple
        {
            get { return _isMultiple; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestListBox(IHTMLElement element)
            : base(element)
        {

            this._type = HTMLTestObjectType.ListBox;

            this._isDelayAfterAction = false;

            try
            {
                _htmlSelectElement = (IHTMLSelectElement)element;

                _isMultiple = _htmlSelectElement.multiple;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("HTML source element can not be null: " + ex.Message);
            }

            try
            {
                this._allValues = this.GetAllValues();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get the list values: " + ex.Message);
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
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get size of list box: " + ex.Message);
            }

            try
            {
                this._className = "Internet Explorer_TridentLstBox";

                //get the windows handle
                IntPtr listboxHandle = Win32API.FindWindowEx(_browser.IEServerHandle, IntPtr.Zero, this._className, null);
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
                        listboxHandle = Win32API.FindWindowEx(_browser.IEServerHandle, listboxHandle, this._className, null);
                    }
                }

                //if (this._handle == IntPtr.Zero)
                //{
                //    throw new CannotBuildObjectException("Can not get windows handle of list box.");
                //}
            }
            //catch (CannotBuildObjectException)
            //{
            //    throw;
            //}
            catch //(Exception ex)
            {
                // throw new CannotBuildObjectException("Can not get windows handle of list box: " + ex.Message);
            }

            try
            {
                //get the height of each item.
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

        public override void Hover()
        {
            if (!_isEnable || !_isVisible || _isReadOnly)
            {
                throw new CannotPerformActionException("Listbox is not enabled.");
            }

            base.Hover();
        }

        /* void Select(string value)
        *  Select an item by text.
        *  This method will get the index by text, and call SelectByIndex method to perform action.
        */
        public virtual void Select(string value)
        {
            int index;

            //if input text is null, select the frist one.
            if (String.IsNullOrEmpty(value))
            {
                index = 0;
            }
            else
            {
                //get the actual index in the listbox items.
                index = GetIndexByString(value);
            }

            SelectByIndex(index);

        }

        /* void SelectByIndex(int index)
         * select an item by index.
         */
        public virtual void SelectByIndex(int index)
        {
            if (this._allValues == null)
            {
                this._allValues = GetAllValues();
            }

            //check the index, if the index is smaller than 0 or larger than the capacity.
            // set it to 0 or the last index.
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

                if (_sendMsgOnly)
                {
                    this._htmlSelectElement.selectedIndex = index;
                }
                else
                {
                    //get the actual position on the screen.
                    Point itemPosition = GetItemPosition(index);

                    MouseOp.Click(itemPosition);
                }

                //refresh the selected value.
                this._selectedValue = this._allValues[index];

                if (_isDelayAfterAction)
                {
                    System.Threading.Thread.Sleep(_delayTime * 1000);
                }

                _actionFinished.Set();

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not select by index: " + index.ToString() + ": " + ex.Message);
            }

        }

        /* void SelectMulti(string[] values)
         * Select more than 1 items.
         */
        public virtual void SelectMulti(string[] values)
        {
            if (!_isMultiple)
            {
                throw new CannotPerformActionException("Can not select multi items.");
            }

            if (values == null)
            {
                throw new CannotPerformActionException("Can not select in listbox, no item.");
            }

            if (this._allValues == null)
            {
                this._allValues = GetAllValues();
            }

            //firstly, we need to get index for each text.
            int[] index = GetIndexByString(values);

            try
            {
                _actionFinished.WaitOne();

                Hover();

                Point itemPosition;

                //click each item.
                foreach (int itemIndex in index)
                {
                    //get the actual position on the screen.
                    itemPosition = GetItemPosition(itemIndex);

                    MouseOp.Click(itemPosition);

                    //refresh the selected value.
                    this._selectedValue = this._allValues[itemIndex];
                }

                if (_isDelayAfterAction)
                {
                    System.Threading.Thread.Sleep(_delayTime * 1000);
                }

                _actionFinished.Set();

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not select multi stings: " + values.ToString() + ": " + ex.Message);
            }
        }

        /* String[] GetAllValues()
         * Return the all items text of the listbox
         */
        public String[] GetAllValues()
        {

            string[] values = new string[_htmlSelectElement.length];

            // 2008/01/10 wan,yu update change HTMLOptionElementClass to IHTMLOptionElement
            IHTMLOptionElement optionElement;
            //HTMLOptionElementClass optionClass;

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

        #region IWindows Interface

        public virtual IntPtr GetHandle()
        {
            return this._handle;
        }

        public virtual string GetClass()
        {
            return this._className;
        }

        #endregion

        #region IInteractive interface

        public virtual void Focus()
        {
            Hover();
            MouseOp.Click();
        }

        public virtual string GetAction()
        {
            return "Select";
        }

        public virtual void DoAction(object para)
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

        public virtual string GetText()
        {
            return this._selectedValue;
        }

        public virtual string GetFontFamily()
        {
            return null;
        }

        public string GetFontSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontColor()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region private methods

        /* int GetSelectedIndex()
         * get the index of selected value.
         */
        protected virtual int GetSelectedIndex()
        {
            return _htmlSelectElement.selectedIndex;
        }

        /* string GetSelectedValue()
         * return the selected value.
         */
        protected virtual string GetSelectedValue()
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

        /* Point GetItemPosition(int index)
         * Get the actual position on screen of the expected item index.
         */
        protected virtual Point GetItemPosition(int index)
        {
            //get the the first item index we can  see currently.
            int startIndex = GetTopIndex();

            //if the index is smaller than 0 or larger than capacity
            if (startIndex < 0 || startIndex >= this._allValues.Length)
            {
                throw new ItemNotFoundException("Can not get the position of item at index: " + index.ToString());
            }

            //0 for visible, 1 for smaller than TopIndex,2 for larger than LastIndex.
            //if 0, we can see it directly without move the scroll bar.
            //if 1, we need to move the scroll bar upward to see it.
            //if 2, we need to move the scroll bar downward to see it.
            int positionFlag = -1;

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

            //find the position of the first visible item.
            int itemX = this._rect.Width / 3 + this._rect.Left;
            int itemY = this._rect.Top + this._itemHeight / 2;

            if (positionFlag == 0)
            {
                //currently, we can see it, just click it.
                itemY += (index - startIndex) * this._itemHeight;
            }
            else if (positionFlag == 1)
            {
                //we need to move the scroll bar upward.
                Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.LISTBOXMSG.LB_SETTOPINDEX), index, 0);
            }
            else if (positionFlag == 2)
            {
                //we need to move the scroll bar downward, and recalculate the position.
                int expectedStartIndex = index - this._itemCountPerPage + 1;
                Win32API.SendMessage(this._handle, Convert.ToInt32(Win32API.LISTBOXMSG.LB_SETTOPINDEX), expectedStartIndex, 0);

                itemY += (this._itemCountPerPage - 1) * this._itemHeight;
            }

            return new Point(itemX, itemY);

        }

        /* int GetTopIndex()
         * return the first item index we can see currently.
         */
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

        /* int[] GetIndexByString(string[] value)
         * get the index of expected text.
         */
        protected virtual int GetIndexByString(string value)
        {
            return GetIndexByString(new string[1] { value })[0];
        }

        protected virtual int[] GetIndexByString(string[] values)
        {
            if (this._allValues == null)
            {
                this._allValues = GetAllValues();
            }

            if (values == null)
            {
                throw new ItemNotFoundException("Can not find item: values can not be empty.");
            }

            int[] res = new int[values.Length];

            for (int j = 0; j < values.Length; j++)
            {
                bool found = false;

                for (int i = 0; i < this._allValues.Length; i++)
                {
                    if (String.Compare(_allValues[i], values[j], true) == 0)
                    {
                        res[j] = i;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new ItemNotFoundException("Can not find item: " + values[j]);
                }
            }

            return res;

        }

        /* void HighLightRectCallback(object obj)
         * HTMLTestListBox is NOT a HTML object, it is a standard Windwos control.
         * So we need to override this function.
         */
        protected override void HighLightRectCallback(object obj)
        {
            base.HighLightRect(true);
        }

        #endregion

        #endregion



    }

}
