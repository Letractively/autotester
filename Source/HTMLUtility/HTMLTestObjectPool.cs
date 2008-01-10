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
* History: 2007/09/04 wan,yu Init version.
*          2007/12/21 wan,yu update for "button" object. 
*          2007/12/24 wan,yu divide cache to Function compent
*          2007/12/24 wan,yu add fuzzy search, we don't need to match the value 100%.
*          2008/01/06 wan,yu update fuzzt search, we will try lower similar percent if object is not found.                
*          2008/01/08 wan,yu add CheckButtonObject to check button object.                
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

using mshtml;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Core;

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
        private static bool _useCache = true;
        private const string _keySplitter = "__shrinerain__";

        //the default similar pencent to  find an object, if 100, that means we should make sure 100% match. 
        private static bool _autoAdjustSimilarPercent = true;
        private static int _similarPercentUpBound = 100;
        private static int _similarPercentLowBound = 70;
        private static int _similarPercentStep = 10;
        private static int _customSimilarPercent = 100;

        //IHTMLElement is the interface for mshtml html object. We build actual test object on IHTMLElement.
        private IHTMLElement _tempElement;

        //IHTMLElementCollection is an array of IHTMLElement, some functions return the array of IHTMLElement.
        private IHTMLElementCollection _allElements;

        private object[] _allObjects;

        //current object used.
        private TestObject _testObj;

        //the max time we need to wait, eg: we may wait for 30s to find a test object.
        private int _maxWaitSeconds = 30;

        //very time we sleep for 3 seconds, and find again.
        private const int _interval = 3;

        //buf to store the key for cache. 
        private static StringBuilder _keySB = new StringBuilder(128);

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

        //flag to determin if we should use cache to store test object.
        public bool UseCache
        {
            get { return HTMLTestObjectPool._useCache; }
            set { HTMLTestObjectPool._useCache = value; }
        }

        public int MaxWaitSeconds
        {
            get { return _maxWaitSeconds; }
            set { _maxWaitSeconds = value; }
        }

        //set the custom similary percent.
        public int SimilarPercent
        {
            get { return HTMLTestObjectPool._customSimilarPercent; }
            set
            {
                _autoAdjustSimilarPercent = false;
                HTMLTestObjectPool._customSimilarPercent = value;
                Searcher.DefaultPercent = value;
            }
        }

        public bool AutoAdjustSimilarPercent
        {
            get { return HTMLTestObjectPool._autoAdjustSimilarPercent; }
            set { HTMLTestObjectPool._autoAdjustSimilarPercent = value; }
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


        #region ITestObjectPool
        /* Object GetObjectByID(string id)
         * return the test object by .id property.
         *
         */
        public Object GetObjectByID(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ObjectNotFoundException("Can not find object by id: id can not be empty.");
            }

            id = id.Trim();

            //first, we will try get object from cache --- a hash table.
            string key = GetKey(id);

            if (ObjectCache.GetObjectFromCache(key, out _testObj))
            {
                return _testObj;
            }

            //we will try 30 seconds to find an object.
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    //get IHTMLElement interface
                    _tempElement = HTMLTestBrowser.GetObjectByID(id);

                    //build actual test object.
                    if (!IsInteractive(_tempElement))
                    {
                        continue;
                    }

                    _testObj = BuildObjectByType(_tempElement);

                    ObjectCache.InsertObjectToCache(key, _testObj);

                    return _testObj;


                }
                catch (CannotBuildObjectException)
                {
                    throw;
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
            if (string.IsNullOrEmpty(name))
            {
                throw new ObjectNotFoundException("Can not find object by name: name can not be empty.");
            }

            name = name.Trim();

            string key = GetKey(name);
            if (ObjectCache.GetObjectFromCache(key, out _testObj))
            {
                return _testObj;
            }

            //we will try 30s to find a object
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    object nameObj = (object)name;
                    object indexObj = (object)0;

                    //get the object collection with the same name.
                    // in HTML, .id is unique, but .name is not, so we may get a collection.
                    _tempElement = (IHTMLElement)HTMLTestBrowser.GetObjectsByName(name).item(nameObj, indexObj);

                    if (!IsInteractive(_tempElement))
                    {
                        continue;
                    }

                    _testObj = BuildObjectByType(_tempElement);

                    ObjectCache.InsertObjectToCache(key, _testObj);

                    return _testObj;


                }
                catch (CannotBuildObjectException)
                {
                    throw;
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
            if (index < 0)
            {
                index = 0;
            }

            string key = GetKey(index.ToString());

            if (ObjectCache.GetObjectFromCache(key, out _testObj))
            {
                return _testObj;
            }

            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    GetAllObjects();

                    _tempElement = (IHTMLElement)_allObjects[index];

                    if (!IsInteractive(_tempElement))
                    {
                        continue;
                    }

                    _testObj = BuildObjectByType(_tempElement);

                    ObjectCache.InsertObjectToCache(key, _testObj);

                    return _testObj;


                }
                catch (CannotBuildObjectException)
                {
                    throw;
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
         * we will use "Fuzzy Search" in this method
         */
        public Object GetObjectByProperty(string property, string value)
        {

            if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(value))
            {
                throw new PropertyNotFoundException("Property and Value can not be empty.");
            }

            value = value.Trim();

            string key = GetKey(property + value);
            if (ObjectCache.GetObjectFromCache(key, out _testObj))
            {
                return _testObj;
            }

            //if user input the property start with "." like ".id", we think it is a mistake, remove "."
            while (property.StartsWith("."))
            {
                property = property.Remove(0, 1);
            }

            //the similar percent to find an object.
            int simPercent;
            if (_autoAdjustSimilarPercent)
            {
                simPercent = _similarPercentUpBound;
            }
            else
            {
                simPercent = _customSimilarPercent;
            }

            //we will try 120s to find an object.
            int times = 0;
            while (times < _maxWaitSeconds)
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

                        //if not interactive object or the property is not found. 
                        if (!IsInteractive(_tempElement) || _tempElement.getAttribute(property, 0) == null || _tempElement.getAttribute(property, 0).GetType().ToString() == "System.DBNull")
                        {
                            continue;
                        }

                        //get property value
                        string propertyValue = _tempElement.getAttribute(property, 0).ToString().Trim();

                        if (!String.IsNullOrEmpty(propertyValue))
                        {
                            //if equal, means we found it.
                            if (Searcher.IsStringLike(propertyValue, value, simPercent))
                            {
                                _testObj = BuildObjectByType(_tempElement);

                                ObjectCache.InsertObjectToCache(key, _testObj);

                                return _testObj;
                            }
                        }

                    }
                    catch (CannotBuildObjectException)
                    {
                        throw;
                    }
                    catch
                    {
                    }
                }

                times += _interval;
                Thread.Sleep(_interval * 1000);

                //while current simpercent is bigger than the low bound,we can still try lower similarity
                if (_autoAdjustSimilarPercent)
                {
                    if (simPercent > _similarPercentLowBound)
                    {
                        simPercent -= _similarPercentStep;
                    }
                    else
                    {
                        simPercent = _similarPercentUpBound;
                    }
                }
            }

            throw new ObjectNotFoundException("Can not find object by property[" + property + "] with value [" + value + "].");
        }

        /* Object GetObjectBySimilarProperties(string[] properties, string[] values, int[] similarity, bool useAll)
         * return the test object by propeties if the value match the similarity of actual value.
         * similarity should be between 1 and 100, means 1% to 100%.
         * NOTICE: this method is much slower than GetObjectByProperty.
         */
        public Object GetObjectBySimilarProperties(string[] properties, string[] values, int[] similarity, bool useAll)
        {
            if (properties == null || values == null)
            {
                throw new ObjectNotFoundException("Properties and values can not be null.");
            }
            else if (properties.Length != values.Length)
            {
                throw new ObjectNotFoundException("Properties and values must have the same count of items.");
            }

            string key = GetKey(properties.ToString() + values.ToString() + similarity.ToString() + useAll.ToString());

            if (ObjectCache.GetObjectFromCache(key, out _testObj))
            {
                return _testObj;
            }

            int times = 0;
            while (times < _maxWaitSeconds)
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

                        if (!IsInteractive(_tempElement))
                        {
                            continue;
                        }

                        for (int j = 0; j < properties.Length; j++)
                        {
                            string property = properties[j].Trim();
                            string value = values[j].Trim();

                            //if user input the property start with "." like ".id", we think it is a mistake, remove "."
                            while (property.StartsWith("."))
                            {
                                property = property.Remove(0, 1);
                            }

                            if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value))
                            {
                                if (useAll)
                                {
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            //if not interactive object or the property is not found. 
                            if (_tempElement.getAttribute(property, 0) == null || _tempElement.getAttribute(property, 0).GetType().ToString() == "System.DBNull")
                            {
                                if (useAll)
                                {
                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            //get property value
                            string propertyValue = _tempElement.getAttribute(property, 0).ToString().Trim();

                            if (!String.IsNullOrEmpty(propertyValue))
                            {

                                int expectexPercent = 100;
                                if (similarity.Length > j)
                                {
                                    expectexPercent = similarity[j];
                                }

                                //check the similarity.
                                if (Searcher.IsStringLike(propertyValue, value, expectexPercent))
                                {
                                    if (useAll && j < properties.Length - 1)
                                    {
                                        continue;
                                    }

                                    _testObj = BuildObjectByType(_tempElement);

                                    ObjectCache.InsertObjectToCache(key, _testObj);

                                    return _testObj;

                                }
                                else
                                {
                                    if (useAll)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                    catch (CannotBuildObjectException)
                    {
                        throw;
                    }
                    catch
                    {
                    }
                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not find object by SimilarProperties: " + key);
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
         *  we will use "Fuzzy Search" in this method
         */
        public Object GetObjectByType(string type, string values, int index)
        {
            if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(values))
            {
                throw new ObjectNotFoundException("Can not get object by type: type and values can not be empty.");
            }

            type = type.Trim();
            values = values.Trim();

            if (index < 0)
            {
                index = 0;
            }

            HTMLTestObjectType typeValue;

            //convert the TYPE text to valid internal type.
            // eg: button to HTMLTestObjectType.Button
            typeValue = GetTypeByString(type);

            if (typeValue == HTMLTestObjectType.Unknow)
            {
                throw new ObjectNotFoundException("Unknow HTML object type.");
            }

            string key = GetKey(type + values + index);

            if (ObjectCache.GetObjectFromCache(key, out _testObj))
            {
                return _testObj;
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

            //the similar percent to find an object.
            int simPercent;
            if (_autoAdjustSimilarPercent)
            {
                simPercent = _similarPercentUpBound;
            }
            else
            {
                simPercent = _customSimilarPercent;
            }

            //we will try 120s to find a object.
            int times = 0;
            while (times < _maxWaitSeconds)
            {
                //because we may convert one type to mutil tags, so check them one by one.
                foreach (string tag in tags)
                {
                    //find the object by tag.
                    _tmpElementCol = _htmlTestBrowser.GetObjectsByTagName(tag);

                    //check object one by one
                    for (int i = 0; i < _tmpElementCol.length; i++)
                    {

                        nameObj = (object)i;
                        indexObj = (object)i;

                        try
                        {
                            _tempElement = (IHTMLElement)_tmpElementCol.item(nameObj, indexObj);

                            // check if it is a interactive object.
                            if (!IsInteractive(_tempElement))
                            {
                                continue;
                            }

                            //if mutil object, check if it is the object at expectd index
                            if (isByIndex)
                            {
                                leftIndex--;
                            }
                            else if (CheckObjectByType(_tempElement, typeValue, values, simPercent)) //check object by type
                            {
                                leftIndex--;
                            }

                            //if index is 0 , that means we found the object.
                            if (leftIndex < 0)
                            {
                                _testObj = BuildObjectByType(_tempElement, typeValue);

                                ObjectCache.InsertObjectToCache(key, _testObj);

                                return _testObj;
                            }


                        }
                        catch (CannotBuildObjectException)
                        {
                            throw;
                        }
                        catch
                        {
                        }
                    }
                }

                //not found, sleep 3 seconds, then try again.
                times += _interval;
                Thread.Sleep(_interval * 1000);

                //not found, we will try lower similarity
                if (_autoAdjustSimilarPercent)
                {
                    if (simPercent > _similarPercentLowBound)
                    {
                        simPercent -= _similarPercentStep;
                    }
                    else
                    {
                        simPercent = _similarPercentUpBound;
                    }
                }
            }

            throw new ObjectNotFoundException("Can not find object by type [" + type.ToString() + "] with value [" + values.ToString() + "]");
        }

        /* Object GetObjectByAI(string value)
         * return object by value, we will use some common property to find the control.
         * eg: use .value for a button
         */
        public Object GetObjectByAI(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ObjectNotFoundException("Can not find object by AI: value can not be empty.");
            }

            value = value.Trim();

            //we will try some normal properties to find the object.
            string[] possibleProperties = new string[] { "value", "innerText", "innerHTML", "title" };

            TestObject tmpObj;

            int lastMaxSnds = _maxWaitSeconds;

            foreach (string s in possibleProperties)
            {
                try
                {
                    _maxWaitSeconds = 3;

                    tmpObj = (TestObject)GetObjectByProperty(s, value);
                }
                catch (ObjectNotFoundException)
                {
                    continue;
                }
                finally
                {
                    _maxWaitSeconds = lastMaxSnds;
                }

                if (tmpObj != null)
                {
                    _testObj = tmpObj;
                    return _testObj;
                }
            }

            throw new ObjectNotFoundException("Can not find object by AI.");
        }


        /* Object GetObjectByPoint(int x, int y)
         * return object from a expected point
         * 
         */
        public Object GetObjectByPoint(int x, int y)
        {
            string key = GetKey(x.ToString() + " " + y.ToString());
            if (ObjectCache.GetObjectFromCache(key, out _testObj))
            {
                return _testObj;
            }

            int times = 0;
            while (times < _maxWaitSeconds)
            {
                try
                {
                    _tempElement = this._htmlTestBrowser.GetObjectFromPoint(x, y);

                    if (!IsInteractive(_tempElement))
                    {
                        continue;
                    }

                    _testObj = BuildObjectByType(_tempElement);

                    ObjectCache.InsertObjectToCache(key, _testObj);

                    return _testObj;
                }
                catch (CannotBuildObjectException)
                {
                    throw;
                }
                catch
                {
                }

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object at point(" + x.ToString() + "" + y.ToString() + ")");
        }

        /* Object GetObjectByRect(int top, int left, int width, int height)
         * return object from a expected rect.
         * like QTP virtual object.
         * NOTICE: need update!!!
         */
        public Object GetObjectByRect(int top, int left, int width, int height, string type)
        {
            return null;
        }

        /* Object GetObjectByColor(string color)
         * return object by expected color
         * NOTICE: need update!!!
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
                    return new string[] { "input", "button", "img" };
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
            if (_needRefresh || _allElements == null)
            {
                _needRefresh = false;

                try
                {
                    this._allElements = HTMLTestBrowser.GetAllObjects();
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
        private static bool CheckObjectByType(IHTMLElement element, HTMLTestObjectType type, string value, int simPercent)
        {

            //check special types
            if (type == HTMLTestObjectType.ListBox || type == HTMLTestObjectType.ComboBox)
            {
                return CheckSelectObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.RadioButton)
            {
                return CheckRadioObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.CheckBox)
            {
                return CheckBoxObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.Image)
            {
                return CheckImageObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.Button)
            {
                return CheckButtonObject(element, value, simPercent);
            }

            string tag = element.tagName.ToString();

            //get default property of each tag
            //eg: for a textbox, the property is .value
            string propertyName = GetVisibleTextPropertyByTag(type, tag);

            if (String.IsNullOrEmpty(propertyName))
            {
                return false;
            }
            else if (element.getAttribute(propertyName, 0) == null || element.getAttribute(propertyName, 0).GetType().ToString() == "System.DBNull")
            {
                return false;
            }

            //not special type, just need to check some property.
            return IsPropertyLike(element, propertyName, value, simPercent);
        }

        /*  bool CheckSelectObject(IHTMLElement element, string value)
         *  check select object like ListBox , Combobox etc.
         *  for select object, we need to check it's first item.
         */
        private static bool CheckSelectObject(IHTMLElement element, string value, int simPercent)
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

                return Searcher.IsStringLike(firstItem, value, simPercent);
            }
            catch
            {
                return false;
            }

        }

        /* bool CheckImageObject(IHTMLElement element, string value)
         * return true if the src of img is ends witdh the input value.
         */
        private static bool CheckImageObject(IHTMLElement element, string value, int simPercent)
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

                imgSrc = System.IO.Path.GetFileName(imgSrc);

                return Searcher.IsStringLike(imgSrc, value, simPercent);

            }
            catch
            {
                return false;
            }

        }

        /* bool CheckRadioObject(IHTMLElement element, string value)
         * Check radio button.
         * we have 2 way to check a radio button.
         * 1. check it's .value property.
         * 2. check it's label. label is the text, you click the text makes you click the radio button
         */
        private static bool CheckRadioObject(IHTMLElement element, string value, int simPercent)
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

                return IsPropertyLike(element, propertyName, value, simPercent);
            }
            catch
            {
                return false;
            }

        }

        /* bool CheckBoxObject(IHTMLElement element, string value)
         * check check box.
         * we have 2 ways to check a check box
         * 1. check it's .value property.
         * 2. check it's label
         */
        private static bool CheckBoxObject(IHTMLElement element, string value, int simPercent)
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

                return IsPropertyLike(element, propertyName, value, simPercent);

            }
            catch
            {
                return false;
            }
        }

        /* bool CheckButtonObject(IHTMLElement element, string value, int simPercent)
         * check button object.
         */
        private static bool CheckButtonObject(IHTMLElement element, string value, int simPercent)
        {
            string buttonTypeProperty = null;
            string propertyName = null;

            if (element.tagName == "INPUT")
            {
                buttonTypeProperty = "type";
                if (element.getAttribute(buttonTypeProperty, 0) != null && element.getAttribute(buttonTypeProperty, 0).GetType().ToString() != "System.DBNull")
                {
                    //we need to skip <input type="text"> and <input type="password">
                    if (String.Compare(element.getAttribute(buttonTypeProperty, 0).ToString(), "text", true) == 0 || String.Compare(element.getAttribute(buttonTypeProperty, 0).ToString(), "password", true) == 0)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                propertyName = "value";
            }
            else if (element.tagName == "IMG")
            {
                buttonTypeProperty = "onclick";

                //if "onclick" is not found, we think it is a normal img.
                if (element.getAttribute(buttonTypeProperty, 0) == null || element.getAttribute(buttonTypeProperty, 0).GetType().ToString() == "System.DBNull")
                {
                    return false;
                }

                propertyName = "src";
            }

            return IsPropertyLike(element, propertyName, value, simPercent);
        }

        /* bool IsPropertyEqual(IHTMLElement element, string propertyName, string value, int simPercent)
         * check if the property == value with the expected similar percent.
         */
        private static bool IsPropertyLike(IHTMLElement element, string propertyName, string value, int simPercent)
        {
            if (element.getAttribute(propertyName, 0) != null && element.getAttribute(propertyName, 0).GetType().ToString() != "System.DBNull")
            {
                string actualValue = element.getAttribute(propertyName, 0).ToString().Trim();

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
        private static string GetVisibleTextPropertyByTag(HTMLTestObjectType type, string tag)
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

            if (type == "BUTTON" || type == "BTN" || type == "B")
            {
                return HTMLTestObjectType.Button;
            }
            else if (type == "TEXTBOX" || type == "TEXT" || type == "INPUTBOX" || type == "TXT" || type == "T")
            {
                return HTMLTestObjectType.TextBox;
            }
            else if (type == "LINK" || type == "HYPERLINK" || type == "LK")
            {
                return HTMLTestObjectType.Link;
            }
            else if (type == "IMAGE" || type == "IMG" || type == "PICTURE" || type == "PIC" || type == "I" || type == "P")
            {
                return HTMLTestObjectType.Image;
            }
            else if (type == "COMBOBOX" || type == "DROPDOWNBOX" || type == "DROPDOWNLIST" || type == "DROPDOWN" || type == "CB")
            {
                return HTMLTestObjectType.ComboBox;
            }
            else if (type == "LISTBOX" || type == "LIST" || type == "LST" || type == "LS")
            {
                return HTMLTestObjectType.ListBox;
            }
            else if (type == "RADIOBOX" || type == "RADIOBUTTON" || type == "RADIO" || type == "RAD" || type == "R")
            {
                return HTMLTestObjectType.RadioButton;
            }
            else if (type == "CHECKBOX" || type == "CHECK" || type == "CHK" || type == "CK")
            {
                return HTMLTestObjectType.CheckBox;
            }
            else if (type == "FILEDIAGLOG" || type == "FILE" || type == "FOLDER" || type == "FOLDERDIALOG" || type == "F")
            {
                return HTMLTestObjectType.FileDialog;
            }
            else if (type == "ACTIVEX")
            {
                return HTMLTestObjectType.ActiveX;
            }
            else if (type == "MSGBOX" || type == "MSG" || type == "MESSAGE" || type == "MESSAGEBOX" || type == "POPWINDOW" || type == "POPBOX" || type == "M")
            {
                return HTMLTestObjectType.MsgBox;
            }
            else if (type == "TABLE" || type == "TBL" || type == "T")
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
        private static HTMLGuiTestObject BuildObjectByType(IHTMLElement element)
        {
            HTMLTestObjectType type = HTMLTestObject.GetObjectType(element);

            return BuildObjectByType(element, type);
        }

        private static HTMLGuiTestObject BuildObjectByType(IHTMLElement element, HTMLTestObjectType type)
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
                case HTMLTestObjectType.MsgBox:
                    tmp = new HTMLTestMsgBox();
                    break;
                case HTMLTestObjectType.FileDialog:
                    tmp = new HTMLTestFileDialog();
                    break;
                default:
                    tmp = null;
                    break;
            }

            return tmp;
        }

        #region object cache

        /* string GetKey(string info)
         * generate key for hash table cache.
         */
        private string GetKey(string info)
        {
            try
            {
                //clear last key.
                if (_keySB.Length > 0)
                {
                    _keySB.Remove(0, _keySB.Length);
                }

                _keySB.Append(this._htmlTestBrowser.GetCurrentUrl());
                _keySB.Append(_keySplitter);
                _keySB.Append(info);

                return _keySB.ToString();
            }
            catch
            {
                return info;
            }

        }

        #endregion

        #endregion

        #endregion
    }
}
