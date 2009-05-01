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
*          2008/02/01 wan,yu update, modify GetObjectByType use RotateSearch to improve performance, in some situation, will be 
*                                    10 times faster than before.
*          2008/02/13 wan,yu update, add event OnNewObjectFound. 
*          2008/02/18 wan,yu udpate, add event OnBeforeNewObjectFound.
*                                       
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestObjectPool : ITestObjectPool
    {

        #region fields

        private HTMLTestBrowser _htmlTestBrowser;

        //we use a hashtable as the cache, the key is generated from Method Name + _keySplitter+parameter.
        private bool _useCache = true;
        private const string _keySplitter = "__shrinerain__";

        //the default similar pencent to  find an object, if 100, that means we should make sure 100% match. 
        private bool _useFuzzySearch = false;
        private bool _autoAdjustSimilarPercent = true;
        private const int _defaultPercent = 100;
        private int _similarPercentUpBound = 100;
        private int _similarPercentLowBound = 70;
        private int _similarPercentStep = 10;
        private int _customSimilarPercent = _defaultPercent;

        //IHTMLElement is the interface for mshtml html object. We build actual test object on IHTMLElement.
        private IHTMLElement _tempElement;
        //IHTMLElementCollection is an array of IHTMLElement, some functions return the array of IHTMLElement.
        private IHTMLElement[] _allElements;
        private TestObject[] _allObjects;

        //current object used.
        private TestObject _testObj;
        private TestObject _cacheObj;

        //the max time we need to wait, eg: we may wait for 30s to find a test object.
        private int _maxWaitSeconds = 15;
        //very time we sleep for 3 seconds, and find again.
        private const int Interval = 3;

        //buf to store the key for cache. 
        private static StringBuilder _keySB = new StringBuilder(128);

        //regex to match tag
        private static Regex _htmlReg = new Regex("<[^>]+>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex _tagReg = new Regex("<[a-zA-Z]+ ", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex _scriptReg = new Regex("<script.*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static Dictionary<string, Regex> _regCache = new Dictionary<string, Regex>(17);

        //if this flag set to ture, we will ignore the table whose "border" < 1.
        private static bool _ignoreBorderlessTable = false;

        public event TestObjectEventHandler OnObjectFound;

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

        public bool UseFuzzySearch
        {
            get { return _useFuzzySearch; }
            set { _useFuzzySearch = value; }
        }

        //set the custom similary percent.
        public int SimilarPercent
        {
            get { return _customSimilarPercent; }
            set
            {
                _useFuzzySearch = true;
                _autoAdjustSimilarPercent = false;
                _customSimilarPercent = value;
                Searcher.DefaultPercent = value;
            }
        }

        public bool AutoAdjustSimilarPercent
        {
            get { return _autoAdjustSimilarPercent; }
            set
            {
                if (value)
                {
                    _useFuzzySearch = true;
                }
                _autoAdjustSimilarPercent = value;
            }
        }


        public bool IgnoreBorderlessTable
        {
            get { return _ignoreBorderlessTable; }
            set { _ignoreBorderlessTable = value; }
        }

        #endregion

        #region event

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
        public TestObject GetObjectByID(string id)
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
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

                times += Interval;
                Thread.Sleep(Interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object by id:" + id);
        }


        /*  Object GetObjectByName(string name)
         *  return the test object by .name property
         */
        public TestObject[] GetObjectsByName(string name)
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ObjectNotFoundException("Can not find object by name: name can not be empty.");
            }

            name = name.Trim();

            //we will try 30s to find a object
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    List<TestObject> result = new List<TestObject>();

                    IHTMLElement[] nameObjectsCol = _htmlTestBrowser.GetObjectsByName(name);
                    for (int i = 0; i < nameObjectsCol.Length; i++)
                    {
                        _tempElement = (IHTMLElement)nameObjectsCol[i];
                        if (!IsVisible(_tempElement))
                        {
                            continue;
                        }

                        _testObj = BuildObjectByType(_tempElement);
                        if (OnObjectFound != null)
                        {
                            OnObjectFound(_testObj, null);
                        }
                        result.Add(_testObj);
                    }

                    if (result.Count > 0)
                    {
                        return result.ToArray();
                    }
                }
                catch (CannotBuildObjectException)
                {
                    throw;
                }
                catch
                {
                }

                times += Interval;
                Thread.Sleep(Interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get objects by name:" + name);
        }

        /* Object GetObjectByIndex(int index)
         * return the test object by an integer index.
         */
        public TestObject GetObjectByIndex(int index)
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
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

                    _tempElement = _allElements[index];
                    if (IsVisible(_tempElement))
                    {
                        _testObj = BuildObjectByType(_tempElement);
                        if (OnObjectFound != null)
                        {
                            OnObjectFound(_testObj, null);
                        }
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

                times += Interval;
                Thread.Sleep(Interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object by index:" + index);

        }


        /* Object GetObjectByProperty(string property, string value)
         * return the test object by expect property.
         * eg: to find a image, we can find it by it's .src property, like .src="111.jpg"
         * we will use "Fuzzy Search" in this method
         */
        public TestObject[] GetObjectsByProperties(TestProperty[] properties)
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (properties == null || properties.Length == 0)
            {
                throw new ObjectNotFoundException("Properties can not be empty.");
            }
            //the similar percent to find an object.
            int simPercent = _defaultPercent;

            if (_useFuzzySearch)
            {
                if (_autoAdjustSimilarPercent)
                {
                    simPercent = _similarPercentUpBound;
                }
                else
                {
                    simPercent = _customSimilarPercent;
                }
            }

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                //get all HTML objects.
                GetAllElements();
                List<TestObject> result = new List<TestObject>();

                //if we have too many objects, we will try to find it's possible position to improve performance.
                int possibleStartIndex = Searcher.GetPossibleStartIndex(this._allElements.Length, _htmlReg, _htmlTestBrowser.GetHTML(), properties[0].Value.ToString());
                int[] searchOrder = Searcher.VibrationSearch(possibleStartIndex, 0, this._allElements.Length - 1);
                // check object one by one, start from the possible position.
                // the "|" means the start position, the "--->" means the search direction.            
                //  -----------------------------------------------------------------------
                //  step 1:                          |--->
                //  step 2:                      <---|
                //  step 3:                          |    --->
                //  ...                      <---    |
                foreach (int currentObjIndex in searchOrder)
                {
                    try
                    {
                        //get element by index.
                        _tempElement = (IHTMLElement)_allElements[currentObjIndex];

                        //if it is not an interactive object or the property is not found. 
                        if (IsVisible(_tempElement))
                        {
                            if (CheckObjectProperties(_tempElement, HTMLTestObjectType.Unknow, properties, out _testObj))
                            {
                                if (OnObjectFound != null)
                                {
                                    OnObjectFound(_testObj, null);
                                }
                                result.Add(_testObj);
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

                if (result.Count > 0)
                {
                    return result.ToArray();
                }

                times += Interval;
                Thread.Sleep(Interval * 1000);

                //while current simpercent is bigger than the low bound,we can still try lower similarity
                if (_useFuzzySearch && _autoAdjustSimilarPercent)
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

            StringBuilder propertiesInfo = new StringBuilder();
            foreach (TestProperty tp in properties)
            {
                propertiesInfo.Append("[" + tp.Name + ":" + tp.Value.ToString() + "],");
            }

            throw new ObjectNotFoundException("Can not find object by properties: " + propertiesInfo.ToString());
        }


        public TestObject[] GetObjectsByType(string type, TestProperty[] properties)
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (type == null || type.Trim() == "")
            {
                throw new ObjectNotFoundException("Can not get object by type: type can not be empty.");
            }
            else
            {
                type = type.Trim();
            }

            //convert the TYPE text to valid internal type.
            // eg: "button" to HTMLTestObjectType.Button
            HTMLTestObjectType typeValue = ConvertStrToHTMLType(type);

            if (typeValue == HTMLTestObjectType.Unknow)
            {
                throw new ObjectNotFoundException("Unknow HTML object type.");
            }

            //convert the type to HTML tags.
            //eg: convert Image to <img>, Button to <input type="button">,<input type="submit">...
            string[] tags = GetObjectTags(typeValue);

            if (tags == null)
            {
                throw new ObjectNotFoundException("Tags can not be empty.");
            }

            //if the expected value is number like, we think it is stand for the index of the object, not text on it.
            // eg: we can use type="textbox", values="1" to find the first textbox.

            //the similar percent to find an object.
            int simPercent = _defaultPercent;

            if (_useFuzzySearch)
            {
                if (_autoAdjustSimilarPercent)
                {
                    simPercent = _similarPercentUpBound;
                }
                else
                {
                    simPercent = _customSimilarPercent;
                }
            }

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                List<TestObject> result = new List<TestObject>();

                //because we may convert one type to multi tags, so check them one by one.
                //eg: Button to <input> and <button>
                foreach (string tag in tags)
                {
                    //find the object by tag.
                    IHTMLElement[] tagElementCol = _htmlTestBrowser.GetObjectsByTagName(tag);
                    if (tagElementCol != null && tagElementCol.Length > 0)
                    {
                        int possibleStartIndex = 0;
                        if (properties != null && properties.Length > 0)
                        {
                            Regex tagReg;
                            if (!_regCache.TryGetValue(tag, out tagReg))
                            {
                                //create new regex to match objects from HTML code.
                                tagReg = new Regex("<" + tag + "[^>]+>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                _regCache.Add(tag, tagReg);
                            }
                            //if we have too many objects, we will try to find it's possible position to improve performance.
                            possibleStartIndex = Searcher.GetPossibleStartIndex(tagElementCol.Length, tagReg, _htmlTestBrowser.GetHTML(), properties[0].Value.ToString());
                        }
                        int[] searchOrder = Searcher.VibrationSearch(possibleStartIndex, 0, tagElementCol.Length - 1);
                        // check object one by one, start from the possible position.
                        // the "|" means the start position, the "--->" means the search direction.            
                        //  -----------------------------------------------------------------------
                        //  step 1:                          |--->
                        //  step 2:                      <---|
                        //  step 3:                          |    --->
                        //  ...                      <---    |
                        foreach (int currentObjIndex in searchOrder)
                        {
                            try
                            {
                                _tempElement = tagElementCol[currentObjIndex];
                                // check if it is a interactive object.
                                if (IsVisible(_tempElement))
                                {
                                    //check object by type
                                    if (GetObjectType(_tempElement) == typeValue)
                                    {
                                        bool found = false;
                                        if (properties == null)
                                        {
                                            _testObj = BuildObjectByType(_tempElement, typeValue);
                                            found = true;
                                        }
                                        else if (CheckObjectProperties(_tempElement, typeValue, properties, out _testObj))
                                        {
                                            found = true;
                                        }

                                        if (found)
                                        {
                                            if (OnObjectFound != null)
                                            {
                                                OnObjectFound(_testObj, null);
                                            }
                                            result.Add(_testObj);
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
                    }
                }

                if (result.Count > 0)
                {
                    return result.ToArray();
                }

                //not found, sleep for 3 seconds, then try again.
                times += Interval;
                Thread.Sleep(Interval * 1000);

                //not found, we will try lower similarity
                if (_useFuzzySearch && _autoAdjustSimilarPercent)
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

            throw new ObjectNotFoundException("Can not find object by type [" + type + "]");
        }

        /* Object GetObjectByPoint(int x, int y)
         * return object from a expected point
         * x, y is the offset with browser, NOT screen.
         * 
         */
        public TestObject GetObjectByPoint(int x, int y)
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
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

                        if (OnObjectFound != null)
                        {
                            OnObjectFound(_testObj, null);
                        }

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

                times += Interval;
                Thread.Sleep(Interval * 1000);
            }

            throw new ObjectNotFoundException("Can not get object at point(" + x.ToString() + "" + y.ToString() + ")");
        }

        /* Object GetObjectByRect(int top, int left, int width, int height)
         * return object from a expected rect.
         */
        public TestObject GetObjectByRect(int left, int top, int width, int height, string typeStr, bool isPercent)
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            if (width < 1 || height < 1)
            {
                throw new ObjectNotFoundException("The width and height of rect can not be 0.");
            }

            HTMLTestObjectType type = ConvertStrToHTMLType(typeStr);

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

                        if (OnObjectFound != null)
                        {
                            OnObjectFound(_testObj, null);
                        }

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

        /* Object[] GetAllObjects()
         * return all object from the  browser.
         * we use _allObjects to store these object.
         */
        public TestObject[] GetAllObjects()
        {
            if (_htmlTestBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            //firstly, get all IHTMLElement from the browser
            GetAllElements();

            _allObjects = new TestObject[this._allElements.Length];

            //convert IHTMLELementCollection to an array.
            for (int i = 0; i < this._allElements.Length; i++)
            {
                _allObjects[i] = this.BuildObjectByType((IHTMLElement)this._allElements[i]);
            }

            return _allObjects;
        }

        /* Object GetLastObject()
         * return the last object we have got.
         */
        public TestObject GetLastObject()
        {
            return _testObj;
        }

        public void SetTimeout(int seconds)
        {
            if (seconds >= 0)
            {
                this._maxWaitSeconds = seconds;
            }
        }

        public int GetTimeout()
        {
            return this._maxWaitSeconds;
        }
        #endregion

        #endregion

        #region help methods

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
                throw new BrowserNotFoundException("Can not find HTML test browser for HTMLTestObjectPool.");
            }

            try
            {
                this._allElements = _htmlTestBrowser.GetAllHTMLElements();
            }
            catch
            {
            }
        }

        #region check test object

        private bool CheckObjectProperties(IHTMLElement element, HTMLTestObjectType type, TestProperty[] properties, out TestObject obj)
        {
            obj = null;
            if (properties == null)
            {
                return true;
            }
            else
            {
                int totalResult = 0;
                foreach (TestProperty tp in properties)
                {
                    //get property value
                    string propertyValue;
                    if (String.Compare(tp.Name, TestObject.VisibleProperty, true) == 0)
                    {
                        string visibleText = null;
                        try
                        {
                            obj = this.BuildObjectByType(element, type);
                            visibleText = ((HTMLTestGUIObject)obj).GetLabel().Trim();
                        }
                        catch
                        {
                            return false;
                        }

                        if (String.IsNullOrEmpty(visibleText) && HTMLTestObject.TryGetProperty(element, "innerText", out propertyValue))
                        {
                            visibleText = propertyValue;
                        }

                        if (!String.IsNullOrEmpty(visibleText))
                        {
                            return Searcher.IsStringLike(visibleText, tp.Value.ToString());
                        }
                        else
                        {
                            return HTMLTestGUIObject.GetAroundText(element).IndexOf(tp.Value.ToString()) >= 0;
                        }
                    }
                    else if (String.Compare(tp.Name, "tag", true) == 0)
                    {
                        return Searcher.IsStringLike(element.tagName, tp.Value.ToString());
                    }
                    else if (HTMLTestObject.TryGetProperty(element, tp.Name, out propertyValue))
                    {
                        //if equal, means we found it.
                        if (Searcher.IsStringLike(propertyValue, tp.Value.ToString()))
                        {
                            totalResult += tp.Weight;
                        }
                        else
                        {
                            totalResult -= tp.Weight;
                        }
                    }
                }

                return totalResult > 0;
            }
        }


        /* bool CheckObjectByType(IHTMLElement element, HTMLTestObjectType type, string value)
         * Check the object by expected type.
         * For different object, we need to check different property.
         * eg: for a listbox, we need to check it's first item.
         */
        private static bool CheckObjectByType(IHTMLElement element, HTMLTestObjectType type, string value, int simPercent)
        {
            if (element == null)
            {
                return false;
            }

            //check special types
            if (type == HTMLTestObjectType.ListBox || type == HTMLTestObjectType.ComboBox)
            {
                return CheckSelectObject(element, value, simPercent);
            }
            else if (type == HTMLTestObjectType.RadioBox)
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
            else if (type == HTMLTestObjectType.Link)
            {
                return CheckLinkObject(element, value, simPercent);
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
            else if (!HTMLTestObject.TryGetProperty(element, propertyName, out propertyValue))
            {
                return false;
            }

            //not special type, just need to check some property.
            return Searcher.IsStringLike(propertyValue, value, simPercent);
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
                    if (HTMLTestObject.TryGetProperty(element, "type", out type))
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

                if (value == null)
                {
                    return true;
                }

                //firstly, check value.
                if (IsPropertyLike(element, "innerText", value, simPercent))
                {
                    return true;
                }

                //then check title
                if (IsPropertyLike(element, "title", value, simPercent))
                {
                    return true;
                }

                string label = HTMLTestTextBox.GetLabelForTextBox(element);

                //for text around textbox, we may have ":", like "UserName:" , remove ":", make sure no writing mistake.
                if (value.Length > 1)
                {
                    if (value.EndsWith(":") || value.EndsWith("£º"))
                    {
                        value = value.Substring(0, value.Length - 1);
                    }

                    if (label.EndsWith(":") || label.EndsWith("£º"))
                    {
                        label = label.Substring(0, label.Length - 1);
                    }
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

                if (!HTMLTestObject.TryGetProperty(element, propertyName, out innerHTML))
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

                    if (!HTMLTestObject.TryGetProperty(element, "innerText", out innerText))
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
                if (!HTMLTestObject.TryGetProperty(element, propertyName, out items))
                {
                    return false;
                }

                if (value == null)
                {
                    return true;
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

                if (value == null)
                {
                    return true;
                }

                //fisrtly, check the .alt property.
                string propertyName = "alt";
                string actualValue;

                if (HTMLTestObject.TryGetProperty(element, propertyName, out actualValue))
                {
                    if (Searcher.IsStringLike(actualValue, value, simPercent))
                    {
                        return true;
                    }
                }

                //then check the .src property.
                propertyName = "src";
                string imgSrc;
                if (!HTMLTestObject.TryGetProperty(element, propertyName, out imgSrc))
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
                    if (HTMLTestObject.TryGetProperty(element, "type", out type))
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

                if (value == null)
                {
                    return true;
                }

                string labelText = HTMLTestRadioBox.GetLabelForRadioBox(element);
                if (!String.IsNullOrEmpty(labelText) && value.Length > 1)
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
                    if (HTMLTestObject.TryGetProperty(element, "type", out type))
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

                if (value == null)
                {
                    return true;
                }

                string labelText = HTMLTestCheckBox.GetLabelForCheckBox(element);

                if (!String.IsNullOrEmpty(labelText) && value.Length > 1)
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

                    if (HTMLTestObject.TryGetProperty(element, buttonTypeProperty, out buttonTypeValue))
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
                    if (!HTMLTestObject.TryGetProperty(element, buttonTypeProperty, out buttonTypeValue))
                    {
                        return false;
                    }

                    propertyName = "src";
                }

                if (value == null)
                {
                    return true;
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

                    if (!HTMLTestObject.TryGetProperty(element, "border", out border))
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

                if (value == null)
                {
                    return true;
                }

                //check caption of the table.
                string caption;

                if (HTMLTestObject.TryGetProperty(element, "caption", out caption))
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
                if (HTMLTestObject.TryGetProperty(cellElement, "innerText", out innerText))
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

        /* bool CheckLinkObject(IHTMLElement element, string value, int simPercent)
         * check link object, <a href...>
         */
        private static bool CheckLinkObject(IHTMLElement element, string value, int simPercent)
        {
            if (element.tagName != "A")
            {
                return false;
            }

            if (value == null && String.IsNullOrEmpty(element.innerText))
            {
                return true;
            }

            //check innerText property.
            string propertyName = "innerText";
            string propertyValue;

            if (!HTMLTestObject.TryGetProperty(element, propertyName, out propertyValue))
            {
                return false;
            }

            return Searcher.IsStringLike(propertyValue, value, simPercent);
        }

        #endregion

        /* bool IsPropertyEqual(IHTMLElement element, string propertyName, string value, int simPercent)
         * check if the property == value with the expected similar percent.
         */
        private static bool IsPropertyLike(IHTMLElement element, string propertyName, string value, int simPercent)
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
                return HTMLTestObjectType.RadioBox;
            }
            else if (type == "CHECKBOX" || type == "CHECK" || type == "CHK" || type == "CK")
            {
                return HTMLTestObjectType.CheckBox;
            }
            else if (type == "ACTIVEX")
            {
                return HTMLTestObjectType.ActiveX;
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
                    return new string[] { "label", "span", "font" };
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
                case HTMLTestObjectType.RadioBox:
                    return new string[] { "input", "label" };
                case HTMLTestObjectType.ComboBox:
                    return new string[] { "select" };
                case HTMLTestObjectType.ListBox:
                    return new string[] { "select" };
                case HTMLTestObjectType.Table:
                    return new string[] { "table" };
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
                    tmp.Browser = _htmlTestBrowser;
                    tmp.HTMLTestObjPool = this;
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

        /* void BeforeSearch(string method, string[] paras)
         * callback method when we want to search an object.
         */
        private void BeforeSearch(string method, string[] paras)
        {
            if (_htmlTestBrowser != null)
            {
                bool needWait = true;
                int times = 0;
                while (needWait && times <= _htmlTestBrowser.MaxWaitSeconds && _htmlTestBrowser.IsBusy)
                {
                    //browser is busy, sleep for 1 second.
                    System.Threading.Thread.Sleep(1 * 1000);
                    times += 1;
                }
            }
        }

        #endregion

        #endregion
    }
}
