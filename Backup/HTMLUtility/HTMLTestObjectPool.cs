/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestObjectPool.cs
*
* Description: This class implement ITestObjectPool interface.
*              we can get HTML object from HTMLTestObjectPool
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

using mshtml;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestObjectPool : ITestObjectPool
    {
        #region fields

        private HTMLTestBrowser _htmlTestBrowser;

        // _needRefresh is used for cache, if this is set to true, we need to get the object
        // from test browser, if it is false, we can just return the ohject from cache.
        private static bool _needRefresh = false;

        //we use a hashtable as the cache, the key is generated from Method Name + _keySplitter+parameter. 
        private static string _keySplitter = "__shrinerain__";
        private static Dictionary<String, TestObject> _testObjectCache = new Dictionary<string, TestObject>();

        //IHTMLElement is the interface for mshtml html object. We build actual test object on IHTMLElement.
        private IHTMLElement _tempElement;

        //IHTMLElementCollection is an array of IHTMLElement, some functions return the array of IHTMLElement.
        private IHTMLElementCollection _allElements;

        private object[] _allObjects;

        //current object used.
        private TestObject _testObj;

        //the max time we need to wait, eg: we may wait for 120s to find a test object.
        private const int _maxWaitSeconds = 120;
        //very time we sleep for 3 seconds, and find again.
        private const int _interval = 3;

        #endregion

        #region Properties

        //set the browser for this object pool, we get objects from a browser
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

        public HTMLTestObjectPool(ITestBrowser brower)
        {
            _htmlTestBrowser = (HTMLTestBrowser)brower;
        }

        #endregion

        /* void SetTestBrowser(ITestBrowser brower)
         * Set the related browser, we will get object from this browser.
         * 
         */
        public void SetTestBrowser(ITestBrowser brower)
        {
            _htmlTestBrowser = (HTMLTestBrowser)brower;
        }

        public void SetTestApp(ITestApp app)
        {
            //for HTML tesing, no desktop application is needed.
        }

        /* Object GetObjectByID(string id)
         * return the test object by .id property.
         *
         */
        public Object GetObjectByID(string id)
        {
            //we will try 120 seconds to find an object.
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    //get IHTMLElement interface
                    _tempElement = _htmlTestBrowser.GetObjectByID(id);

                    //build actual test object.
                    _testObj = BuildObjectByType(_tempElement);
                    return _testObj;
                }
                catch
                {

                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object by id:" + id);

        }


        /*  Object GetObjectByName(string name)
         *  return the test object by .name property
         */
        public Object GetObjectByName(string name)
        {
            //we will try 3 times to find an object.
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    object nameObj = (object)name;
                    object indexObj = (object)0;

                    //get the object collection with the same name.
                    // in HTML, .id is unique, but .name is not, so we may get a collection.
                    _tempElement = (IHTMLElement)_htmlTestBrowser.GetObjectsByName(name).item(nameObj, indexObj);
                    _testObj = BuildObjectByType(_tempElement);
                    return _testObj;

                }
                catch
                {

                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object by name:" + name);
        }

        /* Object GetObjectByIndex(int index)
         * return the test object by an integer index.
         */
        public Object GetObjectByIndex(int index)
        {
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    GetAllObjects();

                    _tempElement = (IHTMLElement)_allObjects[index];

                    _testObj = BuildObjectByType(_tempElement);

                    return _testObj;
                }
                catch
                {

                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object by index:" + index);

        }


        /* Object GetObjectByProperty(string property, string value)
         * return the test object by expect property.
         * eg: to find a image, we can find it by it's .src property, like .src="111.jpg"
         */
        public Object GetObjectByProperty(string property, string value)
        {

            //we will try 120s to find an object.
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(value))
                {
                    throw new PropertyNotFoundException("Property and Value can not be empty.");
                }

                try
                {
                    //get all HTML objects.
                    GetAllElements();

                    IHTMLElementCollection tmpCollection = _allElements;

                    object nameObj = null;
                    object indexObj = null;

                    //go through all element, check it's property value.
                    for (int i = 0; i < tmpCollection.length; i++)
                    {
                        try
                        {
                            nameObj = (object)i;
                            indexObj = (object)i;

                            //get element by index.
                            _tempElement = (IHTMLElement)tmpCollection.item(nameObj, indexObj);

                            // we can not find the property value of current object, next.
                            if (_tempElement.getAttribute(property, 0) == null || _tempElement.getAttribute(property, 0).GetType().ToString() == "System.DBNull")
                            {
                                continue;
                            }

                            //get property value
                            string propertyValue = _tempElement.getAttribute(property, 0).ToString().Trim();

                            if (!String.IsNullOrEmpty(propertyValue))
                            {
                                //if equal, means we found it.
                                if (String.Compare(propertyValue, value, true) == 0)
                                {
                                    _testObj = BuildObjectByType(_tempElement);
                                    return _testObj;
                                }
                            }

                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {

                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not find object by property[" + property + "] with value [" + value + "].");
        }

        /* Object GetObjectByRegex(string property, string regex)
         * return the test object by property, the value match the regular expression
         */
        public Object GetObjectByRegex(string property, string regex)
        {
            return null;
        }

        /*  Object GetObjectByType(string type, string values, int index)
         *  return the test object by an TYPE text, eg: button.
         *  values means the visible value of the control, 
         *  index means if we have more than one object, return which one, normally, it should be 0, return the first one.
         */
        public Object GetObjectByType(string type, string values, int index)
        {
            //we will try 3 times to find a object.
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                HTMLTestObjectType typeValue;

                //convert the TYPE text to valid internal type.
                // eg: button to HTMLTestObjectType.Button
                typeValue = GetTypeByString(type);

                if (typeValue == HTMLTestObjectType.Unknow)
                {
                    throw new ObjectNotFoundException("Unknow HTML object type.");
                }

                //convert the type to HTML tags.
                //eg: convert Image to <img>, Button to <input type="button">,<input type="submit">...
                string[] tags = GetObjectTags(typeValue);

                IHTMLElementCollection _tmpElementCol;

                object nameObj = null;
                object indexObj = null;

                //if the expected value is number like, we think it is stand for the index of the object, not text on it.
                // eg: we can use type="textbox", values="1" to find the first textbox.
                bool isByIndex = false;
                int objIndex;
                if (int.TryParse(values, out objIndex))
                {
                    index = objIndex - 1;
                    isByIndex = true;
                }

                //if we can find more than one test object, we need to consider about the index.
                int leftIndex = index;

                //because we may convert one type to mutil tags, so check them one by one.
                foreach (string tag in tags)
                {
                    //find the object by tag.
                    _tmpElementCol = _htmlTestBrowser.GetObjectsByTagName(tag);

                    //check object one by one
                    for (int i = 0; i < _tmpElementCol.length; i++)
                    {
                        try
                        {
                            nameObj = (object)i;
                            indexObj = (object)i;

                            _tempElement = (IHTMLElement)_tmpElementCol.item(nameObj, indexObj);

                            // check if it is a interactive object.
                            if (IsInteractive(_tempElement))
                            {
                                //if mutil object, check if it is the object at expectd index
                                if (isByIndex)
                                {
                                    leftIndex--;
                                }
                                else if (CheckObjectByType(_tempElement, typeValue, values)) //check object by type
                                {
                                    leftIndex--;
                                }

                                //if index is 0 , that means we found the object.
                                if (leftIndex < 0)
                                {
                                    _testObj = BuildObjectByType(_tempElement, typeValue);
                                    return _testObj;
                                }
                            }

                        }
                        catch
                        {
                        }
                    }
                }

                //not found, sleep 3 seconds, then try again.
                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not find object by type [" + type.ToString() + "] with value [" + values.ToString() + "]");
        }

        /* Object GetObjectByAI(string value)
         * return object by value, we will use some common property to find the control.
         * eg: use .value for a button
         */
        public Object GetObjectByAI(string value)
        {
            return null;
        }


        /* Object GetObjectByPoint(int x, int y)
         * return object from a expected point
         * 
         */
        public Object GetObjectByPoint(int x, int y)
        {
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    _tempElement = this._htmlTestBrowser.GetObjectFromPoint(x, y);
                    _testObj = BuildObjectByType(_tempElement);
                    return _testObj;
                }
                catch
                {
                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new CanNotBuildObjectException("Can not build object at point(" + x.ToString() + "" + y.ToString() + ")");
        }

        /* Object GetObjectByRect(int top, int left, int width, int height)
         * return object from a expected rect.
         * like QTP virtual object.
         * 
         */
        public Object GetObjectByRect(int top, int left, int width, int height)
        {
            return null;
        }

        /* Object GetObjectByColor(string color)
         * return object by expected color
         */
        public Object GetObjectByColor(string color)
        {
            return null;
        }

        /* Object GetObjectByCustomer(object value)
         * for future use.
         */
        public Object GetObjectByCustomer(object value)
        {
            return null;
        }

        /* Object[] GetAllObjects()
         * return all object from the  browser.
         * we use _allObjects to store these object.
         */
        public Object[] GetAllObjects()
        {
            //firstly, get all IHTMLElement from the browser
            GetAllElements();

            _allObjects = new object[this._allElements.length];

            object nameObj;
            object indexObj;

            //convert IHTMLELementCollection to an array.
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

        /* void DocumentRefreshed()
         * happened when document load successfully, we need to reload all test objects.
         */
        public static void DocumentRefreshed()
        {
            _needRefresh = true;
        }


        /*  string[] GetObjectTags(HTMLTestObjectType type)
         *  convert HTMLTestObjectType to HTML tags.
         */
        public static string[] GetObjectTags(HTMLTestObjectType type)
        {
            switch (type)
            {
                case HTMLTestObjectType.ActiveX:
                    return new string[] { "object" };
                case HTMLTestObjectType.Button:
                    return new string[] { "input", "img" };
                case HTMLTestObjectType.CheckBox:
                    return new string[] { "input", "label" };
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
                    return new string[] { "input", "label" };
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

        /* void GetAllElements()
         * load all IHTMLElement from browser.
         * just reload when the _needRefresh flag is set to ture.
         * 
         */
        private void GetAllElements()
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

        /* bool CheckObjectByType(IHTMLElement element, HTMLTestObjectType type, string value)
         * Check the object by expected type.
         * For different object, we need to check different property.
         * eg: for a listbox, we need to check it's first item.
         */
        private static bool CheckObjectByType(IHTMLElement element, HTMLTestObjectType type, string value)
        {

            //check special types
            if (type == HTMLTestObjectType.ListBox || type == HTMLTestObjectType.ComboBox)
            {
                return CheckSelectObject(element, value);
            }
            else if (type == HTMLTestObjectType.RadioButton)
            {
                return CheckRadioObject(element, value);
            }
            else if (type == HTMLTestObjectType.CheckBox)
            {
                return CheckCheckBoxObject(element, value);
            }
            else if (type == HTMLTestObjectType.Image)
            {
                return CheckImageObject(element, value);
            }

            string tag = element.tagName.ToString();

            //get default property of each tag
            //eg: for a textbox, the property is .value
            string property = GetVisibleTextPropertyByTag(type, tag);

            if (String.IsNullOrEmpty(property))
            {
                return false;
            }
            else if (element.getAttribute(property, 0) == null || element.getAttribute(property, 0).GetType().ToString() == "System.DBNull")
            {
                //not special type, just need to check some property.
                return false;
            }

            string propertyValue = element.getAttribute(property, 0).ToString().Trim();
            if (!String.IsNullOrEmpty(propertyValue))
            {
                if (String.Compare(propertyValue, value, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /*  bool CheckSelectObject(IHTMLElement element, string value)
         *  check select object like ListBox , Combobox etc.
         *  for select object, we need to check it's first item.
         */
        private static bool CheckSelectObject(IHTMLElement element, string value)
        {

            try
            {
                string propertyName = "innerHTML";
                if (element.getAttribute(propertyName, 0) == null || element.getAttribute(propertyName, 0).GetType().ToString() == "System.DBNull")
                {
                    return false;
                }

                string items = element.getAttribute(propertyName, 0).ToString().Trim();

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

            return false;
        }

        /* bool CheckImageObject(IHTMLElement element, string value)
         * return true if the src of img is ends witdh the input value.
         */
        private static bool CheckImageObject(IHTMLElement element, string value)
        {
            try
            {
                //check the .src property.
                string propertyName = "src";
                if (element.getAttribute(propertyName, 0) == null || element.getAttribute(propertyName, 0).GetType().ToString() == "System.DBNull")
                {
                    return false;
                }

                string imgSrc = element.getAttribute(propertyName, 0).ToString().Trim();

                if (imgSrc.EndsWith(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;

        }

        /* bool CheckRadioObject(IHTMLElement element, string value)
         * Check radio button.
         * we have 2 way to check a radio button.
         * 1. check it's .value property.
         * 2. check it's label. label is the text, you click the text makes you click the radio button
         */
        private static bool CheckRadioObject(IHTMLElement element, string value)
        {
            try
            {
                string propertyName = null;
                if (element.tagName == "INPUT")
                {
                    propertyName = ".value";
                }
                else if (element.tagName == "LABEL")
                {
                    propertyName = "innerText";
                }

                if (element.getAttribute(propertyName, 0) != null && element.getAttribute(propertyName, 0).GetType().ToString() != "System.DBNull")
                {
                    string actualValue = element.getAttribute(propertyName, 0).ToString().Trim();
                    if (String.Compare(actualValue, value, true) == 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {

            }

            return false;
        }

        /* bool CheckCheckBoxObject(IHTMLElement element, string value)
         * check check box.
         * we have 2 ways to check a check box
         * 1. check it's .value property.
         * 2. check it's label
         */
        private static bool CheckCheckBoxObject(IHTMLElement element, string value)
        {
            try
            {
                string propertyName = null;
                if (element.tagName == "INPUT")
                {
                    propertyName = "value";
                }
                else if (element.tagName == "LABEL")
                {
                    propertyName = "innerText";
                }

                if (element.getAttribute(propertyName, 0) != null && element.getAttribute(propertyName, 0).GetType().ToString() != "System.DBNull")
                {
                    string actualValue = element.getAttribute(propertyName, 0).ToString().Trim();
                    if (String.Compare(actualValue, value, true) == 0)
                    {
                        return true;
                    }
                }

            }
            catch
            {

            }

            return false;
        }

        /*  string GetVisibleTextPropertyByTag(HTMLTestObjectType type, string tag)
         *  get the visible property by tag.
         *  eg: for a button, it's tag is <Input type="button">, it's visible property,
         *  means the text on the button, is ".value"
         */
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

        /* bool IsInteractive(IHTMLElement element)
         * check the object if it is visible, and it can interactive with users.
         */
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
                //return false, if the it is a hidden object.
                if (element.getAttribute("type", 0).ToString().Trim().ToUpper() == "HIDDEN")
                {
                    return false;
                }

            }

            if (element.getAttribute("enable", 0) != null && element.getAttribute("enable", 0).GetType().ToString() != "System.DBNull")
            {
                //return false if it is not enabled.
                if (element.getAttribute("enable", 0).ToString().Trim().ToUpper() == "FALSE")
                {
                    return false;
                }
            }

            if (element.getAttribute("visibility", 0) != null && element.getAttribute("visibility", 0).GetType().ToString() != "System.DBNull")
            {
                //return false if it is not visbile.
                if (element.getAttribute("visibility", 0).ToString().Trim().ToUpper() == "HIDDEN")
                {
                    return false;
                }
            }

            return true;
        }

        /* HTMLTestObjectType GetTypeByString(string type)
         * convert the type text to html type enum. 
         * eg: button to HTMLTestObjectType.Button
         */
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

        /* HTMLTestObject BuildObjectByType(IHTMLElement element)
         * build the actual test object by an IHTMLElement for different type.
         * It will call the actual constructor of each test object.
         */
        private HTMLTestObject BuildObjectByType(IHTMLElement element)
        {
            HTMLTestObjectType type = HTMLTestObject.GetObjectType(element);

            return BuildObjectByType(element, type);
        }

        private HTMLTestObject BuildObjectByType(IHTMLElement element, HTMLTestObjectType type)
        {

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
                case HTMLTestObjectType.Image:
                    tmp = new HTMLTestImage(element);
                    break;
                case HTMLTestObjectType.RadioButton:
                    tmp = new HTMLTestRadioButton(element);
                    break;
                case HTMLTestObjectType.CheckBox:
                    tmp = new HTMLTestCheckBox(element);
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
