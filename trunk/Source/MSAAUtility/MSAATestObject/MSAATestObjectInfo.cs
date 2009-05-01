using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Accessibility;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    //this partial class defines some helper method to get information from MSAATestObject
    public partial class MSAATestObject
    {
        #region state
        private const int COUNT = 34;
        private static int[] _stateArr = new int[]{
            0, 1, 2, 4, 8, 16, 32, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192,16384,32768,65536,131072,262144,524288,1048576,
            2097152,4194304,8388608,16777216,33554432, 67108864,134217728, 268435456,536870912,1073741823,1073741824
        };

        private static string[] _desc = new string[]{
            "None","Unavailable","Selected","Focused","Pressed","Checked","Mixed","Indeterminate","ReadOnly","HotTracked","Default",
            "Expanded","Collapsed","Busy","Floating","Marqueed","Animated","Invisible","Offscreen","Sizeable","Moveable","SelfVoicing",
            "Focusable","Selectable","Linked","Traversed","MultiSelectable","ExtSelectable","AlertLow","AlertMedium","AlertHigh",
            "Protected","Valid","HasPopup"
        };

        private static Dictionary<int, string> _stateTable = new Dictionary<int, string>();
        public static string GetStateText(int stateID)
        {
            if (stateID > 0)
            {
                if (_stateTable.ContainsKey(stateID))
                {
                    return _stateTable[stateID];
                }

                bool found = false;

                int left = stateID;
                StringBuilder res = new StringBuilder();

                for (int i = COUNT - 1; i > 0 && left > 0; i--)
                {
                    if (left >= _stateArr[i])
                    {
                        if (found)
                        {
                            res.Append(",");
                        }
                        else
                        {
                            found = true;
                        }

                        res.Append(_desc[i]);
                        left -= _stateArr[i];
                    }
                }

                string res2 = res.ToString();
                _stateTable.Add(stateID, res2);
                return res2;
            }

            return "None";
        }

        #endregion

        #region role

        public enum RoleType
        {
            Default = -1,
            None = 0,
            TitleBar = 1,
            MenuBar = 2,
            ScrollBar = 3,
            Grip = 4,
            Sound = 5,
            Cursor = 6,
            Caret = 7,
            Alert = 8,
            Window = 9,
            Client = 10,
            MenuPopup = 11,
            MenuItem = 12,
            ToolTip = 13,
            Application = 14,
            Document = 15,
            Pane = 16,
            Chart = 17,
            Dialog = 18,
            Border = 19,
            Grouping = 20,
            Separator = 21,
            ToolBar = 22,
            StatusBar = 23,
            Table = 24,
            ColumnHeader = 25,
            RowHeader = 26,
            Column = 27,
            Row = 28,
            Cell = 29,
            Link = 30,
            HelpBalloon = 31,
            Character = 32,
            List = 33,
            ListItem = 34,
            Outline = 35,
            OutlineItem = 36,
            PageTab = 37,
            PropertyPage = 38,
            Indicator = 39,
            Graphic = 40,
            StaticText = 41,
            Text = 42,
            PushButton = 43,
            CheckButton = 44,
            RadioButton = 45,
            ComboBox = 46,
            DropList = 47,
            ProgressBar = 48,
            Dial = 49,
            HotkeyField = 50,
            Slider = 51,
            SpinButton = 52,
            Diagram = 53,
            Animation = 54,
            Equation = 55,
            ButtonDropDown = 56,
            ButtonMenu = 57,
            ButtonDropDownGrid = 58,
            WhiteSpace = 59,
            PageTabList = 60,
            Clock = 61,
            SplitButton = 62,
            IpAddress = 63,
            OutlineButton = 64
        };

        private const int ROLES_COUNT = 64;
        private static string[] _roles = new string[]{
        "None","TitleBar","MenuBar","ScrollBar","Grip","Sound","Cursor","Caret","Alert","Window","Client","MenuPopup","MenuItem",
        "ToolTip","Application","Document","Pane","Chart","Dialog","Border","Grouping","Separator","ToolBar","StatusBar","Table",
        "ColumnHeader","RowHeader","Column","Row","Cell","Link","HelpBalloon","Character","List","ListItem","Outline","OutlineItem",
        "PageTab","PropertyPage","Indicator","Graphic","StaticText","Text","PushButton","CheckButton","RadioButton","ComboBox",
        "DropList","ProgressBar","Dial","HotkeyField","Slider","SpinButton","Diagram","Animation","Equation","ButtonDropDown",
        "ButtonMenu","ButtonDropDownGrid","WhiteSpace","PageTabList","Clock","SplitButton","IpAddress","OutlineButton"};

        public static string GetRoleStr(int roleType)
        {
            if (roleType < 0 || roleType > ROLES_COUNT)
            {
                roleType = 0;
            }

            return _roles[roleType];
        }

        public static RoleType GetRole(int roleType)
        {
            return (RoleType)roleType;
        }

        #endregion

        #region type
        public enum Type
        {
            Unknow = 0,
            Label, //display text, readonly
            Button,
            CheckBox, //check/uncheck 
            RadioBox, //in a group, you can only check one.
            TextBox,  //you can input something in it.
            ComboBox, //a dropdown list.
            ListBox,  //provide a list of items, you can select/unselect.
            Table,    //a matrix.
            Image,
            Link,     //navigate to other controls.
            MsgBox, //MsgBox is so common, I treat it as a single control.
            Menu,
            Tab, //MDI, like IE7, you can have more than 1 tab.
            Tree,
            ScrollBar
        }
        #endregion

        #region get information
        public static Object GetProperty(IAccessible iAcc, int childID, String propertyName)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                if (propertyName.StartsWith("."))
                {
                    propertyName.Replace(".", "");
                }

                try
                {
                    if (String.Compare(propertyName, "name", true) == 0)
                    {
                        return iAcc.get_accName(childID);
                    }
                    else if (String.Compare(propertyName, "value", true) == 0)
                    {
                        return iAcc.get_accValue(childID);
                    }
                    else if (String.Compare(propertyName, "role", true) == 0)
                    {
                        return iAcc.get_accRole(childID);
                    }
                    else if (String.Compare(propertyName, "state", true) == 0)
                    {
                        return iAcc.get_accState(childID);
                    }
                    else if (String.Compare(propertyName, "defAction", true) == 0 || propertyName.IndexOf("action", StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                        return iAcc.get_accDefaultAction(childID);
                    }
                    else if (String.Compare(propertyName, "description", true) == 0)
                    {
                        return iAcc.get_accDescription(childID);
                    }
                    else if (String.Compare(propertyName, "help", true) == 0)
                    {
                        return iAcc.get_accHelp(childID);
                    }
                    else if (String.Compare(propertyName, "rect", true) == 0 || String.Compare(propertyName, "location", true) == 0)
                    {
                        //get location.
                        int left, top, width, height;
                        iAcc.accLocation(out left, out top, out width, out height, childID);

                        return new Rectangle(left, top, width, height);
                    }
                    else if (String.Compare(propertyName, "childCount", true) == 0)
                    {
                        return iAcc.accChildCount;
                    }
                    else if (String.Compare(propertyName, "selection", true) == 0)
                    {
                        return iAcc.accSelection;
                    }
                    else if (String.Compare(propertyName, "focus", true) == 0)
                    {
                        return iAcc.accFocus;
                    }
                }
                catch
                {
                }
            }

            return "";

        }


        public static Rectangle GetRect(IAccessible iAcc, int childID)
        {
            try
            {
                //get location.
                int left, top, width, height;
                iAcc.accLocation(out left, out top, out width, out height, (object)childID);

                return new Rectangle(left, top, width, height);
            }
            catch
            {
                return new Rectangle(0, 0, 0, 0);
            }
        }

        public static Point GetCenterPoint(IAccessible iAcc, int childID)
        {
            try
            {
                //get location.
                int left, top, width, height;
                iAcc.accLocation(out left, out top, out width, out height, (object)childID);

                int x = left + width / 2;
                int y = top + height / 2;

                return new Point(x, y);
            }
            catch
            {
                return new Point(0, 0);
            }
        }

        public static RoleType GetRole(IAccessible iAcc, int childID)
        {
            try
            {
                int roleType = (int)iAcc.get_accRole((object)childID);
                return GetRole(roleType);
            }
            catch
            {
                return RoleType.None;
            }
        }

        public static String GetValue(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accValue((object)childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetName(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accName((object)childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetDefAction(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accDefaultAction((object)childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetDesc(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accDescription((object)childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetState(IAccessible iAcc, int childID)
        {
            try
            {
                int stateID = (int)iAcc.get_accState((object)childID);

                return GetStateText(stateID);
            }
            catch
            {
                return "";
            }
        }

        public static IAccessible GetChildIAcc(IAccessible iAcc, int childID)
        {
            try
            {
                IAccessible childIAcc = null;
                try
                {
                    childIAcc = (IAccessible)iAcc.get_accChild((object)childID);
                }
                catch
                {

                }

                if (childIAcc == null)
                {
                    IAccessible[] childrenObj = new IAccessible[1];
                    int count = 0;
                    Win32API.AccessibleChildren(iAcc, childID, 1, childrenObj, out count);

                    childIAcc = childrenObj[0];
                }

                return childIAcc;

            }
            catch
            {
            }

            return null;
        }

        public static IAccessible[] GetChildrenIAcc(IAccessible iAcc)
        {
            int childCount = iAcc.accChildCount;
            if (childCount > 0)
            {
                IAccessible[] childrenObj = new IAccessible[childCount];
                int count = 0;
                Win32API.AccessibleChildren(iAcc, 0, childCount, childrenObj, out count);

                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        IAccessible child;
                        if (GetValidObjectFromWindow(childrenObj[i], out child))
                        {
                            childrenObj[i] = child;
                        }
                    }
                }

                return childrenObj;
            }

            return null;
        }

        public static int GetChildCount(IAccessible iAcc, int childID)
        {
            try
            {
                if (iAcc != null && childID == 0)
                {
                    return iAcc.accChildCount;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private static bool GetValidObjectFromWindow(IAccessible iAcc, out IAccessible childIAcc)
        {
            childIAcc = null;

            try
            {
                if (iAcc.accChildCount == 7 && MSAATestObject.GetRole(iAcc, 0) == MSAATestObject.RoleType.Window && !IsValidObject(iAcc, 0))
                {
                    childIAcc = MSAATestObject.GetChildIAcc(iAcc, 3);
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public static bool IsValidObject(IAccessible iAcc, int childID)
        {
            String state = MSAATestObject.GetState(iAcc, childID);

            if (state.IndexOf("Invisible") >= 0 || state.IndexOf("Unavailable") >= 0)
            {
                return false;
            }

            if (MSAATestObject.GetRole(iAcc, childID) == RoleType.Window)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
