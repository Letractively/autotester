using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestObjectFactory
    {
        #region fields

        private static Dictionary<String, string[]> _objectTagsTable = new Dictionary<String, string[]>(59);
        private static Dictionary<string, String> _objectTypeTable = new Dictionary<string, String>(59);

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
        public static String GetHTMLTypeByString(string type)
        {
            if (String.IsNullOrEmpty(type))
            {
                return HTMLTestObjectType.Unknown;
            }

            type = type.ToUpper().Trim();

            String htmlType = HTMLTestObjectType.Unknown;
            if (_objectTypeTable.TryGetValue(type, out htmlType))
            {
                return htmlType;
            }

            if (type == "BUTTON" || type == "BTN" || type == "B")
            {
                htmlType = HTMLTestObjectType.Button;
            }
            else if (type == "LABEL" || type == "LB")
            {
                htmlType = HTMLTestObjectType.Label;
            }
            else if (type == "TEXTBOX" || type == "TEXT" || type == "INPUTBOX" || type == "TXT" || type == "T")
            {
                htmlType = HTMLTestObjectType.TextBox;
            }
            else if (type == "LINK" || type == "HYPERLINK" || type == "LK" || type == "A")
            {
                htmlType = HTMLTestObjectType.Link;
            }
            else if (type == "IMAGE" || type == "IMG" || type == "PICTURE" || type == "PIC" || type == "I" || type == "P")
            {
                htmlType = HTMLTestObjectType.Image;
            }
            else if (type == "COMBOBOX" || type == "DROPDOWNBOX" || type == "DROPDOWNLIST" || type == "DROPDOWN" || type == "DROPLIST" || type == "CB")
            {
                htmlType = HTMLTestObjectType.DropList;
            }
            else if (type == "LISTBOX" || type == "LIST" || type == "LST" || type == "LS")
            {
                htmlType = HTMLTestObjectType.ListBox;
            }
            else if (type == "RADIOBOX" || type == "RADIOBUTTON" || type == "RADIO" || type == "RAD" || type == "R")
            {
                htmlType = HTMLTestObjectType.RadioBox;
            }
            else if (type == "CHECKBOX" || type == "CHECK" || type == "CHK" || type == "CK")
            {
                htmlType = HTMLTestObjectType.CheckBox;
            }
            else if (type == "ACTIVEX")
            {
                htmlType = HTMLTestObjectType.ActiveX;
            }
            else if (type == "TABLE" || type == "TBL" || type == "T")
            {
                htmlType = HTMLTestObjectType.Table;
            }
            else if (type == "FORM")
            {
                htmlType = HTMLTestObjectType.Form;
            }
            else if (type == "DIALOG" || type == "WINDOWS")
            {
                htmlType = HTMLTestObjectType.Dialog;
            }

            _objectTypeTable.Add(type, htmlType);
            return htmlType;
        }


        /*  string[] GetObjectTags(HTMLTestObjectType type)
        *  convert HTMLTestObjectType to HTML tags.
        */
        public static string[] GetObjectTags(String type)
        {
            string[] res = null;
            if (_objectTagsTable.TryGetValue(type, out res))
            {
                return res;
            }

            switch (type)
            {
                case HTMLTestObjectType.Label:
                    res = new string[] { "label", "span", "font" };
                    break;
                case HTMLTestObjectType.Link:
                    res = new string[] { "a" };
                    break;
                case HTMLTestObjectType.Button:
                    res = new string[] { "input", "button" };
                    break;
                case HTMLTestObjectType.TextBox:
                    res = new string[] { "input", "textarea" };
                    break;
                case HTMLTestObjectType.Image:
                    res = new string[] { "img" };
                    break;
                case HTMLTestObjectType.CheckBox:
                    res = new string[] { "input", "label" };
                    break;
                case HTMLTestObjectType.RadioBox:
                    res = new string[] { "input", "label" };
                    break;
                case HTMLTestObjectType.DropList:
                    res = new string[] { "select" };
                    break;
                case HTMLTestObjectType.ListBox:
                    res = new string[] { "select" };
                    break;
                case HTMLTestObjectType.Table:
                    res = new string[] { "table" };
                    break;
                case HTMLTestObjectType.ActiveX:
                    res = new string[] { "object" };
                    break;
                case HTMLTestObjectType.Form:
                    res = new string[] { "form" };
                    break;
                case HTMLTestObjectType.Dialog:
                    res = new string[] { };
                    break;
                case HTMLTestObjectType.Unknown:
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
        public static String GetObjectType(IHTMLElement element)
        {
            string tag = element.tagName;

            if (string.IsNullOrEmpty(tag))
            {
                return HTMLTestObjectType.Unknown;
            }
            else if (tag == "A")
            {
                return HTMLTestObjectType.Link;
            }
            else if (tag == "SPAN" || tag == "LABEL" || tag == "FONT")
            {
                return HTMLTestObjectType.Label;
            }
            else if (tag == "IMG")
            {
                return HTMLTestObjectType.Image;
            }
            else if (tag == "BUTTON")
            {
                return HTMLTestObjectType.Button;
            }
            else if (tag == "INPUT")
            {
                string inputType;
                if (!HTMLTestObject.TryGetProperty(element, "type", out inputType))
                {
                    return HTMLTestObjectType.TextBox;
                }

                inputType = inputType.ToUpper();
                if (inputType == "TEXT" || inputType == "PASSWORD")
                {
                    return HTMLTestObjectType.TextBox;
                }
                else if (inputType == "BUTTON" || inputType == "SUBMIT" || inputType == "RESET"
                      || inputType == "FILE" || inputType == "IMAGE")
                {
                    return HTMLTestObjectType.Button;
                }
                else if (inputType == "CHECKBOX")
                {
                    return HTMLTestObjectType.CheckBox;
                }
                else if (inputType == "RADIO")
                {
                    return HTMLTestObjectType.RadioBox;
                }
            }
            else if (tag == "TEXTAREA")
            {
                return HTMLTestObjectType.TextBox;
            }
            else if (tag == "TABLE")
            {
                return HTMLTestObjectType.Table;
            }
            else if (tag == "SELECT")
            {
                string selectValue;

                if (!HTMLTestObject.TryGetProperty(element, "size", out selectValue))
                {
                    return HTMLTestObjectType.DropList;
                }
                else
                {
                    int selectSize = int.Parse(selectValue);

                    if (selectSize < 2)
                    {
                        return HTMLTestObjectType.DropList;
                    }
                    else
                    {
                        return HTMLTestObjectType.ListBox;
                    }
                }
            }
            else if (tag == "OBJECT")
            {
                return HTMLTestObjectType.ActiveX;
            }
            else if (tag == "FORM")
            {
                return HTMLTestObjectType.Form;
            }

            return HTMLTestObjectType.Unknown;
        }

        /* HTMLTestGUIObject BuildObjectByType(IHTMLElement element)
         * build the actual test object by an IHTMLElement for different type.
         * It will call the actual constructor of each test object.
         */

        public static HTMLTestGUIObject BuildHTMLTestObject(IHTMLElement element)
        {
            return BuildHTMLTestObject(element, null);
        }

        public static HTMLTestGUIObject BuildHTMLTestObject(IHTMLElement element, HTMLTestPage page)
        {
            return BuildHTMLTestObjectByType(element, HTMLTestObjectType.Unknown, page);
        }

        public static HTMLTestGUIObject BuildHTMLTestObject(IntPtr handle, HTMLTestPage page)
        {
            return BuildHTMLTestObjectByType(null, handle, HTMLTestObjectType.Dialog, page);
        }

        public static HTMLTestGUIObject BuildHTMLTestObjectByType(IHTMLElement element, String type, HTMLTestPage page)
        {
            return BuildHTMLTestObjectByType(element, IntPtr.Zero, type, page);
        }

        public static HTMLTestGUIObject BuildHTMLTestObjectByType(IHTMLElement element, IntPtr handle, String type, HTMLTestPage page)
        {
            if (element == null && handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Element and handle can not be null.");
            }

            try
            {
                if (type == HTMLTestObjectType.Unknown)
                {
                    type = GetObjectType(element);
                }

                HTMLTestGUIObject tmp;
                switch (type)
                {
                    case HTMLTestObjectType.Label:
                        tmp = new HTMLTestLabel(element, page);
                        break;
                    case HTMLTestObjectType.Button:
                        tmp = new HTMLTestButton(element, page);
                        break;
                    case HTMLTestObjectType.TextBox:
                        tmp = new HTMLTestTextBox(element, page);
                        break;
                    case HTMLTestObjectType.ListBox:
                        tmp = new HTMLTestListBox(element, page);
                        break;
                    case HTMLTestObjectType.Link:
                        tmp = new HTMLTestLink(element, page);
                        break;
                    case HTMLTestObjectType.DropList:
                        tmp = new HTMLTestDropList(element, page);
                        break;
                    case HTMLTestObjectType.Image:
                        tmp = new HTMLTestImage(element, page);
                        break;
                    case HTMLTestObjectType.RadioBox:
                        tmp = new HTMLTestRadioBox(element, page);
                        break;
                    case HTMLTestObjectType.CheckBox:
                        tmp = new HTMLTestCheckBox(element, page);
                        break;
                    case HTMLTestObjectType.Table:
                        tmp = new HTMLTestTable(element, page);
                        break;
                    case HTMLTestObjectType.Form:
                        tmp = new HTMLTestForm(element, page);
                        break;
                    case HTMLTestObjectType.ActiveX:
                        tmp = new HTMLTestActiveXObject(element, page);
                        break;
                    case HTMLTestObjectType.Dialog:
                        tmp = new HTMLTestDialog(handle, page);
                        break;
                    default:
                        tmp = new HTMLTestGUIObject(element, page);
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
