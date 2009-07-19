/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestCheckPoint.cs
*
* Description: This class provide functions for check point.
*
* History: 2008/01/23 wan,yu Init version.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Core
{
    public class TestCheckPoint : ITestCheckPoint
    {

        #region fields

        protected ITestObjectPool _objPool;

        #endregion

        #region event

        public delegate void BeforeCheckHandler(String methodName, object[] paras, CheckType type);
        public event BeforeCheckHandler OnBeforeCheck;

        public delegate void AfterCheckHandler(bool checkResult, object actualValue, String methodName, object[] paras, CheckType type);
        public event AfterCheckHandler OnAfterCheck;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region ITestVP Members

        /* bool PerformStringTest(string actualResult, string expectResult, VPCheckType type)
         * Check two strings.
         */
        public virtual bool CheckString(string actualResult, string expectResult, CheckType type)
        {
            OnBeforeCheck("CheckString", new object[] { actualResult, expectResult }, type);

            bool checkResult = false;

            try
            {
                if (actualResult != null && expectResult != null)
                {
                    if (type == CheckType.Equal)
                    {
                        checkResult = String.Compare(actualResult, expectResult, false) == 0;
                    }
                    else if (type == CheckType.NotEqual)
                    {
                        checkResult = String.Compare(actualResult, expectResult, false) != 0;
                    }
                    else if (type == CheckType.Small)
                    {
                        checkResult = actualResult.CompareTo(expectResult) < 0;
                    }
                    else if (type == CheckType.Larger)
                    {
                        checkResult = actualResult.CompareTo(expectResult) > 0;
                    }
                    else if (type == CheckType.SmallOrEqual)
                    {
                        checkResult = (String.Compare(actualResult, expectResult, false) == 0) || (actualResult.CompareTo(expectResult) < 0);
                    }
                    else if (type == CheckType.LargerOrEqual)
                    {
                        checkResult = (String.Compare(actualResult, expectResult, false) == 0) || (actualResult.CompareTo(expectResult) > 0);
                    }
                    else if (type == CheckType.Included)
                    {
                        checkResult = actualResult != "" && actualResult.IndexOf(expectResult) >= 0;
                    }
                    else if (type == CheckType.Excluded)
                    {
                        checkResult = actualResult != "" && actualResult.IndexOf(expectResult) < 0;
                    }
                    else if (type == CheckType.Existent)
                    {
                        checkResult = actualResult != "" && actualResult.IndexOf(expectResult) >= 0;
                    }
                    else if (type == CheckType.NotExistent)
                    {
                        checkResult = actualResult != "" && actualResult.IndexOf(expectResult) < 0;
                    }
                    else if (type == CheckType.Cross)
                    {
                        checkResult = CheckArray(actualResult.Split(' '), expectResult.Split(' '), CheckType.Cross);
                    }


                }

                return checkResult;
            }
            catch
            {
                return false;
            }
            finally
            {
                OnAfterCheck(checkResult, null, "CheckString", new object[] { actualResult, expectResult }, type);
            }
        }

        /* bool PerformArrayTest(Object[] expectArray, Object[] actualArray, VPCheckType type)
         * Check two arraies.
         */
        public virtual bool CheckArray(Object[] actualArray, Object[] expectArray, CheckType type)
        {

            OnBeforeCheck("CheckArray", new object[] { actualArray, expectArray }, type);

            bool checkResult = false;

            try
            {
                if (expectArray != null && actualArray != null)
                {
                    if (type == CheckType.Equal)
                    {
                        checkResult = true;

                        if (expectArray.Length == actualArray.Length)
                        {
                            for (int i = 0; i < expectArray.Length; i++)
                            {
                                if (!expectArray[i].Equals(actualArray[i]))
                                {
                                    checkResult = false;
                                    return checkResult;
                                }
                            }
                        }
                    }
                    else if (type == CheckType.NotEqual)
                    {
                        checkResult = false;

                        if (expectArray.Length != actualArray.Length)
                        {
                            checkResult = true;
                        }
                        else
                        {
                            for (int i = 0; i < expectArray.Length; i++)
                            {
                                if (!expectArray[i].Equals(actualArray[i]))
                                {
                                    checkResult = true;
                                    return checkResult;
                                }
                            }
                        }
                    }
                    else if (type == CheckType.Included)
                    {
                        checkResult = false;

                        if (actualArray.Length >= expectArray.Length)
                        {
                            foreach (Object eObj in expectArray)
                            {
                                foreach (Object aObj in actualArray)
                                {
                                    if (eObj.Equals(aObj))
                                    {
                                        checkResult = true;
                                        return checkResult;
                                    }
                                }
                            }
                        }
                    }
                    else if (type == CheckType.Excluded)
                    {
                        checkResult = true;

                        foreach (Object eObj in expectArray)
                        {
                            foreach (Object aObj in actualArray)
                            {
                                if (eObj.Equals(aObj))
                                {
                                    checkResult = false;
                                    return checkResult;
                                }
                            }
                        }
                    }
                    else if (type == CheckType.Cross)
                    {
                        bool same = false;
                        bool diff = false;

                        checkResult = false;

                        foreach (Object eObj in expectArray)
                        {
                            foreach (Object aObj in actualArray)
                            {
                                if (eObj.Equals(aObj))
                                {
                                    same = true;
                                }
                                else
                                {
                                    diff = true;
                                }

                                if (same && diff)
                                {
                                    checkResult = true;
                                    return checkResult;
                                }
                            }
                        }
                    }
                }

                return checkResult;
            }
            catch
            {
                return false;
            }
            finally
            {
                OnAfterCheck(checkResult, null, "CheckArray", new object[] { actualArray, expectArray }, type);
            }
        }

        public virtual bool CheckRegex(object testObj, string vpProperty, string expectReg, CheckType type, out String actualResult)
        {
            actualResult = null;

            if (testObj != null && !String.IsNullOrEmpty(vpProperty) && !String.IsNullOrEmpty(expectReg))
            {
                try
                {
                    TestObject obj = (TestObject)testObj;

                    actualResult = obj.GetProperty(vpProperty).ToString();

                    if (actualResult != null)
                    {
                        Regex reg = new Regex(expectReg);

                        //we treat "Equal" as "Match" here.
                        if (type == CheckType.Equal)
                        {
                            return reg.IsMatch(actualResult);
                        }
                        else if (type == CheckType.NotEqual)
                        {
                            return !reg.IsMatch(actualResult);
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public virtual bool CheckImage(String actualImgPath, String expectImgPath, CheckType type)
        {
            throw new NotImplementedException();
        }

        public virtual bool CheckDataTable(Object actualDataTable, Object expectedDataTable, CheckType type)
        {
            throw new NotImplementedException();
        }

        public virtual bool CheckTestObject(Object testObj, Object expectedObject, CheckType type)
        {
            if (type == CheckType.Equal)
            {
                return testObj.Equals(expectedObject);
            }
            else if (type == CheckType.NotEqual)
            {
                return !testObj.Equals(expectedObject);
            }

            return false;
        }

        /* bool CheckProperty(object testObj, string vpProperty, object expectResult, VPCheckType type, out object actualResult)
         * Check a proerpty of a test object.
         */
        public virtual bool CheckProperty(object testObj, string vpProperty, object expectResult, CheckType type, out object actualResult)
        {
            actualResult = null;

            if (testObj != null && !String.IsNullOrEmpty(vpProperty))
            {
                try
                {
                    TestObject obj = (TestObject)testObj;

                    //get actual property value
                    actualResult = obj.GetProperty(vpProperty);

                    if (actualResult != null && expectResult != null)
                    {
                        if (expectResult is String)
                        {
                            return CheckString(actualResult.ToString(), expectResult.ToString(), type);
                        }
                        else if (expectResult is Array)
                        {
                            return CheckArray((object[])actualResult, (object[])expectResult, type);
                        }
                        else if (type == CheckType.Equal)
                        {
                            return actualResult.Equals(expectResult);
                        }
                        else if (type == CheckType.NotEqual)
                        {
                            return !actualResult.Equals(expectResult);
                        }
                    }

                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /* bool CheckFile(String actualFile, String expectedFile, VPCheckType type)
         * Check two files.
         * 
         * type should be :
         * Equal: two files are exactly the same.
         * NotEqual: two files are not the same.
         * Existent: the expected file should be existed.
         * NotExistent: the expected file should not be existed.
         * Included: the actual file should include the expected file.
         * Excluded: the actual file should not include the expected file.
         */
        public virtual bool CheckFile(String actualFile, String expectedFile, CheckType type)
        {
            if (String.IsNullOrEmpty(actualFile) && String.IsNullOrEmpty(expectedFile))
            {
                return false;
            }

            if (type == CheckType.Existent)
            {
                string testFile = !String.IsNullOrEmpty(expectedFile) ? expectedFile : actualFile;

                return File.Exists(testFile);
            }
            else if (type == CheckType.NotExistent)
            {
                string testFile = !String.IsNullOrEmpty(expectedFile) ? expectedFile : actualFile;

                return !File.Exists(testFile);
            }
            else
            {
                try
                {
                    if (File.Exists(actualFile) && File.Exists(expectedFile))
                    {
                        FileInfo actualInfo = new FileInfo(actualFile);
                        FileInfo expectedInfo = new FileInfo(expectedFile);

                        if (type == CheckType.Equal)
                        {
                            //different size, can not be the same.
                            if (actualInfo.Length != expectedInfo.Length)
                            {
                                return false;
                            }
                            else
                            {
                                TextReader actualReader = File.OpenText(actualFile);
                                TextReader expectedReader = File.OpenText(expectedFile);

                                String actualLine;
                                String expectedLine;

                                bool result = true; ;

                                //compare line by line.
                                while (true)
                                {
                                    actualLine = actualReader.ReadLine();
                                    expectedLine = expectedReader.ReadLine();

                                    if (actualLine == null || expectedLine == null)
                                    {
                                        break;
                                    }

                                    //current line is not the same, return false.
                                    if (String.Compare(actualLine, expectedLine) != 0)
                                    {
                                        result = false;
                                        break;
                                    }
                                }

                                actualReader.Close();
                                expectedReader.Close();

                                return result;
                            }
                        }
                        else if (type == CheckType.NotEqual)
                        {
                            TextReader actualReader = File.OpenText(actualFile);
                            TextReader expectedReader = File.OpenText(expectedFile);

                            String actualLine;
                            String expectedLine;

                            bool result = false;

                            while (true)
                            {
                                actualLine = actualReader.ReadLine();
                                expectedLine = expectedReader.ReadLine();

                                if (actualLine == null || expectedLine == null)
                                {
                                    break;
                                }

                                //current line is not the same, return true.
                                if (String.Compare(actualLine, expectedLine) != 0)
                                {
                                    result = true;
                                    break;
                                }
                            }

                            actualReader.Close();
                            expectedReader.Close();

                            return result;
                        }
                        else if (type == CheckType.Included)
                        {
                            //actual file is smaller than expected file, so actual can not include expected.
                            if (actualInfo.Length < expectedFile.Length)
                            {
                                return false;
                            }
                            else
                            {
                                TextReader actualReader = File.OpenText(actualFile);
                                TextReader expectedReader = File.OpenText(expectedFile);

                                String actualContent = actualReader.ReadToEnd();
                                String expectedContent = actualReader.ReadToEnd();

                                bool result = true;

                                if (actualContent.IndexOf(expectedContent) < 0)
                                {
                                    result = false;
                                }

                                actualReader.Close();
                                expectedReader.Close();

                                return result;
                            }
                        }
                        else if (type == CheckType.Excluded)
                        {
                            TextReader actualReader = File.OpenText(actualFile);
                            TextReader expectedReader = File.OpenText(expectedFile);

                            String actualContent = actualReader.ReadToEnd();
                            String expectedContent = actualReader.ReadToEnd();

                            bool result = true;

                            if (actualContent.IndexOf(expectedContent) >= 0)
                            {
                                result = false;
                            }

                            actualReader.Close();
                            expectedReader.Close();

                            return result;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public virtual bool CheckNetwork(object testObj, object expectResult, CheckType type, out object actualNetwork)
        {
            throw new NotImplementedException();
        }

        public virtual void SetTestObjectPool(ITestObjectPool pool)
        {
            this._objPool = pool;
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion
    }
}
