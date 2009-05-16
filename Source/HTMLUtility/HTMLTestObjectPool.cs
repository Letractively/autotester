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

        //the max time we need to wait, eg: we may wait for 30s to find a test object.
        private int _maxWaitSeconds = 15;
        //very time we sleep for 2 seconds, and find again.
        private const int Interval = 2;

        //regex to match tag
        private static Regex _htmlReg = new Regex("< *[a-z]+[^>]+>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
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
                        _testObj = HTMLTestObjectFactory.BuildHTMLTestObject(_tempElement, this._htmlTestBrowser, this);
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
                        if (!HTMLTestObjectFactory.IsVisible(_tempElement))
                        {
                            continue;
                        }

                        _testObj = HTMLTestObjectFactory.BuildHTMLTestObject(_tempElement, this._htmlTestBrowser, this);
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

            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    GetAllElements();

                    _tempElement = _allElements[index];
                    if (HTMLTestObjectFactory.IsVisible(_tempElement))
                    {
                        _testObj = HTMLTestObjectFactory.BuildHTMLTestObject(_tempElement, this._htmlTestBrowser, this);
                        if (OnObjectFound != null)
                        {
                            OnObjectFound(_testObj, null);
                        }
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
                IHTMLElement[] candidateElements = null;
                string id;
                if (TestProperty.TryGetIDValue(properties, out id))
                {
                    IHTMLElement element = _htmlTestBrowser.GetObjectByID(id);
                    if (element != null)
                    {
                        candidateElements = new IHTMLElement[] { element };
                    }
                }
                else
                {
                    //get all HTML objects.
                    GetAllElements();
                    candidateElements = _allElements;
                }

                if (candidateElements != null || candidateElements.Length > 0)
                {
                    List<TestObject> result = new List<TestObject>();

                    bool isOnlyOneObject = false;
                    //if we have too many objects, we will try to find it's possible position to improve performance.
                    int possibleStartIndex = 0;
                    if (Searcher.IsNeedCalPossibleStartIndex(candidateElements.Length))
                    {
                        string searchVal = System.Web.HttpUtility.HtmlEncode(properties[0].Value.ToString());
                        string htmlContent = _htmlTestBrowser.GetHTMLContent();
                        int startPos = htmlContent.IndexOf(searchVal);
                        if (startPos > 0)
                        {
                            //if we have too many objects, we will try to find it's possible position to improve performance.             
                            possibleStartIndex = Searcher.GetPossibleStartIndex(candidateElements.Length, _htmlReg, htmlContent, searchVal);
                            if (startPos == htmlContent.LastIndexOf(searchVal))
                            {
                                isOnlyOneObject = true;
                            }
                        }
                    }
                    int[] searchOrder = Searcher.VibrationSearch(possibleStartIndex, 0, candidateElements.Length - 1);
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
                            _tempElement = (IHTMLElement)candidateElements[currentObjIndex];

                            //if it is not an interactive object or the property is not found. 
                            if (HTMLTestObjectFactory.IsVisible(_tempElement))
                            {
                                if (CheckObjectProperties(_tempElement, HTMLTestObjectType.Unknow, properties, out _testObj))
                                {
                                    if (OnObjectFound != null)
                                    {
                                        OnObjectFound(_testObj, null);
                                    }
                                    result.Add(_testObj);
                                    if (isOnlyOneObject)
                                    {
                                        break;
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

                    if (result.Count > 0)
                    {
                        return result.ToArray();
                    }
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
            HTMLTestObjectType typeValue = HTMLTestObjectFactory.ConvertStrToHTMLType(type);
            if (typeValue == HTMLTestObjectType.Unknow)
            {
                throw new ObjectNotFoundException("Unknow HTML object type.");
            }

            //convert the type to HTML tags.
            //eg: convert Image to <img>, Button to <input type="button">,<input type="submit">...
            string[] tags = HTMLTestObjectFactory.GetObjectTags(typeValue);
            if (tags == null)
            {
                throw new ObjectNotFoundException("Tags can not be empty.");
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
                List<TestObject> result = new List<TestObject>();

                IHTMLElement[] candidateElements = null;
                string id;
                if (TestProperty.TryGetIDValue(properties, out id))
                {
                    IHTMLElement element = _htmlTestBrowser.GetObjectByID(id);
                    if (element != null)
                    {
                        candidateElements = new IHTMLElement[] { element };
                    }
                }

                bool isOnlyOneObject = false;
                //because we may convert one type to multi tags, so check them one by one.
                //eg: Button to <input> and <button>
                foreach (string tag in tags)
                {
                    if (candidateElements == null)
                    {
                        candidateElements = _htmlTestBrowser.GetObjectsByTagName(tag);
                    }

                    if (candidateElements != null && candidateElements.Length > 0)
                    {
                        int possibleStartIndex = 0;
                        if (properties != null && properties.Length > 0 && Searcher.IsNeedCalPossibleStartIndex(candidateElements.Length))
                        {
                            Regex tagReg;
                            if (!_regCache.TryGetValue(tag, out tagReg))
                            {
                                //create new regex to match objects from HTML code.
                                tagReg = new Regex("<" + tag + "[^>]+>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                _regCache.Add(tag, tagReg);
                            }

                            string searchVal = System.Web.HttpUtility.HtmlEncode(properties[0].Value.ToString());
                            string htmlContent = _htmlTestBrowser.GetHTMLContent();

                            int startPos = htmlContent.IndexOf(searchVal);
                            if (startPos > 0)
                            {
                                //if we have too many objects, we will try to find it's possible position to improve performance.             
                                possibleStartIndex = Searcher.GetPossibleStartIndex(candidateElements.Length, tagReg, htmlContent, searchVal);
                                if (startPos == htmlContent.LastIndexOf(searchVal))
                                {
                                    isOnlyOneObject = true;
                                }
                            }
                        }
                        int[] searchOrder = Searcher.VibrationSearch(possibleStartIndex, 0, candidateElements.Length - 1);
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
                                _tempElement = candidateElements[currentObjIndex];
                                // check if it is a interactive object.
                                if (HTMLTestObjectFactory.IsVisible(_tempElement))
                                {
                                    //check object by type
                                    if (HTMLTestObjectFactory.GetObjectType(_tempElement) == typeValue)
                                    {
                                        bool found = false;
                                        if (properties == null)
                                        {
                                            _testObj = HTMLTestObjectFactory.BuildHTMLTestObjectByType(_tempElement, typeValue, this._htmlTestBrowser, this);
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

                                            if (isOnlyOneObject)
                                            {
                                                break;
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
                    }

                    if (result.Count > 0 && isOnlyOneObject)
                    {
                        break;
                    }

                    candidateElements = null;
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

            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    _tempElement = this._htmlTestBrowser.GetObjectFromPoint(x, y);

                    if (HTMLTestObjectFactory.IsVisible(_tempElement))
                    {
                        _testObj = HTMLTestObjectFactory.BuildHTMLTestObject(_tempElement, this._htmlTestBrowser, this);

                        if (OnObjectFound != null)
                        {
                            OnObjectFound(_testObj, null);
                        }

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

            HTMLTestObjectType type = HTMLTestObjectFactory.ConvertStrToHTMLType(typeStr);

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
                _allObjects[i] = HTMLTestObjectFactory.BuildHTMLTestObject((IHTMLElement)this._allElements[i], this._htmlTestBrowser, this);
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
                            obj = HTMLTestObjectFactory.BuildHTMLTestObjectByType(element, type, this._htmlTestBrowser, this);
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
                            return tp.IsValueMatch(visibleText);
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
                        if (tp.IsValueMatch(propertyValue))
                        {
                            totalResult += tp.Weight;
                        }
                        else
                        {
                            totalResult -= tp.Weight;
                        }
                    }
                }

                if (totalResult > 0)
                {
                    obj = HTMLTestObjectFactory.BuildHTMLTestObjectByType(element, type, this._htmlTestBrowser, this);
                    return true;
                }

                return false;
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
