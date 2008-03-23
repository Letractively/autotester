
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

        //the max time we need to wait, eg: we may wait for 30s to find a test object.
        private int _maxWaitSeconds = 30;

        //very time we sleep for 3 seconds, and find again.
        private const int _interval = 3;

        //buf to store the key for cache. 
        private static StringBuilder _keySB = new StringBuilder(128);

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
            throw new NotImplementedException();
        }

        public object GetObjectByType(string type, string values, int index)
        {
            if (!_isBrowser && this._testApp == null)
            {
                throw new TestAppNotFoundExpcetion("Can not find test app.");
            }

            if (_isBrowser && this._testBrowser == null)
            {
                throw new TestBrowserNotFoundException("Can not find test browser.");
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
                    //if (CheckObjectByType(_tempElement, typeValue, values, simPercent))
                    //{
                    //    leftIndex--;
                    //}

                    ////if index is 0 , that means we found the object.
                    //if (leftIndex < 0)
                    //{
                    //    _testObj = BuildObjectByType(_tempElement, typeValue);

                    //    ObjectCache.InsertObjectToCache(key, _testObj);

                    //    OnNewObjectFound("GetObjectByType", new string[] { type, values }, _testObj);

                    //    return _testObj;
                    //}


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
            throw new NotImplementedException();
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

        private static bool CheckObjectByType(IAccessible iAcc, int childID, MSAATestObjectType type, String value, int simPercent)
        {
            if (type == MSAATestObjectType.Unknow)
            {
                return false;
            }
            else if (type == MSAATestObjectType.Button)
            {
                return CheckButtonObject(iAcc, childID, value, simPercent);
            }

            return false;
        }

        private static bool CheckButtonObject(IAccessible iAcc, int childID, String value, int simPercent)
        {
            if (GetObjectType(iAcc, childID) != MSAATestObjectType.Button)
            {
                return false;
            }

            string name = MSAATestObject.GetName(iAcc, childID);

            if (String.IsNullOrEmpty(name))
            {
                return false;
            }
            else
            {
                return Searcher.IsStringLike(value, name, simPercent);
            }
        }


        /* IAccessible GetRootMSAAInterface()
         * Get MSAA interface for app or browser.
         * we will search the test object from this interface.
         */
        private IAccessible GetRootMSAAInterface()
        {
            try
            {
                if (!_isBrowser)
                {
                    Win32API.AccessibleObjectFromWindow(this._testApp.Handle, (uint)Win32API.IACC.OBJID_CLIENT, Win32API.IACCUID, ref this._rootMSAAInterface);
                }
                else
                {
                    Win32API.AccessibleObjectFromWindow(this._testBrowser.IEServerHandle, (uint)Win32API.IACC.OBJID_CLIENT, Win32API.IACCUID, ref this._rootMSAAInterface);
                }

                if (this._rootMSAAInterface != null)
                {
                    return _rootMSAAInterface;
                }

                throw new CannotAttachAppException("Can not get MSAA interface.");
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
                    if (role.IndexOf("push") >= 0 || role.IndexOf("button") >= 0)
                    {
                        return MSAATestObjectType.Button;
                    }
                    else if (role.IndexOf("editable") >= 0)
                    {
                        if (String.IsNullOrEmpty(action))
                        {
                            if (state.IndexOf("read only") >= 0)
                            {
                                return MSAATestObjectType.Label;
                            }
                        }
                        else
                        {
                            if (action.IndexOf("jump") >= 0)
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
