using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestObjectFactory
    {
        #region fields

        private static Dictionary<HTMLTestObjectTypeEnum, string[]> _objectTagsTable = new Dictionary<HTMLTestObjectTypeEnum, string[]>(59);
        private static Dictionary<string, HTMLTestObjectTypeEnum> _objectTypeTable = new Dictionary<string, HTMLTestObjectTypeEnum>(59);

        #endregion

        #region methods

        #region ctor

        private HTMLTestObjectFactory()
        {
        }

        #endregion
        /* HTMLTestObjectType GetHTMLTypeByString(string type)
        * convert the type text to html type enum. 
        * eg: button to HTMLTestObjectType.Button
        */
        public static HTMLTestObjectTypeEnum GetHTMLTypeByString(string type)
        {
            if (String.IsNullOrEmpty(type))
            {
                return HTMLTestObjectTypeEnum.Unknow;
            }

            type = type.ToUpper().Trim();

            HTMLTestObjectTypeEnum htmlType = HTMLTestObjectTypeEnum.Unknow;
            if (_objectTypeTable.TryGetValue(type, out htmlType))
            {
                return htmlType;
            }

            if (type == "BUTTON" || type == "BTN" || type == "B")
            {
                htmlType = HTMLTestObjectTypeEnum.Button;
            }
            else if (type == "LABEL" || type == "LB")
            {
                htmlType = HTMLTestObjectTypeEnum.Label;
            }
            else if (type == "TEXTBOX" || type == "TEXT" || type == "INPUTBOX" || type == "TXT" || type == "T")
            {
                htmlType = HTMLTestObjectTypeEnum.TextBox;
            }
            else if (type == "LINK" || type == "HYPERLINK" || type == "LK" || type == "A")
            {
                htmlType = HTMLTestObjectTypeEnum.Link;
            }
            else if (type == "IMAGE" || type == "IMG" || type == "PICTURE" || type == "PIC" || type == "I" || type == "P")
            {
                htmlType = HTMLTestObjectTypeEnum.Image;
            }
            else if (type == "COMBOBOX" || type == "DROPDOWNBOX" || type == "DROPDOWNLIST" || type == "DROPDOWN" || type == "DROPLIST" || type == "CB")
            {
                htmlType = HTMLTestObjectTypeEnum.DropList;
            }
            else if (type == "LISTBOX" || type == "LIST" || type == "LST" || type == "LS")
            {
                htmlType = HTMLTestObjectTypeEnum.ListBox;
            }
            else if (type == "RADIOBOX" || type == "RADIOBUTTON" || type == "RADIO" || type == "RAD" || type == "R")
            {
                htmlType = HTMLTestObjectTypeEnum.RadioBox;
            }
            else if (type == "CHECKBOX" || type == "CHECK" || type == "CHK" || type == "CK")
            {
                htmlType = HTMLTestObjectTypeEnum.CheckBox;
            }
            else if (type == "ACTIVEX")
            {
                htmlType = HTMLTestObjectTypeEnum.ActiveX;
            }
            else if (type == "TABLE" || type == "TBL" || type == "T")
            {
                htmlType = HTMLTestObjectTypeEnum.Table;
            }
            else if (type == "DIALOG" || type == "WINDOWS")
            {
                htmlType = HTMLTestObjectTypeEnum.Dialog;
            }

            _objectTypeTable.Add(type, htmlType);
            return htmlType;
        }


        /*  string[] GetObjectTags(HTMLTestObjectType type)
        *  convert HTMLTestObjectType to HTML tags.
        */
        public static string[] GetObjectTags(HTMLTestObjectTypeEnum type)
        {
            string[] res = null;
            if (_objectTagsTable.TryGetValue(type, out res))
            {
                return res;
            }

            switch (type)
            {
                case HTMLTestObjectTypeEnum.Label:
                    res = new string[] { "label", "span", "font" };
                    break;
                case HTMLTestObjectTypeEnum.Link:
                    res = new string[] { "a" };
                    break;
                case HTMLTestObjectTypeEnum.Button:
                    res = new string[] { "input", "button" };
                    break;
                case HTMLTestObjectTypeEnum.TextBox:
                    res = new string[] { "input", "textarea" };
                    break;
                case HTMLTestObjectTypeEnum.Image:
                    res = new string[] { "img" };
                    break;
                case HTMLTestObjectTypeEnum.CheckBox:
                    res = new string[] { "input", "label" };
                    break;
                case HTMLTestObjectTypeEnum.RadioBox:
                    res = new string[] { "input", "label" };
                    break;
                case HTMLTestObjectTypeEnum.DropList:
                    res = new string[] { "select" };
                    break;
                case HTMLTestObjectTypeEnum.ListBox:
                    res = new string[] { "select" };
                    break;
                case HTMLTestObjectTypeEnum.Table:
                    res = new string[] { "table" };
                    break;
                case HTMLTestObjectTypeEnum.ActiveX:
                    res = new string[] { "object" };
                    break;
                case HTMLTestObjectTypeEnum.Dialog:
                    res = new string[] { };
                    break;
                case HTMLTestObjectTypeEnum.Unknow:
                    res = new string[] { };
                    break;
                default:
                    res = new string[] { };
                    break;
            }

            _objectTagsTable.Add(type, res);
            return res;
        }

        /*  private static HTMLTestObjectType GetObjectType(IHTMLElement element)
         *  Get the HTMLTestObjectType from element's tag name.
         */
        public static HTMLTestObjectTypeEnum GetObjectType(IHTMLElement element)
        {
            string tag = element.tagName;

            if (string.IsNullOrEmpty(tag))
            {
                return HTMLTestObjectTypeEnum.Unknow;
            }
            else if (tag == "A")
            {
                return HTMLTestObjectTypeEnum.Link;
            }
            else if (tag == "SPAN" || tag == "LABEL" || tag == "FONT")
            {
                return HTMLTestObjectTypeEnum.Label;
            }
            else if (tag == "IMG")
            {
                return HTMLTestObjectTypeEnum.Image;
            }
            else if (tag == "BUTTON")
            {
                return HTMLTestObjectTypeEnum.Button;
            }
            else if (tag == "INPUT")
            {
                string inputType;
                if (!HTMLTestObject.TryGetProperty(element, "type", out inputType))
                {
                    return HTMLTestObjectTypeEnum.TextBox;
                }

                inputType = inputType.ToUpper();
                if (inputType == "TEXT" || inputType == "PASSWORD")
                {
                    return HTMLTestObjectTypeEnum.TextBox;
                }
                else if (inputType == "BUTTON" || inputType == "SUBMIT" || inputType == "RESET"
                      || inputType == "FILE" || inputType == "IMAGE")
                {
                    return HTMLTestObjectTypeEnum.Button;
                }
                else if (inputType == "CHECKBOX")
                {
                    return HTMLTestObjectTypeEnum.CheckBox;
                }
                else if (inputType == "RADIO")
                {
                    return HTMLTestObjectTypeEnum.RadioBox;
                }
            }
            else if (tag == "TEXTAREA")
            {
                return HTMLTestObjectTypeEnum.TextBox;
            }
            else if (tag == "TABLE")
            {
                return HTMLTestObjectTypeEnum.Table;
            }
            else if (tag == "SELECT")
            {
                string selectValue;

                if (!HTMLTestObject.TryGetProperty(element, "size", out selectValue))
                {
                    return HTMLTestObjectTypeEnum.DropList;
                }
                else
                {
                    int selectSize = int.Parse(selectValue);

                    if (selectSize < 2)
                    {
                        return HTMLTestObjectTypeEnum.DropList;
                    }
                    else
                    {
                        return HTMLTestObjectTypeEnum.ListBox;
                    }
                }
            }
            else if (tag == "OBJECT")
            {
                return HTMLTestObjectTypeEnum.ActiveX;
            }

            return HTMLTestObjectTypeEnum.Unknow;
        }

        /* HTMLTestGUIObject BuildObjectByType(IHTMLElement element)
         * build the actual test object by an IHTMLElement for different type.
         * It will call the actual constructor of each test object.
         */

        public static HTMLTestGUIObject BuildHTMLTestObject(IHTMLElement element)
        {
            return BuildHTMLTestObject(element, null);
        }

        public static HTMLTestGUIObject BuildHTMLTestObject(IHTMLElement element, HTMLTestBrowser browser)
        {
            return BuildHTMLTestObjectByType(element, HTMLTestObjectTypeEnum.Unknow, browser);
        }

        public static HTMLTestGUIObject BuildHTMLTestObject(IntPtr handle, HTMLTestBrowser browser)
        {
            return BuildHTMLTestObjectByType(null, handle, HTMLTestObjectTypeEnum.Dialog, browser);
        }

        public static HTMLTestGUIObject BuildHTMLTestObjectByType(IHTMLElement element, HTMLTestObjectTypeEnum type, HTMLTestBrowser browser)
        {
            return BuildHTMLTestObjectByType(element, IntPtr.Zero, type, browser);
        }

        public static HTMLTestGUIObject BuildHTMLTestObjectByType(IHTMLElement element, IntPtr handle, HTMLTestObjectTypeEnum type, HTMLTestBrowser browser)
        {
            if (element == null && handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Element and handle can not be null.");
            }

            try
            {
                if (type == HTMLTestObjectTypeEnum.Unknow)
                {
                    type = GetObjectType(element);
                }

                HTMLTestGUIObject tmp;
                switch (type)
                {
                    case HTMLTestObjectTypeEnum.Label:
                        tmp = new HTMLTestLabel(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.Button:
                        tmp = new HTMLTestButton(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.TextBox:
                        tmp = new HTMLTestTextBox(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.ListBox:
                        tmp = new HTMLTestListBox(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.Link:
                        tmp = new HTMLTestLink(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.DropList:
                        tmp = new HTMLTestDropList(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.Image:
                        tmp = new HTMLTestImage(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.RadioBox:
                        tmp = new HTMLTestRadioBox(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.CheckBox:
                        tmp = new HTMLTestCheckBox(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.Table:
                        tmp = new HTMLTestTable(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.ActiveX:
                        tmp = new HTMLTestActiveXObject(element, browser);
                        break;
                    case HTMLTestObjectTypeEnum.Dialog:
                        tmp = new HTMLTestDialog(handle, browser);
                        break;
                    default:
                        tmp = new HTMLTestGUIObject(element, browser);
                        break;
                }

                if (tmp != null)
                {
                    return tmp;
                }
                else
                {
                    throw new CannotBuildObjectException();
                }
            }
            catch (CannotGetObjectPositionException ex)
            {
                throw new CannotBuildObjectException("Can not get object position: " + ex.ToString());
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException(ex.ToString());
            }
        }

        /* bool IsInteractive(IHTMLElement element)
         * check the object if it is visible, and it can interactive with users.
         */
        public static bool IsVisible(IHTMLElement element)
        {
            if (element == null || element.offsetWidth < 2 || element.offsetHeight < 2)
            {
                return false;
            }

            //return false, if the it is a hidden object.
            string value;
            if (HTMLTestObject.TryGetProperty(element, "type", out value))
            {
                return String.Compare(value, "HIDDEN", true) != 0;
            }
            if (HTMLTestObject.TryGetProperty(element, "visibility", out value))
            {
                return String.Compare(value, "HIDDEN", true) != 0;
            }

            return true;
        }

        //get most common properties used in HTML testing.
        public static bool TryGetCommonProperties(TestProperty[] properties, out string id, out string name, out string tag)
        {
            id = "";
            name = "";
            tag = "";
            if (properties != null && properties.Length > 0)
            {
                foreach (TestProperty tp in properties)
                {
                    string tpName = tp.Name;
                    string tpValue = tp.Value.ToString();
                    if (String.IsNullOrEmpty(tpValue))
                    {
                        continue;
                    }

                    if (String.Compare(tpName, "id", true) == 0)
                    {
                        id = tpValue;
                        return true;
                    }
                    else if (String.Compare(tpName, "name", true) == 0)
                    {
                        name = tpValue;
                        return true;
                    }
                    else if (String.Compare(tpName, "tag", true) == 0 || String.Compare(tpName, "tagName", true) == 0)
                    {
                        tag = tpValue;
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
