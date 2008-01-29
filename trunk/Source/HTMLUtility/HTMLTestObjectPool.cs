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
*          2007/12/21 wan,yu udpate, update for "button" object. 
*          2007/12/24 wan,yu update, divide cache to Core compent
*          2007/12/24 wan,yu update, add fuzzy search, we don't need to match the value 100%.
*          2008/01/06 wan,yu update, update fuzzy search, we will try lower similar percent if object is not found.                
*          2008/01/08 wan,yu update, add CheckButtonObject to check button object.     
*          2008/01/10 wan,yu update, update GetTypeByString() method, accept more strings.   
*          2008/01/10 wan,yu update, move GetObjectType from HTMLTestObject.cs to HTMLTestObjectPool.cs  
*          2008/01/12 wan,yu update, add CheckTableObject, we can find the <table> object.
*          2008/01/12 wan,yu update, add CheckMsgBoxObject and CheckFileDialogObject         
*          2008/01/12 wan,yu update, bug fix for GetObjectByName, origin version will just check the first one. 
*          2008/01/15 wan,yu update, modify some static members to instance. 
*          2008/01/21 wan,yu update, modify to use HTMLTestObject.TryGetValueByProperty to check something. 
*          2008/01/21 wan,yu update, add CheckLabelObject.          
*          2008/01/22 wan,yu update, add CheckTextBoxObject.          
*
*********************************************************************/


using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Helper;
using Shrinerain.AutoTester.Win32;

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
        private bool _useCache = true;
        private const string _keySplitter = "__shrinerain__";

        //the default similar pencent to  find an object, if 100, that means we should make sure 100% match. 
        private bool _autoAdjustSimilarPercent = true;
        private int _similarPercentUpBound = 100;
        private int _similarPercentLowBound = 70;
        private int _similarPercentStep = 10;
        private int _customSimilarPercent = 100;

        //IHTMLElement is the interface for mshtml html object. We build actual test object on IHTMLElement.
        private IHTMLElement _tempElement;

        //IHTMLElementCollection is an array of IHTMLElement, some functions return the array of IHTMLElement.
        private IHTMLElementCollection _allElements;

        private object[] _allObjects;

        //current object used.
        private TestObject _testObj;
        private Object _cacheObj;

        //the max time we need to wait, eg: we may wait for 30s to find a test object.
        private int _maxWaitSeconds = 30;

        //very time we sleep for 3 seconds, and find again.
        private const int _interval = 3;

        //buf to store the key for cache. 
        private static StringBuilder _keySB = new StringBuilder(128);

        //regex to match tag
        private static Regex _tagReg = new Regex("<[a-zA-Z]+ ", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //if this flag set to ture, we will ignore those tables whose "border" < 1.
        private static bool _ignoreBorderlessTable = false;

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
            get { return _useCache; }
            set { _useCache = value; }
        }

        public int MaxWaitSeconds
        {
            get { return _maxWaitSeconds; }
            set { _maxWaitSeconds = value; }
        }

        //set the custom similary percent.
        public int SimilarPercent
        {
            get { return _customSimilarPercent; }
            set
            {
                _autoAdjustSimilarPercent = false;
                _customSimilarPercent = value;
                Searcher.DefaultPercent = value;
            }
        }

        public bool AutoAdjustSimilarPercent
        {
            get { return _autoAdjustSimilarPercent; }
            set { _autoAdjustSimilarPercent = value; }
        }


        public bool IgnoreBorderlessTable
        {
            get { return _ignoreBorderlessTable; }
            set { _ignoreBorderlessTable = value; }
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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (String.IsNullOrEmpty(id))
            {
                throw new ObjectNotFoundException("Can not find object by id: id can not be empty.");
            }

            id = id.Trim();

            //first, we will try get object from cache --- a hash table.
            string key = GetKey(id);

            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
            }

            //we will try 30 seconds to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    //get IHTMLElement interface
                    _tempElement = _htmlTestBrowser.GetObjectByID(id);

                    if (_tempElement != null)
                    {

                        //build actual test object.
                        _testObj = BuildObjectByType(_tempElement);

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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ObjectNotFoundException("Can not find object by name: name can not be empty.");
            }

            name = name.Trim();

            string key = GetKey(name);
            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
            }

            //we will try 30s to find a object
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    //2008/01/12 wan,yu update
                    //getObjectByName will return more than 1 object(if have), so we need a collection to
                    //store these objects.
                    //we will check each object.
                    IHTMLElementCollection nameObjectsCol = _htmlTestBrowser.GetObjectsByName(name);

                    for (int i = 0; i < nameObjectsCol.length; i++)
                    {
                        object nameObj = (object)i;
                        object indexObj = (object)i;

                        //get the object collection with the same name.
                        // in HTML, .id is unique, but .name is not, so we may get a collection.
                        _tempElement = (IHTMLElement)nameObjectsCol.item(nameObj, indexObj);


                        if (!IsVisible(_tempElement))
                        {
                            continue;
                        }

                        _testObj = BuildObjectByType(_tempElement);

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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (index < 0)
            {
                index = 0;
            }

            string key = GetKey(index.ToString());

            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
            }

            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    GetAllElements();

                    _tempElement = (IHTMLElement)_allElements.item((object)index, (object)index);

                    if (_tempElement != null)
                    {
                        if (!IsVisible(_tempElement))
                        {
                            //this object is not interactive, we will try next object.
                            index++;
                            continue;
                        }

                        _testObj = BuildObjectByType(_tempElement);

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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(value))
            {
                throw new PropertyNotFoundException("Property and Value can not be empty.");
            }

            value = value.Trim();

            string key = GetKey(property + value);
            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
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

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {

                //get all HTML objects.
                GetAllElements();

                object nameObj = null;
                object indexObj = null;

                //go through all element, check it's property value.
                for (int i = 0; i < _allElements.length; i++)
                {
                    try
                    {
                        nameObj = (object)i;
                        indexObj = (object)i;

                        //get element by index.
                        _tempElement = (IHTMLElement)_allElements.item(nameObj, indexObj);

                        //get property value
                        string propertyValue;

                        //if not interactive object or the property is not found. 
                        if (!HTMLTestObject.TryGetValueByProperty(_tempElement, property, out propertyValue) || !IsVisible(_tempElement))
                        {
                            continue;
                        }
                        else
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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (properties == null || values == null)
            {
                throw new ObjectNotFoundException("Properties and values can not be null.");
            }
            else if (properties.Length != values.Length)
            {
                throw new ObjectNotFoundException("Properties and values must have the same count of items.");
            }

            string key = GetKey(properties.ToString() + values.ToString() + similarity.ToString() + useAll.ToString());

            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
            }

            int times = 0;
            while (times <= _maxWaitSeconds)
            {

                //get all HTML objects.
                GetAllElements();

                object nameObj = null;
                object indexObj = null;

                //go through all element, check it's property value.
                for (int i = 0; i < _allElements.length; i++)
                {
                    try
                    {
                        nameObj = (object)i;
                        indexObj = (object)i;

                        //get element by index.
                        _tempElement = (IHTMLElement)_allElements.item(nameObj, indexObj);

                        if (!IsVisible(_tempElement))
                        {
                            continue;
                        }

                        for (int j = 0; j < properties.Length; j++)
                        {
                            string property = properties[j].Trim();
                            string value = values[j].Trim();

                            string propertyValue;
                            //if not interactive object or the property is not found. 
                            if (String.IsNullOrEmpty(property) || String.IsNullOrEmpty(value)
                                || !HTMLTestObject.TryGetValueByProperty(_tempElement, property, out propertyValue))
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
                            else
                            {
                                int expectedPercent = 100;

                                if (similarity.Length > j)
                                {
                                    expectedPercent = similarity[j];
                                }

                                //check the similarity.
                                if (Searcher.IsStringLike(propertyValue, value, expectedPercent))
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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (String.IsNullOrEmpty(type))
            {
                throw new ObjectNotFoundException("Can not get object by type: type can not be empty.");
            }

            type = type.Trim();

            if (index < 0)
            {
                index = 0;
            }

            //try to get object from cache.
            string key = GetKey(type + values + index.ToString());

            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
            }


            //some control, like Messagebox, File dialog, they are Windows control, we don't need to check HTML DOM.
            bool isHTMLType = true;

            //convert the TYPE text to valid internal type.
            // eg: "button" to HTMLTestObjectType.Button
            HTMLTestObjectType typeValue = GetTypeByString(type);

            if (typeValue == HTMLTestObjectType.Unknow)
            {
                throw new ObjectNotFoundException("Unknow HTML object type.");
            }
            else if (typeValue == HTMLTestObjectType.MsgBox ||
                typeValue == HTMLTestObjectType.FileDialog)
            {
                isHTMLType = false;
            }

            if (isHTMLType)
            {
                //not special type, then we need values to find the object.
                if (String.IsNullOrEmpty(values))
                {
                    throw new ObjectNotFoundException("Can not get object by type: values can not be empty.");
                }
                else
                {
                    values = values.Trim();
                }
            }

            //convert the type to HTML tags.
            //eg: convert Image to <img>, Button to <input type="button">,<input type="submit">...
            string[] tags = GetObjectTags(typeValue);

            if (tags == null)
            {
                throw new CannotBuildObjectException("Tags can not be empty.");
            }

            //if the expected value is number like, we think it is stand for the index of the object, not text on it.
            // eg: we can use type="textbox", values="1" to find the first textbox.
            bool isByIndex = false;
            int objIndex;
            if (int.TryParse(values, out objIndex))
            {
                if (objIndex < 10)
                {
                    index = objIndex - 1;
                    isByIndex = true;
                }
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

            //windows handle of Message Box and File Dialog
            IntPtr handle = IntPtr.Zero;

            //we will try 30s to find a object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                //special type, we don't need to check HTML object.
                if (!isHTMLType)
                {
                    try
                    {
                        //check object by type
                        if (CheckObjectByType(ref handle, typeValue, values, simPercent))
                        {
                            leftIndex--;

                            if (leftIndex >= 0)
                            {
                                continue;
                            }
                            else
                            {
                                //if index is 0 , that means we found the object.
                                _testObj = BuildObjectByType(handle, typeValue);

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
                else
                {
                    //normal HTML objects

                    IHTMLElementCollection tagElementCol;

                    object nameObj = null;
                    object indexObj = null;

                    //because we may convert one type to multi tags, so check them one by one.
                    //eg: Button to <input> and <button>
                    foreach (string tag in tags)
                    {
                        //find the object by tag.
                        tagElementCol = _htmlTestBrowser.GetObjectsByTagName(tag);

                        //check object one by one
                        for (int i = 0; i < tagElementCol.length; i++)
                        {

                            nameObj = (object)i;
                            indexObj = (object)i;

                            try
                            {
                                _tempElement = (IHTMLElement)tagElementCol.item(nameObj, indexObj);

                                // check if it is a interactive object.
                                if (!IsVisible(_tempElement))
                                {
                                    continue;
                                }

                                //if multi object, check if it is the object at expectd index
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
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (String.IsNullOrEmpty(value))
            {
                throw new ObjectNotFoundException("Can not find object by AI: value can not be empty.");
            }

            value = value.Trim();

            //we will try some properties to find the object.
            string[] possibleProperties = new string[] { "value", "innerText", "innerHTML", "title", "id", "name" };

            TestObject tmpObj;

            int lastMaxSnds = _maxWaitSeconds;

            foreach (string s in possibleProperties)
            {
                try
                {
                    //each object we just try 3s, because we need to check other property.
                    _maxWaitSeconds = 3;

                    tmpObj = (TestObject)GetObjectByProperty(s, value);
                }
                catch (ObjectNotFoundException)
                {
                    continue;
                }
                finally
                {
                    //we need to resume the _maxWaitSeconds, because other methods not just wait for 3s.
                    _maxWaitSeconds = lastMaxSnds;
                }

                if (tmpObj != null)
                {
                    _testObj = tmpObj;

                    return _testObj;
                }
            }

            throw new ObjectNotFoundException("Can not find object by AI with value: " + value);
        }


        /* Object GetObjectByPoint(int x, int y)
         * return object from a expected point
         * x, y is the offset with browser, NOT screen.
         * 
         */
        public Object GetObjectByPoint(int x, int y)
        {
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            string key = GetKey(x.ToString() + " " + y.ToString());
            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
            }

            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    _tempElement = this._htmlTestBrowser.GetObjectFromPoint(x, y);

                    if (IsVisible(_tempElement))
                    {
                        _testObj = BuildObjectByType(_tempElement);

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

                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object at point(" + x.ToString() + "" + y.ToString() + ")");
        }

        /* Object GetObjectByRect(int top, int left, int width, int height)
         * return object from a expected rect.
         */
        public Object GetObjectByRect(int left, int top, int width, int height, string typeStr, bool isPercent)
        {
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (width < 1 || height < 1)
            {
                throw new ObjectNotFoundException("The width and height of rect can not be 0.");
            }

            HTMLTestObjectType type = GetTypeByString(typeStr);

            if (type == HTMLTestObjectType.Unknow)
            {
                throw new ObjectNotFoundException("Unknow type.");
            }

            int x = 0;
            int y = 0;

            if (isPercent)
            {
                isPercent = false;

                if ((left >= 1 && left <= 100) && (top >= 1 && top <= 100))
                {
                    x = _htmlTestBrowser.ClientWidth * left / 100;
                    y = _htmlTestBrowser.ClientHeight * top / 100;
                }
                else
                {
                    throw new ObjectNotFoundException("Left and top percent must between 1 and 100.");
                }
            }
            else
            {
                x = left + width / 2;
                y = top + height / 2;
            }

            //try to get 5 objects from different area.
            HTMLTestGUIObject[] tmpObj = new HTMLTestGUIObject[5];

            int originMaxWaitTime = this._maxWaitSeconds;

            //set the max time 
            this._maxWaitSeconds = 3;

            for (int i = 0; i < tmpObj.Length; i++)
            {
                try
                {
                    if (i == 0)
                    {
                        tmpObj[0] = (HTMLTestGUIObject)GetObjectByPoint(x, y);
                    }
                    else if (width > 3 && height > 3)
                    {
                        if (i == 1)
                        {
                            tmpObj[1] = (HTMLTestGUIObject)GetObjectByRect(x, y, width / 2, height / 2, typeStr, isPercent);
                        }
                        else if (i == 2)
                        {
                            tmpObj[2] = (HTMLTestGUIObject)GetObjectByRect(x + width / 2, y, width / 2, height / 2, typeStr, isPercent);
                        }
                        else if (i == 3)
                        {
                            tmpObj[3] = (HTMLTestGUIObject)GetObjectByRect(x + width / 2, y + height / 2, width / 2, height / 2, typeStr, isPercent);
                        }
                        else if (i == 4)
                        {
                            tmpObj[4] = (HTMLTestGUIObject)GetObjectByRect(x, y + height / 2, width / 2, height / 2, typeStr, isPercent);
                        }
                    }

                    if (tmpObj[i] != null && tmpObj[i].Type == type)
                    {
                        _testObj = tmpObj[i];

                        return _testObj;
                    }
                }
                catch
                {
                    continue;
                }
            }

            this._maxWaitSeconds = originMaxWaitTime;

            throw new ObjectNotFoundException("Can not get object by rect.");
        }

        /* Object GetObjectByColor(string color)
         * return object by expected color
         * NOTICE: need update!!!
         */
        public Object GetObjectByColor(string color)
        {
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            return null;
        }

        /* Object GetObjectByCustomer(object value)
         * for future use.
         */
        public Object GetObjectByCustomer(object value)
        {
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            return null;
        }

        /* Object[] GetAllObjects()
         * return all object from the  browser.
         * we use _allObjects to store these object.
         */
        public Object[] GetAllObjects()
        {
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

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

        /* Object GetLastObject()
         * return the last object we have got.
         */
        public Object GetLastObject()
        {
            return _testObj;
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


        #region private methods

        /* void GetAllElements()
         * load all IHTMLElement from browser.
         * just reload when the _needRefresh flag is set to ture.
         * 
         */
        private void GetAllElements()
        {
            if (_htmlTestBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (_needRefresh || _allElements == null)
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


        #region check test object

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
                return CheckCheckBoxObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.Image)
            {
                return CheckImageObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.Button)
            {
                return CheckButtonObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.Table)
            {
                return CheckTableObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.Label)
            {
                return CheckLabelObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.TextBox)
            {
                return CheckTextBoxObject(element, value, simPercent);
            }

            string tag = element.tagName;

            //get default property of each tag
            //eg: for a textbox, the property is .value
            string propertyName = GetVisibleTextPropertyByTag(type, tag);
            string propertyValue;

            if (String.IsNullOrEmpty(propertyName))
            {
                return false;
            }
            else if (!HTMLTestObject.TryGetValueByProperty(element, propertyName, out propertyValue))
            {
                return false;
            }

            //not special type, just need to check some property.
            return IsPropertyLike(element, propertyName, value, simPercent);
        }

        /* bool CheckObjectByType(ref IntPtr handle, HTMLTestObjectType type, string value, int simPercent)
         * check windows object, like message box and file dialog
         */
        private bool CheckObjectByType(ref IntPtr handle, HTMLTestObjectType type, string value, int simPercent)
        {

            if (type == HTMLTestObjectType.MsgBox)
            {
                return CheckMsgBoxObject(ref handle, value, simPercent);
            }
            else if (type == HTMLTestObjectType.FileDialog)
            {
                return CheckFileDialogObject(ref handle, value, simPercent);
            }
            else
            {
                return false;
            }

        }

        /* bool CheckTextBoxObject(IHTMLElement, string value, int simPercent)
         * check text box object, we will check text around the text box.
         */
        private static bool CheckTextBoxObject(IHTMLElement element, string value, int simPercent)
        {
            try
            {
                //fristly, check type.
                if (element == null || (element.tagName != "INPUT" && element.tagName != "TEXTAREA"))
                {
                    return false;
                }

                if (element.tagName == "INPUT")
                {
                    string type;
                    if (HTMLTestObject.TryGetValueByProperty(element, "type", out type))
                    {
                        if (String.Compare(type, "TEXT", true) != 0 && String.Compare(type, "PASSWORD", true) != 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }


                //firstly, check title.
                string title;

                if (HTMLTestObject.TryGetValueByProperty(element, "title", out title))
                {
                    if (!String.IsNullOrEmpty(title) && Searcher.IsStringLike(value, title, simPercent))
                    {
                        return true;
                    }
                }

                string label = HTMLTestTextBox.GetLabelForTextBox(element);

                //for text around textbox, we may have ":", like "UserName:" , remove ":", make sure no writing mistake.
                if (value.EndsWith(":") || value.EndsWith("£º"))
                {
                    value = value.Substring(0, value.Length - 1);
                }

                if (label.EndsWith(":") || label.EndsWith("£º"))
                {
                    label = label.Substring(0, label.Length - 1);
                }

                return Searcher.IsStringLike(value, label, simPercent);
            }
            catch
            {
                return false;
            }
        }

        /* bool CheckLabelObject(IHTMLElement element, string value, int simPercent)
         * check label object.
         * <label>,<td>,<div>,<span>
         */
        private static bool CheckLabelObject(IHTMLElement element, string value, int simPercent)
        {
            try
            {
                string propertyName = "innerHTML";

                string innerHTML;

                if (!HTMLTestObject.TryGetValueByProperty(element, propertyName, out innerHTML))
                {
                    return false;
                }
                else
                {
                    if (innerHTML.IndexOf("<") >= 0 && innerHTML.IndexOf(">") > 0)
                    {

                        MatchCollection mc = _tagReg.Matches(innerHTML);

                        foreach (Match m in mc)
                        {
                            if (String.Compare(m.Value, "<SPAN", true) != 0
                                && String.Compare(m.Value, "<FONT", true) != 0
                                && String.Compare(m.Value, "<BR", true) != 0
                                && String.Compare(m.Value, "<P", true) != 0)
                            {
                                return false;
                            }
                        }

                    }

                    string innerText;

                    if (!HTMLTestObject.TryGetValueByProperty(element, "innerText", out innerText))
                    {
                        return false;
                    }
                    else
                    {
                        return Searcher.IsStringLike(value, innerText, simPercent);
                    }

                }

            }
            catch
            {
                return false;
            }
        }

        /*  bool CheckSelectObject(IHTMLElement element, string value)
         *  check select object like ListBox , Combobox etc.
         *  for select object, we need to check it's first item.
         */
        private static bool CheckSelectObject(IHTMLElement element, string value, int simPercent)
        {

            try
            {
                if (element == null || element.tagName != "SELECT")
                {
                    return false;
                }

                string propertyName = "innerHTML";

                string items;

                if (!HTMLTestObject.TryGetValueByProperty(element, propertyName, out items))
                {
                    return false;
                }

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
                if (element == null || element.tagName != "IMG")
                {
                    return false;
                }

                //check the .src property.
                string propertyName = "src";

                string imgSrc;

                if (!HTMLTestObject.TryGetValueByProperty(element, propertyName, out imgSrc))
                {
                    return false;
                }

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
                if (element == null || element.tagName != "INPUT")
                {
                    return false;
                }

                if (element.tagName == "INPUT")
                {
                    string type;
                    if (HTMLTestObject.TryGetValueByProperty(element, "type", out type))
                    {
                        if (String.Compare(type, "radio", true) != 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                string labelText = HTMLTestRadioButton.GetLabelForRadioBox(element);

                if (!String.IsNullOrEmpty(labelText))
                {
                    if (value.EndsWith(".") || value.EndsWith("¡£"))
                    {
                        value = value.Substring(0, value.Length - 1);
                    }

                    if (labelText.EndsWith(".") || labelText.EndsWith("¡£"))
                    {
                        labelText = value.Substring(0, labelText.Length - 1);
                    }

                    if (Searcher.IsStringLike(labelText, value, simPercent))
                    {
                        return true;
                    }
                }

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

        /* bool CheckCheckBoxObject(IHTMLElement element, string value)
         * check check box.
         * we have 2 ways to check a check box
         * 1. check it's .value property.
         * 2. check it's label
         */
        private static bool CheckCheckBoxObject(IHTMLElement element, string value, int simPercent)
        {
            try
            {
                if (element == null || element.tagName != "INPUT")
                {
                    return false;
                }

                if (element.tagName == "INPUT")
                {
                    string type;
                    if (HTMLTestObject.TryGetValueByProperty(element, "type", out type))
                    {
                        if (String.Compare(type, "checkbox", true) != 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                string labelText = HTMLTestCheckBox.GetLabelForCheckBox(element);

                if (!String.IsNullOrEmpty(labelText))
                {
                    if (value.EndsWith(".") || value.EndsWith("¡£"))
                    {
                        value = value.Substring(0, value.Length - 1);
                    }

                    if (labelText.EndsWith(".") || labelText.EndsWith("¡£"))
                    {
                        labelText = value.Substring(0, labelText.Length - 1);
                    }

                    if (Searcher.IsStringLike(labelText, value, simPercent))
                    {
                        return true;
                    }
                }

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
            try
            {
                if (element == null || (element.tagName != "INPUT" && element.tagName != "BUTTON"))
                {
                    return false;
                }

                string buttonTypeProperty = null;
                string propertyName = null;
                string buttonTypeValue;

                if (element.tagName == "INPUT")
                {
                    buttonTypeProperty = "type";

                    if (HTMLTestObject.TryGetValueByProperty(element, buttonTypeProperty, out buttonTypeValue))
                    {
                        if (String.Compare(buttonTypeValue, "text", true) == 0 || String.Compare(buttonTypeValue, "password", true) == 0
                            || String.Compare(buttonTypeValue, "checkbox", true) == 0 || String.Compare(buttonTypeValue, "radio", true) == 0)
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
                    if (!HTMLTestObject.TryGetValueByProperty(element, buttonTypeProperty, out buttonTypeValue))
                    {
                        return false;
                    }

                    propertyName = "src";
                }

                return IsPropertyLike(element, propertyName, value, simPercent);
            }
            catch
            {
                return false;
            }
        }

        /* bool CheckTableObject(IHTMLElement element, string value, int simPercent)
         * check the test table <table>
         * we get the most left-top cell of the table, compare it's "innerText" with the expected value,
         * if match, return true.
         */
        private static bool CheckTableObject(IHTMLElement element, string value, int simPercent)
        {
            if (element.tagName != "TABLE")
            {
                return false;
            }

            try
            {
                //because in HTML page, we may use table to control layout, and these tables are not for displaying data.
                //so I decide to just check those tables whose "borser" > 0, that means we can "see" this table directly.
                //if the table doesn't have a "border" property, I think it is just for layout, not data.
                if (_ignoreBorderlessTable)
                {
                    string border;

                    if (!HTMLTestObject.TryGetValueByProperty(element, "border", out border))
                    {
                        return false;
                    }
                    else
                    {
                        if (Convert.ToInt32(border) < 1)
                        {
                            return false;
                        }
                    }
                }


                //check caption of the table.
                string caption;

                if (HTMLTestObject.TryGetValueByProperty(element, "caption", out caption))
                {
                    if (!String.IsNullOrEmpty(caption) && Searcher.IsStringLike(caption, value, simPercent))
                    {
                        return true;
                    }
                }

                //get table.
                IHTMLTable tableElement = (IHTMLTable)element;

                //get the first row.
                IHTMLTableRow tableRowElement = (IHTMLTableRow)tableElement.rows.item((object)0, (object)0);

                //get the first cell.
                IHTMLElement cellElement = (IHTMLElement)tableRowElement.cells.item((object)0, (object)0);

                string innerText;

                if (HTMLTestObject.TryGetValueByProperty(cellElement, "innerText", out innerText))
                {
                    return Searcher.IsStringLike(innerText, value, simPercent);
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }
        }

        /* bool CheckMsgBoxObject(string value, int simPercent)
         * check pop up message box.
         * it is a window control, not a HTML control, so we don't need IHTMLElement.
         * NEED UPDATE!!!
         */
        private bool CheckMsgBoxObject(ref IntPtr handle, string value, int simPercent)
        {

            //get the handle, the text of MessageBox is "Windows Internet Explorer", we use this to find it.
            handle = Win32API.FindWindowEx(_htmlTestBrowser.MainHandle, handle, null, "Windows Internet Explorer");

            return handle != IntPtr.Zero;
        }

        /* bool CheckFileDialogObject(string value, int simPercent)
         * check file dialog, when you need input/browse file/folder, we can see this control.
         * it is a windows control, not a HTML control, so we don't need IHTMLElement.
         */
        private bool CheckFileDialogObject(ref IntPtr handle, string value, int simPercent)
        {
            handle = IntPtr.Zero;
            return false;
        }

        #endregion

        /* bool IsPropertyEqual(IHTMLElement element, string propertyName, string value, int simPercent)
         * check if the property == value with the expected similar percent.
         */
        private static bool IsPropertyLike(IHTMLElement element, string propertyName, string value, int simPercent)
        {
            string actualValue;

            if (HTMLTestObject.TryGetValueByProperty(element, propertyName, out actualValue))
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
        private static bool IsVisible(IHTMLElement element)
        {
            if (element == null)
            {
                return false;
            }

            string tag = element.tagName;

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

            string value;

            if (tag == "INPUT")
            {
                //return false, if the it is a hidden object.
                if (HTMLTestObject.TryGetValueByProperty(element, "type", out value))
                {
                    return String.Compare(value, "HIDDEN", true) != 0;
                }
            }
            else if (HTMLTestObject.TryGetValueByProperty(element, "visibility", out value))
            {
                //return false if it is hidden.
                return String.Compare(value, "HIDDEN", true) != 0;
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
            else if (type == "LABEL" || type == "LB")
            {
                return HTMLTestObjectType.Label;
            }
            else if (type == "TEXTBOX" || type == "TEXT" || type == "INPUTBOX" || type == "TXT" || type == "T")
            {
                return HTMLTestObjectType.TextBox;
            }
            else if (type == "LINK" || type == "HYPERLINK" || type == "LK" || type == "A")
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


        /*  string[] GetObjectTags(HTMLTestObjectType type)
        *  convert HTMLTestObjectType to HTML tags.
        */
        private static string[] GetObjectTags(HTMLTestObjectType type)
        {
            switch (type)
            {
                case HTMLTestObjectType.Label:
                    return new string[] { "label", "span", "td", "div" };
                case HTMLTestObjectType.Link:
                    return new string[] { "a" };
                case HTMLTestObjectType.Button:
                    return new string[] { "input", "button" };
                case HTMLTestObjectType.TextBox:
                    return new string[] { "input", "textarea" };
                case HTMLTestObjectType.Image:
                    return new string[] { "img" };
                case HTMLTestObjectType.CheckBox:
                    return new string[] { "input", "label" };
                case HTMLTestObjectType.RadioButton:
                    return new string[] { "input", "label" };
                case HTMLTestObjectType.ComboBox:
                    return new string[] { "select" };
                case HTMLTestObjectType.ListBox:
                    return new string[] { "select" };
                case HTMLTestObjectType.Table:
                    return new string[] { "table" };
                case HTMLTestObjectType.FileDialog:
                    return new string[] { "windows" };
                case HTMLTestObjectType.MsgBox:
                    return new string[] { "windows" };
                case HTMLTestObjectType.ActiveX:
                    return new string[] { "object" };
                case HTMLTestObjectType.Unknow:
                    return new string[] { };
                default:
                    return new string[] { };
            }
        }

        /*  private static HTMLTestObjectType GetObjectType(IHTMLElement element)
         *  Get the HTMLTestObjectType from element's tag name.
         */
        private static HTMLTestObjectType GetObjectType(IHTMLElement element)
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
            else if (tag == "SPAN" || tag == "LABEL")
            {
                return HTMLTestObjectType.Label;
            }
            //else if (tag == "TD" || tag == "DIV")
            //{
            //    string innerHTML;

            //    if (HTMLTestObject.TryGetValueByProperty(element, "innerHTML", out innerHTML))
            //    {
            //        if (innerHTML.IndexOf("<") >= 0 && innerHTML.IndexOf(">") > 0)
            //        {
            //            bool isLabel = true;

            //            MatchCollection mc = _tagReg.Matches(innerHTML);

            //            foreach (Match m in mc)
            //            {
            //                if (String.Compare(m.Value, "<SPAN", true) != 0
            //                    && String.Compare(m.Value, "<FONT", true) != 0
            //                    && String.Compare(m.Value, "<BR", true) != 0
            //                    && String.Compare(m.Value, "<P", true) != 0)
            //                {
            //                    isLabel = false;
            //                    break;
            //                }
            //            }

            //            if (isLabel)
            //            {
            //                return HTMLTestObjectType.Label;
            //            }

            //        }
            //        else
            //        {
            //            return HTMLTestObjectType.Label;
            //        }
            //    }
            //}
            else if (tag == "IMG")
            {
                string value;

                if (HTMLTestObject.TryGetValueByProperty(element, "onclick", out value))
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

                if (!HTMLTestObject.TryGetValueByProperty(element, "type", out inputType))
                {
                    return HTMLTestObjectType.Unknow;
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
                    return HTMLTestObjectType.RadioButton;
                }
            }
            else if (tag == "TEXTAERA")
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

                if (!HTMLTestObject.TryGetValueByProperty(element, "size", out selectValue))
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
        public HTMLTestGUIObject BuildObjectByType(IHTMLElement element)
        {
            if (element == null)
            {
                throw new CannotBuildObjectException("Element can not be null.");
            }

            HTMLTestObjectType type = GetObjectType(element);

            return BuildObjectByType(element, type);
        }

        public HTMLTestGUIObject BuildObjectByType(IHTMLElement element, HTMLTestObjectType type)
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
                case HTMLTestObjectType.RadioButton:
                    tmp = new HTMLTestRadioButton(element);
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
                tmp.Browser = _htmlTestBrowser;
                tmp.HtmlObjPool = this;
                return tmp;
            }
            else
            {
                throw new CannotBuildObjectException();
            }

        }

        /* HTMLGuiTestObject BuildObjectByType(IntPtr handle, HTMLTestObjectType type)
         * Build some special object, like MessageBox and FileDialog, they are Windows control.
         */
        public HTMLTestGUIObject BuildObjectByType(IntPtr handle, HTMLTestObjectType type)
        {
            if (handle == IntPtr.Zero || type == HTMLTestObjectType.Unknow)
            {
                throw new CannotBuildObjectException("Handle and type can not be null.");
            }

            HTMLTestGUIObject tmp;

            switch (type)
            {
                case HTMLTestObjectType.MsgBox:
                    tmp = new HTMLTestMsgBox(handle);
                    break;
                case HTMLTestObjectType.FileDialog:
                    tmp = new HTMLTestFileDialog();
                    break;
                default:
                    tmp = null;
                    break;
            }

            if (tmp != null)
            {
                tmp.Browser = _htmlTestBrowser;
                return tmp;
            }
            else
            {
                throw new CannotBuildObjectException();
            }
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
