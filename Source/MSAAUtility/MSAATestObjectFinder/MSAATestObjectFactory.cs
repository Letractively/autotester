using System;
using System.Collections.Generic;
using System.Text;

using Accessibility;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public sealed class MSAATestObjectFactory
    {
        #region fields

        private static Dictionary<String, MSAATestObject.RoleType[]> _objectTypeTable = new Dictionary<string, MSAATestObject.RoleType[]>(37);

        #endregion

        #region methods

        #region ctor

        static MSAATestObjectFactory()
        {
            Init();
        }

        private static void Init()
        {
            _objectTypeTable.Add("BUTTON", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.PushButton, MSAATestObject.RoleType.SpinButton, MSAATestObject.RoleType.SplitButton });
            _objectTypeTable.Add("LABEL", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.StaticText });
            _objectTypeTable.Add("TEXTBOX", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.Text });
            _objectTypeTable.Add("LINK", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.Link });
            _objectTypeTable.Add("IMAGE", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.Graphic });
            _objectTypeTable.Add("COMBOBOX", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.ComboBox });
            _objectTypeTable.Add("LISTBOX", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.List });
            _objectTypeTable.Add("RADIOBOX", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.RadioButton });
            _objectTypeTable.Add("RADIOBUTTON", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.RadioButton });
            _objectTypeTable.Add("CHECKBOX", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.CheckButton });
            _objectTypeTable.Add("CHECKBUTTON", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.CheckButton });
            _objectTypeTable.Add("TABLE", new MSAATestObject.RoleType[] { MSAATestObject.RoleType.Table });
        }

        #endregion

        public static MSAATestObject BuildObject(IAccessible iAcc, int childID)
        {
            String type = GetObjectType(iAcc, childID);
            return BuildObjectByType(iAcc, childID, type);
        }

        public static MSAATestObject BuildObjectByType(IAccessible iAcc, int childID, String type)
        {
            if (type == MSAATestObjectType.Button)
            {
                return new MSAATestButton(iAcc, childID);
            }
            else if (type == MSAATestObjectType.TextBox)
            {
                return new MSAATestTextBox(iAcc, childID);
            }
            else if (type == MSAATestObjectType.CheckBox)
            {
                return new MSAATestCheckBox(iAcc, childID);
            }
            else if (type == MSAATestObjectType.RadioBox)
            {
                return new MSAATestRadioButton(iAcc, childID);
            }
            else if (type == MSAATestObjectType.ComboBox)
            {
                return new MSAATestComboBox(iAcc, childID);
            }

            return new MSAATestObject(iAcc, childID);
        }

        /* MSAATestObject.Type GetObjectType(IAccessible iAcc) 
         * return the acc object type.
         */
        public static String GetObjectType(IAccessible iAcc, int childID)
        {
            if (iAcc != null && childID >= 0)
            {
                MSAATestObject.RoleType role = MSAATestObject.GetRole(iAcc, childID);
                string action = MSAATestObject.GetDefAction(iAcc, childID);
                string state = MSAATestObject.GetState(iAcc, childID);

                if (role != MSAATestObject.RoleType.None)
                {
                    if (role == MSAATestObject.RoleType.PushButton || role == MSAATestObject.RoleType.SplitButton || role == MSAATestObject.RoleType.SpinButton)
                    {
                        return MSAATestObjectType.Button;
                    }
                    else if (role == MSAATestObject.RoleType.Text)
                    {
                        if (String.IsNullOrEmpty(action))
                        {
                            if (state.IndexOf("read only", StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                return MSAATestObjectType.Label;
                            }
                        }
                        else
                        {
                            if (action.IndexOf("Jump", StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                return MSAATestObjectType.Link;
                            }
                        }

                        return MSAATestObjectType.TextBox;
                    }
                    else if (role == MSAATestObject.RoleType.ComboBox || role == MSAATestObject.RoleType.DropList)
                    {
                        return MSAATestObjectType.ComboBox;
                    }
                    else if (role == MSAATestObject.RoleType.RadioButton)
                    {
                        return MSAATestObjectType.RadioBox;
                    }
                    else if (role == MSAATestObject.RoleType.CheckButton)
                    {
                        return MSAATestObjectType.CheckBox;
                    }
                    else if (role == MSAATestObject.RoleType.StaticText)
                    {
                        return MSAATestObjectType.Label;
                    }
                    else if (role == MSAATestObject.RoleType.Table)
                    {
                        return MSAATestObjectType.Table;
                    }
                    else if (role == MSAATestObject.RoleType.Graphic)
                    {
                        return MSAATestObjectType.Image;
                    }
                }

                if (!String.IsNullOrEmpty(action))
                {
                    if (action.IndexOf("Press", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return MSAATestObjectType.Button;
                    }
                    else if (action.IndexOf("Collapse", StringComparison.CurrentCultureIgnoreCase) >= 0 || action.IndexOf("Expand", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return MSAATestObjectType.Tree;
                    }
                }
            }

            return MSAATestObjectType.Unknown;
        }

        public static MSAATestObject.RoleType[] GetMSAATypeByString(string type)
        {
            if (!String.IsNullOrEmpty(type))
            {
                type = type.ToUpper();
                if (_objectTypeTable.ContainsKey(type))
                {
                    return _objectTypeTable[type];
                }
                else
                {
                    return new MSAATestObject.RoleType[] { MSAATestObject.GetRole(type) };
                }
            }

            return new MSAATestObject.RoleType[] { MSAATestObject.RoleType.None };
        }

        #region private


        #endregion

        #endregion
    }
}
