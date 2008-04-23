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
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestComboBox : MSAATestGUIObject, ISelectable, IShowInfo
    {

        #region fields

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
        }

        #endregion

        #region public methods

        #region ISelectable Members

        public void Select(string str)
        {
            throw new NotImplementedException();
        }

        public void SelectMulti(string[] strs)
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
                int childrenCount = MSAATestObject.GetChildCount(this._iAcc, (int)this._selfID);

                for (int i = 0; i < childrenCount; i++)
                {
                    //there are some children for combobox.
                    //one child's role text may "list", it contains the sub items.
                    String role = MSAATestObject.GetRole(this._iAcc, i);

                    if (String.Compare(role, "list", true) == 0)
                    {
                        IAccessible listIAcc = MSAATestObject.GetChild(this._iAcc, i);

                        int itemCount = MSAATestObject.GetChildCount(listIAcc, 0);

                        String[] items = new String[itemCount];

                        for (int j = 0; j < itemCount; j++)
                        {
                            items[j] = MSAATestObject.GetName(listIAcc, j);
                        }

                        return items;
                    }
                }

                throw new ItemNotFoundException("Can not get items of combo box.");
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ItemNotFoundException("Can not get items of combo box: " + ex.Message);
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
            return "Check";
        }

        public void DoAction(object parameter)
        {
            try
            {
                int index = 0;

                if (parameter != null)
                {
                    string value = parameter.ToString();

                    if (int.TryParse(value, out index))
                    {
                        SelectByIndex(index);
                    }
                    else
                    {
                        Select(value);
                    }
                }

                SelectByIndex(index);

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform action: " + ex.Message);
            }
        }

        #endregion

        #region IShowInfo Members

        public string GetText()
        {
            return MSAATestObject.GetValue(this._iAcc, Convert.ToInt32(this._selfID));
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

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
