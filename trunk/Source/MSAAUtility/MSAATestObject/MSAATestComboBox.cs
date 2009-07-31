/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MSAATestComboBox.cs
*
* Description: This class define the combo box object for MSAA.
*
* History: 2008/04/23 wan,yu init version.
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestComboBox : MSAATestGUIObject, IComboBox, IText
    {

        #region fields

        protected String _selectedItem;
        protected String[] _items;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public MSAATestComboBox(IAccessible iAcc)
            : this(iAcc, 0)
        {
        }

        public MSAATestComboBox(IAccessible iAcc, int childID)
            : base(iAcc, childID)
        {
            try
            {
                _items = GetAllValues();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build combo box: " + ex.ToString());
            }
        }

        public MSAATestComboBox(IntPtr handle)
            : base(handle)
        {
            try
            {
                _items = GetAllValues();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build combo box: " + ex.ToString());
            }
        }

        #endregion

        #region public methods

        #region ISelectable Members

        public void Select(string str)
        {
            try
            {
                if (String.IsNullOrEmpty(str))
                {
                    throw new CannotPerformActionException("Value is empty.");
                }

                if (_items == null || _items.Length < 1)
                {
                    _items = GetAllValues();
                }

                if (_items == null || _items.Length < 1)
                {
                    throw new CannotPerformActionException("Can not get sub items.");
                }

                bool found = false;
                int index = 0;
                for (index = 0; index < _items.Length; index++)
                {
                    String curItem = _items[index];

                    if (String.Compare(str, curItem, true) == 0)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    SelectByIndex(index);
                }
                else
                {
                    throw new CannotPerformActionException("Value: " + str + " is not existed.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not select by value: " + ex.ToString());
            }
        }

        public void MultiSelect(string[] strs)
        {
            throw new NotImplementedException();
        }

        public void MultiSelectByIndex(int[] items)
        {
            throw new NotImplementedException();
        }

        public void SelectByIndex(int index)
        {
            throw new NotImplementedException();
        }

        public string[] GetAllValues()
        {
            try
            {
                int childrenCount = MSAATestObject.GetChildCount(this.IAcc, (int)this.ChildID);

                for (int i = 0; i < childrenCount; i++)
                {
                    //there are some children for combobox.
                    //one child's role text may "list", it contains the sub items.
                    MSAATestObject.RoleType role = MSAATestObject.GetRole(this.IAcc, i);

                    if (role == RoleType.List)
                    {
                        IAccessible listIAcc = MSAATestObject.GetChildIAcc(this.IAcc, i);

                        int itemCount = MSAATestObject.GetChildCount(listIAcc, 0);

                        String[] items = new String[itemCount];

                        for (int j = 0; j < itemCount; j++)
                        {
                            items[j] = MSAATestObject.GetName(listIAcc, j);
                        }

                        return items;
                    }
                }

                return null;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ItemNotFoundException("Can not get items of combo box: " + ex.ToString());
            }
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetAction()
        {
            return "Select";
        }

        public void DoAction(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    string value = parameter.ToString();
                    int index = 0;
                    if (int.TryParse(value, out index))
                    {
                        SelectByIndex(index);
                    }
                    else
                    {
                        Select(value);
                    }
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform action: " + ex.ToString());
            }
        }

        #endregion

        #region IText Members

        public string GetText()
        {
            return GetValue();
        }

        public override string GetLabel()
        {
            return GetValue();
        }

        public string GetFontFamily()
        {
            throw new Exception("The method or operation is not implemented.");
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


        #region IInputable Members

        public void Input(string values)
        {
            if (!_sendMsgOnly)
            {
                Hover();
                MouseOp.Click();
                KeyboardOp.SendChars(values);
            }
            else
            {
                IntPtr handle = GetHandle();
                if (handle == IntPtr.Zero)
                {
                    SetProperty("value", values.ToString());
                }
                else
                {
                    List<IntPtr> textBox = WindowsAsstFunctions.GetChildren(handle);
                    if (textBox != null && textBox.Count > 0)
                    {
                        String className = Win32API.GetClassName(textBox[0]);
                        if (String.Compare(className, "Edit", true) == 0)
                        {
                            MSAATestObject obj = new MSAATestObject(textBox[0]);
                            obj.SetProperty("value", values.ToString());
                        }
                    }
                }
            }
        }

        public void InputKeys(string keys)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private methods

        protected override void GetMSAAInfo()
        {
            this._type = MSAATestObjectType.ComboBox;
            base.GetMSAAInfo();
        }

        #endregion

        #endregion
    }
}
