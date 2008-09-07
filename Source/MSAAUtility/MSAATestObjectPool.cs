
/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MSAATestObjectPool.cs
*
* Description: This class provide methods to find a MSAA test object.
*
* History: 2008/03/19 wan,yu Init version.
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Helper;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public sealed class MSAATestObjectPool : ITestObjectPool
    {

        #region fields

        private struct MSAAElement
        {
            public IAccessible _iAcc;
            public int _childID;
        }

        private List<MSAAElement> _allElementsList;

        private TestApp _testApp;
        private TestBrowser _testBrowser;
        private bool _isBrowser = false;

        private IAccessible _rootMSAAInterface;
        private const int _selfID = 0;


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

        //current object used.
        private TestObject _testObj;
        private Object _cacheObj;

        private IAccessible _curIACC;
        private int _curChildID;

        //the max time we need to wait, eg: we may wait for 30s to find a test object.
        private int _maxWaitSeconds = 30;

        //very time we sleep for 3 seconds, and find again.
        private const int _interval = 3;

        //buf to store the key for cache. 
        private static StringBuilder _keySB = new StringBuilder(128);

        //delegate for check methods.
        private delegate bool CheckObjectDelegate(IAccessible iAcc, int childID, Object[] values, int simPercent);

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region ITestObjectPool Members

        public object GetObjectByIndex(int index)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByProperty(string property, string value)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByRegex(string property, string regex)
        {
            throw new NotImplementedException();
        }

        public object GetObjectBySimilarProperties(string[] properties, string[] values, int[] similarity, bool useAll)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByID(string id)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByName(string name)
        {
            if (!_isBrowser && this._testApp == null)
            {
                throw new AppNotFoundExpcetion("Can not find test app.");
            }

            if (_isBrowser && this._testBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find test browser.");
            }
            if (name == null || name.Trim().Length == 0)
            {
                throw new ObjectNotFoundException("Can not find object by name: name can not be empty.");
            }
            else
            {
                name = name.Trim();
            }

            //try to get object from cache.
            string key = GetKey(name);

            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
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
                try
                {
                    //get root msaa interface.
                    GetRootMSAAInterface();

                    //check object by type
                    if (CheckMSAAElement(ref _curIACC, ref _curChildID, new object[] { name }, simPercent, CheckObjectByName))
                    {
                        _testObj = BuildObjectByType(_curIACC, _curChildID, GetObjectType(_curIACC, _curChildID));

                        ObjectCache.InsertObjectToCache(key, _testObj);

                        //OnNewObjectFound("GetObjectByType", new string[] { type, values }, _testObj);

                        return _testObj;
                    }

                }
                catch (CannotBuildObjectException)
                {
                    throw;
                }
                catch
                {
                    continue;
                }
            }

            //not found, sleep for 3 seconds, then try again.
            times += _interval;
            Thread.Sleep(_interval * 1000);

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


            throw new ObjectNotFoundException("Can not find object by name: " + name);

        }

        public object GetObjectByType(string type, string values, int index)
        {
            if (!_isBrowser && this._testApp == null)
            {
                throw new AppNotFoundExpcetion("Can not find test app.");
            }

            if (_isBrowser && this._testBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find test browser.");
            }

            if (type == null || type.Trim() == "")
            {
                throw new ObjectNotFoundException("Can not get object by type: type can not be empty.");
            }
            else
            {
                type = type.Trim();
            }

            if (!String.IsNullOrEmpty(values))
            {
                values = values.Trim();
            }

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

            //convert the TYPE text to valid internal type.
            // eg: "button" to MSAATestObjectType.Button
            MSAATestObjectType typeValue = ConvertStrToMSAAType(type);

            if (typeValue == MSAATestObjectType.Unknow)
            {
                throw new ObjectNotFoundException("Unknow MSAA object type.");
            }


            //if we can find more than one test object, we need to consider about the index.
            int leftIndex = index;

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
                try
                {
                    //get root msaa interface.
                    GetRootMSAAInterface();

                    //check object by type
                    if (CheckObjectByType(out _curIACC, out _curChildID, typeValue, values, simPercent))
                    {
                        leftIndex--;
                    }

                    ////if index is 0 , that means we found the object.
                    if (leftIndex < 0)
                    {
                        _testObj = BuildObjectByType(_curIACC, _curChildID, typeValue);

                        ObjectCache.InsertObjectToCache(key, _testObj);

                        //OnNewObjectFound("GetObjectByType", new string[] { type, values }, _testObj);

                        return _testObj;
                    }


                }
                catch (CannotBuildObjectException)
                {
                    throw;
                }
                catch
                {
                    continue;
                }
            }

            //not found, sleep for 3 seconds, then try again.
            times += _interval;
            Thread.Sleep(_interval * 1000);

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


            throw new ObjectNotFoundException("Can not find object by type [" + type.ToString() + "] with value [" + values.ToString() + "]");

        }

        public object GetObjectByAI(string value)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByPoint(int x, int y)
        {
            if (!_isBrowser && this._testApp == null)
            {
                throw new AppNotFoundExpcetion("Can not find test app.");
            }

            if (_isBrowser && this._testBrowser == null)
            {
                throw new BrowserNotFoundException("Can not find test browser.");
            }

            //try to get object from cache.
            string key = GetKey(x + "," + y);

            if (ObjectCache.TryGetObjectFromCache(key, out _cacheObj))
            {
                return _testObj = (TestObject)_cacheObj;
            }

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    //get root msaa interface.
                    GetRootMSAAInterface();

                    Win32API.POINT p = new Win32API.POINT();
                    p.x = x;
                    p.y = y;

                    Object childID;

                    Win32API.AccessibleObjectFromPoint(p, out _curIACC, out childID);

                    if (_curIACC != null)
                    {
                        _curChildID = Convert.ToInt32(childID);

                        _testObj = BuildObjectByType(_curIACC, _curChildID, GetObjectType(_curIACC, _curChildID));

                        ObjectCache.InsertObjectToCache(key, _testObj);

                        //OnNewObjectFound("GetObjectByType", new string[] { type, values }, _testObj);

                        return _testObj;
                    }

                }
                catch (CannotBuildObjectException)
                {
                    throw;
                }
                catch
                {
                    continue;
                }

                //not found, sleep for 3 seconds, then try again.
                times += _interval;
                Thread.Sleep(_interval * 1000);
            }

            throw new ObjectNotFoundException("Can not find object at Point:[" + x + "," + y + "]");
        }

        public object GetObjectByRect(int top, int left, int width, int height, string typeStr, bool isPercent)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByColor(string color)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByCustomer(object value)
        {
            throw new NotImplementedException();
        }

        public object[] GetAllObjects()
        {
            throw new NotImplementedException();
        }

        public object GetLastObject()
        {
            throw new NotImplementedException();
        }

        public void SetTestBrowser(ITestBrowser testBrowser)
        {
            this._testBrowser = (TestBrowser)testBrowser;
            _isBrowser = true;
        }

        public void SetTestApp(ITestApp testApp)
        {
            this._testApp = (TestApp)testApp;
            _isBrowser = false;
        }

        #endregion

        #endregion

        #region private methods

        private bool CheckObjectByName(IAccessible iAcc, int childID, Object[] name, int simPercent)
        {

            if (iAcc != null && childID >= 0 && name != null)
            {
                string expectedName = name[0].ToString();

                string curName = MSAATestObject.GetName(iAcc, childID);

                return Searcher.IsStringLike(curName, expectedName, simPercent);
            }

            return false;
        }

        private bool CheckObjectByType(out IAccessible iAcc, out int childID, MSAATestObjectType type, String value, int simPercent)
        {
            iAcc = null;
            childID = 0;

            if (type == MSAATestObjectType.Unknow)
            {
                return false;
            }
            else if (type == MSAATestObjectType.Button)
            {
                return CheckMSAAElement(ref iAcc, ref childID, new object[] { value }, simPercent, CheckButtonObject);
            }

            return false;
        }

        private bool CheckButtonObject(IAccessible iAcc, int childID, Object[] value, int simPercent)
        {
            if (iAcc != null && childID >= 0)
            {
                if (GetObjectType(iAcc, childID) == MSAATestObjectType.Button)
                {
                    if (value == null || value[0] == null)
                    {
                        return true;
                    }
                    else
                    {
                        string expectedVal = value[0].ToString();

                        string name = MSAATestObject.GetName(iAcc, childID);

                        if (!String.IsNullOrEmpty(name))
                        {
                            if (Searcher.IsStringLike(expectedVal, name, simPercent))
                            {
                                return true;
                            }
                        }
                    }

                }
            }

            return false;
        }

        /* bool CheckMSAAElement(ref IAccessible iAcc, ref int childID, string value, int simPercent, CheckObjectDelegate checkObjDelegate)
         * Go through each MSAA element, use the delegate to check it.
         */
        private bool CheckMSAAElement(ref IAccessible iAcc, ref int childID, object[] values, int simPercent, CheckObjectDelegate checkObjDelegate)
        {
            //if the iAcc is null, means it is the first time to check.
            if (iAcc == null)
            {
                iAcc = GetRootMSAAInterface();
                childID = 0;
            }

            if (iAcc != null)
            {
                //check state, ensure the object is visible.
                string state = MSAATestObject.GetState(iAcc, childID);
                if (state.IndexOf("invisible", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return false;
                }

                //call delegate to perform check.
                if (checkObjDelegate(iAcc, childID, values, simPercent))
                {
                    return true;
                }

                IAccessible parentIAcc = iAcc;

                //check each child element.
                int childCount = childID > 0 ? 0 : iAcc.accChildCount;
                if (childCount > 0)
                {
                    for (int i = 0; i < childCount; i++)
                    {
                        try
                        {
                            IAccessible childIAcc = MSAATestObject.GetChildIAcc(parentIAcc, i);

                            if (childIAcc != null)
                            {
                                iAcc = childIAcc;
                                childID = 0;
                            }
                            else
                            {
                                iAcc = parentIAcc;
                                childID = i + 1;
                            }

                            if (CheckMSAAElement(ref iAcc, ref childID, values, simPercent, checkObjDelegate))
                            {
                                return true;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return false;
        }

        private static MSAATestGUIObject BuildObjectByType(IAccessible iAcc, int childID, MSAATestObjectType type)
        {
            if (type == MSAATestObjectType.Button)
            {
                return new MSAATestButton(iAcc, childID);
            }
            else if (type == MSAATestObjectType.TextBox)
            {
                return new MSAATestTextBox(iAcc, childID);
            }

            return new MSAATestGUIObject(iAcc, childID);
        }

        /* List<MSAAElement> GetAllMSAAElements()
         * Get all MSAA elements in the application under test.
         */
        private List<MSAAElement> GetAllMSAAElements()
        {
            if (_rootMSAAInterface == null)
            {
                _rootMSAAInterface = GetRootMSAAInterface();
            }

            try
            {
                _allElementsList = GetChildrenMSAAElements(_rootMSAAInterface, 0);

                return _allElementsList;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (_isBrowser)
                {
                    throw new CannotAttachBrowserException("Can not get MSAA elements: " + ex.Message);
                }
                else
                {
                    throw new CannotAttachAppException("Can not get MSAA elements: " + ex.Message);
                }
            }
        }

        /* List<MSAAElement> GetChildrenMSAAElements(IAccessible iACC)
         * get all the children of the expected IAccessible.
         */
        private List<MSAAElement> GetChildrenMSAAElements(IAccessible iACC, int childID)
        {
            if (iACC != null)
            {

                List<MSAAElement> elementsList = new List<MSAAElement>();

                MSAAElement curElement;

                string state = MSAATestObject.GetState(iACC, childID);
                if (state.IndexOf("invisible", StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    //add self.
                    curElement = new MSAAElement();
                    curElement._iAcc = iACC;
                    curElement._childID = childID;
                    elementsList.Add(curElement);
                }

                int childCount = childID > 0 ? 0 : iACC.accChildCount;

                if (childCount > 0)
                {
                    for (int i = 1; i <= childCount; i++)
                    {
                        List<MSAAElement> childrenList = null;

                        object tmpChildID = (object)i;

                        try
                        {
                            IAccessible childIACC = (IAccessible)iACC.get_accChild(tmpChildID);

                            state = MSAATestObject.GetState(childIACC, 0);

                            //child also support IACC, check each child.
                            if (state.IndexOf("invisible", StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                childrenList = GetChildrenMSAAElements(childIACC, 0);
                            }
                        }
                        catch
                        {
                            state = MSAATestObject.GetState(iACC, i);

                            if (state.IndexOf("invisible", StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                childrenList = GetChildrenMSAAElements(iACC, i);
                            }
                        }
                        finally
                        {
                            if (childrenList != null)
                            {
                                foreach (MSAAElement e in childrenList)
                                {
                                    elementsList.Add(e);
                                }
                            }
                        }
                    }
                }

                return elementsList;
            }

            return null;
        }

        /* IAccessible GetRootMSAAInterface()
         * Get MSAA interface for app or browser.
         * we will search the test object from this interface.
         */
        private IAccessible GetRootMSAAInterface()
        {
            try
            {
                if (this._rootMSAAInterface == null)
                {
                    if (!_isBrowser)
                    {
                        Win32API.AccessibleObjectFromWindow(this._testApp.Handle, (int)Win32API.IACC.OBJID_CLIENT, ref Win32API.IACCUID, ref this._rootMSAAInterface);
                    }
                    else
                    {
                        Win32API.AccessibleObjectFromWindow(this._testBrowser.IEServerHandle, (int)Win32API.IACC.OBJID_CLIENT, ref Win32API.IACCUID, ref this._rootMSAAInterface);
                    }
                }

                if (this._rootMSAAInterface == null)
                {
                    throw new CannotAttachAppException("Can not get MSAA interface.");
                }

                return _rootMSAAInterface;

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotAttachAppException("Can not get MSAA interface: " + ex.Message);
            }
        }

        /* MSAATestObjectType GetObjectType(IAccessible iAcc) 
         * return the acc object type.
         */
        private static MSAATestObjectType GetObjectType(IAccessible iAcc)
        {
            return GetObjectType(iAcc, _selfID);
        }

        private static MSAATestObjectType GetObjectType(IAccessible iAcc, int childID)
        {
            if (iAcc != null)
            {
                string role = MSAATestObject.GetRole(iAcc, childID);
                string action = MSAATestObject.GetDefAction(iAcc, childID);
                string state = MSAATestObject.GetState(iAcc, childID);

                if (!String.IsNullOrEmpty(role))
                {
                    role = role.ToLower();

                    if (role.IndexOf("push") >= 0 || role.IndexOf("button") >= 0)
                    {
                        return MSAATestObjectType.Button;
                    }
                    else if (role.IndexOf("editable") >= 0)
                    {
                        if (String.IsNullOrEmpty(action))
                        {
                            if (state.IndexOf("read only", StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                return MSAATestObjectType.Label;
                            }
                        }
                        else
                        {
                            if (action.IndexOf("Jump", StringComparison.CurrentCultureIgnoreCase) >= 0)
                            {
                                return MSAATestObjectType.Link;
                            }
                        }

                        return MSAATestObjectType.TextBox;
                    }
                    else if (role.IndexOf("combo") >= 0)
                    {
                        return MSAATestObjectType.ComboBox;
                    }
                    else if (role.IndexOf("radio") >= 0)
                    {
                        return MSAATestObjectType.RadioButton;
                    }
                    else if (role.IndexOf("check") >= 0)
                    {
                        return MSAATestObjectType.CheckBox;
                    }
                    else if (role.IndexOf("text") >= 0)
                    {
                        return MSAATestObjectType.Label;
                    }
                    else if (role.IndexOf("table") >= 0)
                    {
                        return MSAATestObjectType.Table;
                    }
                    else if (role.IndexOf("graphic") >= 0)
                    {
                        return MSAATestObjectType.Image;
                    }
                }

                if (!String.IsNullOrEmpty(action))
                {
                    if (action.IndexOf("Press", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return MSAATestObjectType.Button;
                    }
                    else if (action.IndexOf("Collapse", StringComparison.CurrentCultureIgnoreCase) >= 0 || action.IndexOf("Expand", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return MSAATestObjectType.Tree;
                    }
                    else if (action.IndexOf("Jump", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return MSAATestObjectType.Link;
                    }
                }
            }

            return MSAATestObjectType.Unknow;

        }

        private static MSAATestObjectType ConvertStrToMSAAType(string type)
        {
            if (String.IsNullOrEmpty(type))
            {
                return MSAATestObjectType.Unknow;
            }

            type = type.ToUpper().Replace(" ", "");

            if (type == "BUTTON" || type == "BTN" || type == "B")
            {
                return MSAATestObjectType.Button;
            }
            else if (type == "LABEL" || type == "LB")
            {
                return MSAATestObjectType.Label;
            }
            else if (type == "TEXTBOX" || type == "TEXT" || type == "INPUTBOX" || type == "TXT" || type == "T")
            {
                return MSAATestObjectType.TextBox;
            }
            else if (type == "LINK" || type == "HYPERLINK" || type == "LK" || type == "A")
            {
                return MSAATestObjectType.Link;
            }
            else if (type == "IMAGE" || type == "IMG" || type == "PICTURE" || type == "PIC" || type == "I" || type == "P")
            {
                return MSAATestObjectType.Image;
            }
            else if (type == "COMBOBOX" || type == "DROPDOWNBOX" || type == "DROPDOWNLIST" || type == "DROPDOWN" || type == "CB")
            {
                return MSAATestObjectType.ComboBox;
            }
            else if (type == "LISTBOX" || type == "LIST" || type == "LST" || type == "LS")
            {
                return MSAATestObjectType.ListBox;
            }
            else if (type == "RADIOBOX" || type == "RADIOBUTTON" || type == "RADIO" || type == "RAD" || type == "R")
            {
                return MSAATestObjectType.RadioButton;
            }
            else if (type == "CHECKBOX" || type == "CHECK" || type == "CHK" || type == "CK")
            {
                return MSAATestObjectType.CheckBox;
            }
            else if (type == "FILEDIAGLOG" || type == "FILE" || type == "FOLDER" || type == "FOLDERDIALOG" || type == "F")
            {
                return MSAATestObjectType.FileDialog;
            }
            else if (type == "MSGBOX" || type == "MSG" || type == "MESSAGE" || type == "MESSAGEBOX" || type == "POPWINDOW" || type == "POPBOX" || type == "M")
            {
                return MSAATestObjectType.MsgBox;
            }
            else if (type == "TABLE" || type == "TBL" || type == "T")
            {
                return MSAATestObjectType.Table;
            }
            else
            {
                return MSAATestObjectType.Unknow;
            }

        }

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

                _keySB.Append(this._testBrowser.GetCurrentUrl());
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

    }
}
