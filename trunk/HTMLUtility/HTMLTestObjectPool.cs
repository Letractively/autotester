using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using mshtml;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestObjectPool : ITestObjectPool
    {
        #region fields

        private HTMLTestBrowser _htmlTestBrowser;
        private static bool _needRefresh = false;

        private IHTMLElement _tempElement;
        private IHTMLElementCollection _allElements;
        private object[] _allObjects;

        private TestObject _testObj;


        #endregion

        #region Properties

        public HTMLTestBrowser TestBrower
        {
            get { return _htmlTestBrowser; }
            set
            {
                _htmlTestBrowser = value;
            }
        }

        #endregion

        #region public methods

        #region ctor

        public HTMLTestObjectPool()
        {

        }

        public HTMLTestObjectPool(TestBrowser brower)
        {
            _htmlTestBrowser = (HTMLTestBrowser)brower;
        }
        #endregion

        public void SetTestBrowser(ITestBrowser brower)
        {
            _htmlTestBrowser = (HTMLTestBrowser)brower;
        }

        public Object GetObjectByID(string id)
        {
            try
            {
                _tempElement = _htmlTestBrowser.GetObjectByID(id);
                _testObj = BuildObjectByType(_tempElement);
                return _testObj;
            }
            catch
            {
                throw new ObjectNotFoundException("Can not get object by id:" + id);
            }

        }

        public Object GetObjectByName(string name)
        {
            try
            {
                object nameObj = (object)name;
                object indexObj = (object)0;

                _tempElement = (IHTMLElement)_htmlTestBrowser.GetObjectsByName(name).item(nameObj, indexObj);
                _testObj = BuildObjectByType(_tempElement);
                return _testObj;

            }
            catch
            {
                throw new ObjectNotFoundException("Can not get object by name:" + name);
            }
        }

        public Object GetObjectByIndex(int index)
        {
            return null;
        }

        public Object GetObjectByProperty(string property, string value)
        {

            if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(value))
            {
                throw new PropertyNotFoundException("Property and Value can not be empty.");
            }

            try
            {
                IHTMLElementCollection tmpCollection = _htmlTestBrowser.GetAllObjects();
                for (int i = 0; i < tmpCollection.length; i++)
                {
                    object nameObj = (object)i;
                    object indexObj = (object)i;

                    _tempElement = (IHTMLElement)tmpCollection.item(nameObj, indexObj);

                    try
                    {
                        string propertyValue = _tempElement.getAttribute(property, 0).ToString();
                        if (String.IsNullOrEmpty(propertyValue))
                        {
                            continue;
                        }
                        if (String.Compare(propertyValue, value, true) == 0)
                        {
                            _testObj = BuildObjectByType(_tempElement);
                            return _testObj;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            catch
            {

            }

            throw new ObjectNotFoundException("Can not find object by property[" + property + "] with value [" + value + "].");
        }

        public Object GetObjectByRegex(string property, string regex)
        {
            return null;
        }

        public Object GetObjectByType(string type, string values, int index)
        {

            HTMLTestObjectType typeValue;
            typeValue = GetTypeByString(type);

            if (typeValue == HTMLTestObjectType.Unknow)
            {
                throw new ObjectNotFoundException("Unknow HTML object type.");
            }

            string[] tags = GetObjectTags(typeValue);

            IHTMLElementCollection _tmpElementCol;
            // IHTMLElement _tmpElement;

            object nameObj = null;
            object indexObj = null;

            //if the expected value is number like, we think it is stand for the index of the object, not text on it.
            bool isByIndex = false;
            int objIndex;
            if (int.TryParse(values, out objIndex))
            {
                index = objIndex - 1;
                isByIndex = true;
            }

            int leftIndex = index;

            foreach (string tag in tags)
            {
                _tmpElementCol = _htmlTestBrowser.GetObjectsByTagName(tag);
                int len = _tmpElementCol.length;

                for (int i = 0; i < len; i++)
                {
                    nameObj = (object)i;
                    indexObj = (object)i;

                    _tempElement = (IHTMLElement)_tmpElementCol.item(nameObj, indexObj);


                    try
                    {
                        // check if it is a interactive object.
                        if (!IsInteractive(_tempElement))
                        {
                            continue;
                        }

                        if (isByIndex)
                        {
                            leftIndex--;
                        }
                        else if (CheckObjectByType(_tempElement, typeValue, values))
                        {
                            leftIndex--;
                        }

                        if (leftIndex < 0)
                        {
                            _testObj = BuildObjectByType(_tempElement);
                            return _testObj;
                        }
                        else
                        {
                            continue;
                        }

                    }
                    catch
                    {
                        continue;
                    }

                }

            }

            throw new ObjectNotFoundException("Can not find object by type [" + type.ToString() + "] with value [" + values.ToString() + "]");
        }

        public Object GetObjectByAI(string value)
        {
            return null;
        }

        public Object GetObjectByPoint(int x, int y)
        {
            try
            {
                _tempElement = this._htmlTestBrowser.GetObjectFromPoint(x, y);
                _testObj = BuildObjectByType(_tempElement);
                return _testObj;
            }
            catch (Exception e)
            {
                throw new CanNotBuildObjectException("Can not build object: " + e.Message);
            }
        }

        public Object GetObjectByRect(int top, int left, int width, int height)
        {
            return null;
        }

        public Object GetObjectByColor(string color)
        {
            return null;
        }

        public Object GetObjectByCustomer(object value)
        {
            return null;
        }
        public Object[] GetAllObjects()
        {
            GetIEAllObjects();

            _allObjects = new object[this._allElements.length];

            object nameObj;
            object indexObj;

            for (int i = 0; i < this._allElements.length; i++)
            {
                nameObj = (object)i;
                indexObj = (object)i;
                _allObjects[i] = this._allElements.item(nameObj, indexObj);
            }

            return _allObjects;
        }
        #endregion

        #region help methods

        public static void DocumentRefreshed()
        {
            _needRefresh = true;
        }

        private void GetIEAllObjects()
        {
            if (_needRefresh)
            {
                _needRefresh = false;
                try
                {
                    this._allElements = _htmlTestBrowser.GetAllObjects();
                }
                catch
                {
                    _needRefresh = true;
                }
            }
        }



        public static string[] GetObjectTags(HTMLTestObjectType type)
        {
            switch (type)
            {
                case HTMLTestObjectType.ActiveX:
                    return new string[] { "object" };
                case HTMLTestObjectType.Button:
                    return new string[] { "input", "img" };
                case HTMLTestObjectType.CheckBox:
                    return new string[] { "input" };
                case HTMLTestObjectType.ComboBox:
                    return new string[] { "select" };
                case HTMLTestObjectType.FileDialog:
                    return new string[] { "input" };
                case HTMLTestObjectType.Image:
                    return new string[] { "img" };
                case HTMLTestObjectType.Link:
                    return new string[] { "a" };
                case HTMLTestObjectType.ListBox:
                    return new string[] { "select" };
                case HTMLTestObjectType.MsgBox:
                    return new string[] { };
                case HTMLTestObjectType.RadioButton:
                    return new string[] { "input" };
                case HTMLTestObjectType.Table:
                    return new string[] { "table" };
                case HTMLTestObjectType.TextBox:
                    return new string[] { "input", "textarea" };
                case HTMLTestObjectType.Unknow:
                    return new string[] { };
                default:
                    return new string[] { };
            }
        }


        #region private methods

        private static bool CheckObjectByType(IHTMLElement element, HTMLTestObjectType type, string value)
        {
            string tag = element.tagName.ToString();
            string property = GetVisibleTextPropertyByTag(type, tag);

            if (String.IsNullOrEmpty(property))
            {
                return false;
            }

            if (type == HTMLTestObjectType.ListBox || type == HTMLTestObjectType.ComboBox)
            {
                return CheckSelectObject(element, value);
            }

            string propertyValue = element.getAttribute(property, 0).ToString();
            if (String.IsNullOrEmpty(propertyValue))
            {
                return false;
            }
            if (String.Compare(propertyValue, value, true) == 0)
            {
                return true;
            }

            return false;
        }

        private static bool CheckSelectObject(IHTMLElement element, string value)
        {
            bool result = false;

            try
            {
                string items = element.getAttribute("innerHTML", 0).ToString();

                //get the position of ">"
                int pos1 = items.IndexOf(">");

                //get the position of "<" 
                int pos2 = items.IndexOf("<", pos1);

                // then we can get the first item of select object.
                string firstItem = items.Substring(pos1 + 1, pos2 - pos1 - 1);

                if (String.Compare(firstItem, value, true) == 0)
                {
                    return true;
                }
            }
            catch
            {

            }

            return result;
        }

        private static string GetVisibleTextPropertyByTag(HTMLTestObjectType type, string tag)
        {
            string property = null;

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
                    case HTMLTestObjectType.Image:
                        property = "src";
                        break;
                    default:
                        property = "innerText";
                        break;
                }
            }
            else if (tagValue == "TEXTAREA")
            {
                property = "innerText";
            }
            else if (tagValue == "SELECT")
            {
                property = "innerHTML";
            }
            else if (tagValue == "A")
            {
                property = "innerText";
            }

            return property;
        }

        //check the object if it is visible, and it can interactive with users.
        private static bool IsInteractive(IHTMLElement element)
        {
            string tag = element.tagName.ToUpper();

            if (String.IsNullOrEmpty(tag))
            {
                return false;
            }
            else if (tag == "BR"
                  || tag == "TR"
                  || tag == "P"
                  || tag == "TH"
                  || tag == "SCRIPT"
                  || tag == "FORM"
                    )
            {
                return false;
            }

            if (tag == "INPUT")
            {
                if (element.getAttribute("type", 0).ToString().ToUpper() == "HIDDEN")
                {
                    return false;
                }

            }

            if (element.getAttribute("enable", 0).ToString().ToUpper() == "FALSE")
            {
                return false;
            }

            if (element.getAttribute("visibility", 0).ToString().ToUpper() == "HIDDEN")
            {
                return false;
            }

            return true;
        }

        // convert the type text to html type enum. eg: button to HTMLTestObjectType.Button
        private static HTMLTestObjectType GetTypeByString(string type)
        {
            if (String.IsNullOrEmpty(type))
            {
                return HTMLTestObjectType.Unknow;
            }

            type = type.ToUpper().Replace(" ", "");

            if (type == "BUTTON" || type == "BTN")
            {
                return HTMLTestObjectType.Button;
            }
            else if (type == "TEXTBOX" || type == "TEXT" || type == "INPUTBOX")
            {
                return HTMLTestObjectType.TextBox;
            }
            else if (type == "LINK" || type == "HYPERLINK")
            {
                return HTMLTestObjectType.Link;
            }
            else if (type == "IMAGE" || type == "IMG")
            {
                return HTMLTestObjectType.Image;
            }
            else if (type == "COMBOBOX" || type == "DROPDOWNBOX" || type == "DROPDOWNLIST" || type == "DROPDOWN")
            {
                return HTMLTestObjectType.ComboBox;
            }
            else if (type == "LISTBOX" || type == "LIST")
            {
                return HTMLTestObjectType.ListBox;
            }
            else if (type == "RADIOBOX" || type == "RADIOBUTTON" || type == "RADIO")
            {
                return HTMLTestObjectType.RadioButton;
            }
            else if (type == "CHECKBOX" || type == "CHECK")
            {
                return HTMLTestObjectType.CheckBox;
            }
            else if (type == "FILEDIAGLOG")
            {
                return HTMLTestObjectType.FileDialog;
            }
            else if (type == "ACTIVEX")
            {
                return HTMLTestObjectType.ActiveX;
            }
            else if (type == "MSGBOX" || type == "MESSAGE" || type == "POPWINDOW" || type == "POPBOX")
            {
                return HTMLTestObjectType.MsgBox;
            }
            else if (type == "TABLE")
            {
                return HTMLTestObjectType.Table;
            }
            else
            {
                return HTMLTestObjectType.Unknow;
            }

        }

        private HTMLTestObject BuildObjectByType(IHTMLElement element)
        {
            HTMLTestObjectType type = HTMLTestObject.GetObjectType(element);

            HTMLGuiTestObject tmp;

            switch (type)
            {
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
                default:
                    tmp = null;
                    break;
            }
            return tmp;
        }

        #endregion

        #endregion
    }
}
