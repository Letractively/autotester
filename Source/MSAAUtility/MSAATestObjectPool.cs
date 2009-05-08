
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
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public sealed class MSAATestObjectPool : ITestObjectPool
    {
        #region fields

        private MSAATestApp _testApp;

        IAccessible _curIACC;
        int _curChildID;

        //current object used.
        private TestObject _testObj;

        //the max time we need to wait, eg: we may wait for 30s to find a test object.
        private int _maxWaitSeconds = 15;

        //very time we sleep for 3 seconds, and find again.
        private const int Interval = 3;

        //buf to store the key for cache. 
        private static StringBuilder _keySB = new StringBuilder(128);

        //delegate for check methods.
        private delegate bool CheckObjectDelegate(IAccessible iAcc, int childID, TestProperty[] properties);

        public event TestObjectEventHandler OnObjectFound;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region ITestObjectPool Members


        public TestObject GetObjectByIndex(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TestObject[] GetObjectsByProperties(TestProperty[] properties)
        {
            if (this._testApp == null)
            {
                throw new AppNotFoundExpcetion("Can not find test app.");
            }

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    //check object by type
                    List<MSAATestObject> res = CheckAllElements(new MSAATestObject.RoleType[] { MSAATestObject.RoleType.None }, properties, CheckElementProperties, false);
                    if (res != null)
                    {
                        return res.ToArray();
                    }
                    else
                    {
                        //not found, sleep for 3 seconds, then try again.
                        times += Interval;
                        Thread.Sleep(Interval * 1000);
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
            times += Interval;
            Thread.Sleep(Interval * 1000);

            throw new ObjectNotFoundException("Can not find object by properties.");
        }

        public TestObject GetObjectByID(string id)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TestObject GetObjectByRect(int top, int left, int width, int height, string type, bool isPercent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TestObject[] GetAllObjects()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TestObject GetLastObject()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TestObject[] GetObjectsByName(string name)
        {
            if (this._testApp == null)
            {
                throw new AppNotFoundExpcetion("Can not find test app.");
            }

            if (name == null || name.Trim().Length == 0)
            {
                throw new ObjectNotFoundException("Can not find object by name: name can not be empty.");
            }
            else
            {
                name = name.Trim();
            }

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    //check object by name
                    List<MSAATestObject> res = CheckAllElements(new MSAATestObject.RoleType[] { MSAATestObject.RoleType.None }, new TestProperty[] { new TestProperty(name) }, CheckObjectByName, true);
                    if (res != null)
                    {
                        return res.ToArray();
                    }
                    else
                    {
                        //not found, sleep for 3 seconds, then try again.
                        times += Interval;
                        Thread.Sleep(Interval * 1000);
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

            throw new ObjectNotFoundException("Can not find object by name: " + name);

        }

        public TestObject[] GetObjectsByType(string type, TestProperty[] properties)
        {
            if (this._testApp == null)
            {
                throw new AppNotFoundExpcetion("Can not find test app.");
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
            // eg: "button" to MSAATestObject.Type.Button
            MSAATestObject.RoleType[] typeValue = MSAATestObjectFactory.GetMSAATypeByString(type);

            if (typeValue[0] == MSAATestObject.RoleType.None)
            {
                throw new ObjectNotFoundException("Unknow MSAA object type.");
            }

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    //check object by type
                    List<MSAATestObject> res = CheckAllElements(typeValue, properties, CheckObjectByType, false);
                    if (res != null)
                    {
                        return res.ToArray();
                    }
                    else
                    {
                        //not found, sleep for 3 seconds, then try again.
                        times += Interval;
                        Thread.Sleep(Interval * 1000);
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
            times += Interval;
            Thread.Sleep(Interval * 1000);

            throw new ObjectNotFoundException("Can not find object by type [" + type.ToString() + "]");

        }

        public TestObject GetObjectByPoint(int x, int y)
        {
            if (this._testApp == null)
            {
                throw new AppNotFoundExpcetion("Can not find test app.");
            }

            //we will try 30s to find an object.
            int times = 0;
            while (times <= _maxWaitSeconds)
            {
                try
                {
                    Win32API.POINT p = new Win32API.POINT();
                    p.x = x;
                    p.y = y;

                    Object childID;
                    Win32API.AccessibleObjectFromPoint(p, out _curIACC, out childID);

                    if (_curIACC != null)
                    {
                        _curChildID = Convert.ToInt32(childID);

                        _testObj = MSAATestObjectFactory.BuildObject(_curIACC, _curChildID);

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
                times += Interval;
                Thread.Sleep(Interval * 1000);
            }

            throw new ObjectNotFoundException("Can not find object at Point:[" + x + "," + y + "]");
        }

        public void SetTestApp(ITestApp testApp)
        {
            if (testApp == null)
            {
                throw new AppNotFoundExpcetion("Application is not exist.");
            }
            else
            {
                this._testApp = (MSAATestApp)testApp;
            }
        }

        public void SetTestBrowser(ITestBrowser browser)
        {
            throw new NotImplementedException();
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

        #region private methods

        private static bool CheckObjectByName(IAccessible iAcc, int childID, TestProperty[] name)
        {
            string expectedName = name[0].Value.ToString();
            string curName = MSAATestObject.GetName(iAcc, childID);

            return Searcher.IsStringLike(curName, expectedName);
        }

        private static bool CheckObjectByType(IAccessible iAcc, int childID, TestProperty[] values)
        {
            MSAATestObject.RoleType type = (MSAATestObject.RoleType)values[0].Value;
            if (type == MSAATestObject.RoleType.None)
            {
                return false;
            }
            else if (type == MSAATestObject.RoleType.PushButton || type == MSAATestObject.RoleType.SplitButton ||
                type == MSAATestObject.RoleType.SpinButton)
            {
                return CheckButtonObject(iAcc, childID, values);
            }
            else if (type == MSAATestObject.RoleType.CheckButton)
            {
                return CheckButtonObject(iAcc, childID, values);
            }
            else if (type == MSAATestObject.RoleType.RadioButton)
            {
                return CheckButtonObject(iAcc, childID, values);
            }
            else if (type == MSAATestObject.RoleType.Text)
            {
                return CheckTextboxObject(iAcc, childID, values);
            }

            return false;
        }

        private static bool CheckTextboxObject(IAccessible iAcc, int childID, TestProperty[] value)
        {
            if (value == null || value[0] == null)
            {
                return true;
            }
            else
            {
                string expectedVal = value[1].Value.ToString();
                string curValue = MSAATestObject.GetValue(iAcc, childID);

                return Searcher.IsStringLike(expectedVal, curValue);
            }
        }
        private static bool CheckButtonObject(IAccessible iAcc, int childID, TestProperty[] value)
        {
            if (value == null || value[0] == null)
            {
                return true;
            }
            else
            {
                string expectedVal = value[1].Value.ToString();
                string name = MSAATestObject.GetName(iAcc, childID);

                if (!String.IsNullOrEmpty(name))
                {
                    return Searcher.IsStringLike(expectedVal, name);
                }
            }

            return false;
        }

        private static bool CheckElementProperties(IAccessible iAcc, int childID, TestProperty[] properties)
        {
            int totalResult = 0;
            foreach (TestProperty tp in properties)
            {
                //get property value
                string propertyValue = MSAATestObject.GetProperty(iAcc, childID, tp.Name).ToString();
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

            return totalResult > 0;
        }

        private List<MSAATestObject> CheckAllElements(MSAATestObject.RoleType[] objTypes, TestProperty[] properties, CheckObjectDelegate checkObjDelegate, bool onlyOne)
        {

            List<MSAATestObject> resultList = new List<MSAATestObject>();

            Dictionary<MSAATestObject.RoleType, TestProperty[]> propertiesTable = new Dictionary<MSAATestObject.RoleType, TestProperty[]>(13);

            if (objTypes == null || objTypes.Length == 0)
            {
                objTypes = new MSAATestObject.RoleType[] { MSAATestObject.RoleType.None };
            }

            Queue<IAccessible> que = new Queue<IAccessible>();
            MSAATestObject rootObj = this._testApp.RootObject;
            System.Drawing.Rectangle rootRect = rootObj.GetRect();
            que.Enqueue(rootObj.IAcc);

            //BFS
            while (que.Count > 0)
            {
                IAccessible curObj = que.Dequeue();
                //check if object is valid.
                if (MSAATestObject.IsValidObject(curObj, 0) && rootRect.Contains(MSAATestObject.GetRect(curObj, 0)))
                {
                    foreach (MSAATestObject.RoleType objType in objTypes)
                    {
                        if (properties != null && properties.Length > 0)
                        {
                            if (!propertiesTable.TryGetValue(objType, out properties))
                            {
                                List<TestProperty> newProperties = new List<TestProperty>();
                                TestProperty typeProperty = new TestProperty("RoleType", objType, false, 100);
                                newProperties.Add(typeProperty);
                                foreach (TestProperty tp in properties)
                                {
                                    newProperties.Add(tp);
                                }
                                propertiesTable.Add(objType, newProperties.ToArray());
                            }
                        }

                        if (objType == MSAATestObject.RoleType.None || MSAATestObject.GetRole(curObj, 0) == objType)
                        {
                            if (properties == null || checkObjDelegate(curObj, 0, properties))
                            {
                                MSAATestObject resObj = MSAATestObjectFactory.BuildObject(curObj, 0);
                                resultList.Add(resObj);

                                if (onlyOne)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                if (MSAATestObject.GetRole(curObj, 0) == MSAATestObject.RoleType.ComboBox)
                {
                    continue;
                }

                IAccessible[] childrenObj = MSAATestObject.GetChildrenIAcc(curObj);
                if (childrenObj != null)
                {
                    for (int i = 0; i < childrenObj.Length; i++)
                    {
                        try
                        {
                            que.Enqueue(childrenObj[i]);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return resultList;
        }

        #endregion

        #endregion

    }
}
