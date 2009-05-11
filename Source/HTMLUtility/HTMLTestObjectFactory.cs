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

        private static Dictionary<HTMLTestObjectType, string[]> _objectTagsTable = new Dictionary<HTMLTestObjectType, string[]>(59);
        private static Dictionary<string, HTMLTestObjectType> _objectTypeTable = new Dictionary<string, HTMLTestObjectType>(59);

        #endregion

        #region methods

        #region ctor

        private HTMLTestObjectFactory()
        {
        }

        #endregion
        /* HTMLTestObjectType ConvertStrToHTMLType(string type)
        * convert the type text to html type enum. 
        * eg: button to HTMLTestObjectType.Button
        */
        public static HTMLTestObjectType ConvertStrToHTMLType(string type)
        {
            if (String.IsNullOrEmpty(type))
            {
                return HTMLTestObjectType.Unknow;
            }

            type = type.ToUpper().Replace(" ", "");

            HTMLTestObjectType htmlType = HTMLTestObjectType.Unknow;
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
            else if (type == "COMBOBOX" || type == "DROPDOWNBOX" || type == "DROPDOWNLIST" || type == "DROPDOWN" || type == "CB")
            {
                htmlType = HTMLTestObjectType.ComboBox;
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

            _objectTypeTable.Add(type, htmlType);
            return htmlType;
        }


        /*  string[] GetObjectTags(HTMLTestObjectType type)
        *  convert HTMLTestObjectType to HTML tags.
        */
        public static string[] GetObjectTags(HTMLTestObjectType type)
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
                case HTMLTestObjectType.ComboBox:
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
                case HTMLTestObjectType.Unknow:
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
        public static HTMLTestObjectType GetObjectType(IHTMLElement element)
        {
            string tag = element.tagName;

            if (string.IsNullOrEmpty(tag))
            {
                return HTMLTestObjectType.Unknow;
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
                string value;

                if (HTMLTestObject.TryGetProperty(element, "onclick", out value))
                {
                    return HTMLTestObjectType.Button;
                }
                else
                {
                    return HTMLTestObjectType.Image;
                }
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
                    return HTMLTestObjectType.ComboBox;
                }
                else
                {
                    int selectSize = int.Parse(selectValue);

                    if (selectSize < 2)
                    {
                        return HTMLTestObjectType.ComboBox;
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

            return HTMLTestObjectType.Unknow;
        }

        /* HTMLTestGUIObject BuildObjectByType(IHTMLElement element)
         * build the actual test object by an IHTMLElement for different type.
         * It will call the actual constructor of each test object.
         */

        public static HTMLTestGUIObject BuildHTMLTestObject(IHTMLElement element)
        {
            if (element == null)
            {
                throw new CannotBuildObjectException("Element can not be null.");
            }

            HTMLTestObjectType type = GetObjectType(element);

            return BuildHTMLTestObjectByType(element, type, null, null);
        }

        public static HTMLTestGUIObject BuildHTMLTestObject(IHTMLElement element, HTMLTestBrowser browser, HTMLTestObjectPool pool)
        {
            if (element == null)
            {
                throw new CannotBuildObjectException("Element can not be null.");
            }

            HTMLTestObjectType type = GetObjectType(element);

            return BuildHTMLTestObjectByType(element, type, browser, pool);
        }

        public static HTMLTestGUIObject BuildHTMLTestObjectByType(IHTMLElement element, HTMLTestObjectType type, HTMLTestBrowser browser, HTMLTestObjectPool pool)
        {
            if (element == null || type == HTMLTestObjectType.Unknow)
            {
                throw new CannotBuildObjectException("Element and type can not be null.");
            }

            HTMLTestGUIObject tmp;

            switch (type)
            {
                case HTMLTestObjectType.Label:
                    tmp = new HTMLTestLabel(element);
                    break;
                case HTMLTestObjectType.Button:
                    tmp = new HTMLTestButton(element);
                    break;
                case HTMLTestObjectType.TextBox:
                    tmp = new HTMLTestTextBox(element);
                    break;
                case HTMLTestObjectType.ListBox:
                    tmp = new HTMLTestListBox(element);
                    break;
                case HTMLTestObjectType.Link:
                    tmp = new HTMLTestLink(element);
                    break;
                case HTMLTestObjectType.ComboBox:
                    tmp = new HTMLTestComboBox(element);
                    break;
                case HTMLTestObjectType.Image:
                    tmp = new HTMLTestImage(element);
                    break;
                case HTMLTestObjectType.RadioBox:
                    tmp = new HTMLTestRadioBox(element);
                    break;
                case HTMLTestObjectType.CheckBox:
                    tmp = new HTMLTestCheckBox(element);
                    break;
                case HTMLTestObjectType.Table:
                    tmp = new HTMLTestTable(element);
                    break;
                default:
                    tmp = null;
                    break;
            }

            if (tmp != null)
            {
                try
                {
                    tmp.Browser = browser;
                    tmp.HTMLTestObjPool = pool;
                    return tmp;
                }
                catch (CannotGetObjectPositionException ex)
                {
                    throw new CannotBuildObjectException("Can not get object position: " + ex.Message);
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotBuildObjectException(ex.Message);
                }
            }
            else
            {
                throw new CannotBuildObjectException();
            }
        }

        /* bool IsPropertyEqual(IHTMLElement element, string propertyName, string value, int simPercent)
       * check if the property == value with the expected similar percent.
       */
        public static bool IsPropertyLike(IHTMLElement element, string propertyName, string value, int simPercent)
        {
            string actualValue;

            if (HTMLTestObject.TryGetProperty(element, propertyName, out actualValue))
            {
                return Searcher.IsStringLike(actualValue, value, simPercent);
            }
            else
            {
                return false;
            }
        }

        /*  string GetVisibleTextPropertyByTag(HTMLTestObjectType type, string tag)
         *  get the visible property by tag.
         *  eg: for a button, it's tag is <Input type="button">, it's visible property,
         *  means the text on the button, is ".value"
         */
        public static string GetVisibleTextPropertyByTag(HTMLTestObjectType type, string tag)
        {
            //default is ".innerText".
            string property = "innerText";

            string tagValue = tag.ToUpper();
            if (tagValue == "INPUT")
            {
                switch (type)
                {
                    case HTMLTestObjectType.Button:
                        property = "value";
                        break;
                    case HTMLTestObjectType.TextBox:
                        property = "value";
                        break;
                    default:
                        break;
                }
            }
            else if (tagValue == "SELECT")
            {
                property = "innerHTML";
            }

            return property;
        }

        /* bool IsInteractive(IHTMLElement element)
         * check the object if it is visible, and it can interactive with users.
         */
        public static bool IsVisible(IHTMLElement element)
        {
            if (element == null)
            {
                return false;
            }
            else if (element.offsetWidth < 1 || element.offsetHeight < 1)
            {
                return false;
            }

            string tag = element.tagName;

            if (String.IsNullOrEmpty(tag))
            {
                return false;
            }

            string value;

            if (tag == "INPUT")
            {
                //return false, if the it is a hidden object.
                if (HTMLTestObject.TryGetProperty(element, "type", out value))
                {
                    return String.Compare(value, "HIDDEN", true) != 0;
                }
            }
            else if (HTMLTestObject.TryGetProperty(element, "visibility", out value))
            {
                //return false if it is hidden.
                return String.Compare(value, "HIDDEN", true) != 0;
            }

            return true;
        }

        #endregion

    }
}
