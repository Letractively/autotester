/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestVP.cs
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

using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Core
{
    public class TestVP : ITestVP
    {

        #region fields


        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region ITestVP Members

        public virtual bool PerformStringTest(string actualResult, string expectResult, VPCheckType type)
        {
            if (actualResult != null && expectResult != null)
            {
                try
                {
                    if (type == VPCheckType.Equal)
                    {
                        return String.Compare(actualResult, expectResult, false) == 0;
                    }
                    else if (type == VPCheckType.NotEqual)
                    {
                        return String.Compare(actualResult, expectResult, false) != 0;
                    }
                    else if (type == VPCheckType.Small)
                    {
                        return actualResult.CompareTo(expectResult) < 0;
                    }
                    else if (type == VPCheckType.Larger)
                    {
                        return actualResult.CompareTo(expectResult) > 0;
                    }
                    else if (type == VPCheckType.SmallOrEqual)
                    {
                        return (String.Compare(actualResult, expectResult, false) == 0) || (actualResult.CompareTo(expectResult) < 0);
                    }
                    else if (type == VPCheckType.LargerOrEqual)
                    {
                        return (String.Compare(actualResult, expectResult, false) == 0) || (actualResult.CompareTo(expectResult) > 0);
                    }
                    else if (type == VPCheckType.Included)
                    {
                        return actualResult != "" && actualResult.IndexOf(expectResult) >= 0;
                    }
                    else if (type == VPCheckType.Cross)
                    {
                        return PerformArrayTest(expectResult.Split(' '), actualResult.Split(' '), VPCheckType.Cross);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public virtual bool PerformArrayTest(Object[] expectArray, Object[] actualArray, VPCheckType type)
        {
            if (expectArray != null && actualArray != null)
            {
                try
                {
                    if (type == VPCheckType.Equal)
                    {
                        if (expectArray.Length == actualArray.Length)
                        {
                            for (int i = 0; i < expectArray.Length; i++)
                            {
                                if (!expectArray[i].Equals(actualArray[i]))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }
                    }
                    else if (type == VPCheckType.NotEqual)
                    {
                        if (expectArray.Length != actualArray.Length)
                        {
                            return true;
                        }
                        else
                        {
                            for (int i = 0; i < expectArray.Length; i++)
                            {
                                if (!expectArray[i].Equals(actualArray[i]))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    else if (type == VPCheckType.Included)
                    {
                        if (actualArray.Length >= expectArray.Length)
                        {
                            foreach (Object eObj in expectArray)
                            {
                                foreach (Object aObj in actualArray)
                                {
                                    if (eObj.Equals(aObj))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else if (type == VPCheckType.Cross)
                    {
                        bool same = false;
                        bool diff = false;

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
                                    return true;
                                }
                            }
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

        public virtual bool PerformRegexTest(object testObj, string vpProperty, string expectReg, VPCheckType type, out String actualResult)
        {
            actualResult = null;

            if (testObj != null && !String.IsNullOrEmpty(vpProperty) && !String.IsNullOrEmpty(expectReg))
            {
                try
                {
                    TestObject obj = (TestObject)testObj;

                    actualResult = obj.GetPropertyByName(vpProperty).ToString();

                    if (actualResult != null)
                    {
                        Regex reg = new Regex(expectReg);

                        //we treat "Equal" as "Match" here.
                        if (type == VPCheckType.Equal)
                        {
                            return reg.IsMatch(actualResult);
                        }
                        else if (type == VPCheckType.NotEqual)
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

        public virtual bool PerformImageTest(String actualImgPath, String expectImgPath, VPCheckType type, out object actualImg)
        {
            throw new NotImplementedException();
        }

        public virtual bool PerformDataTableTest(Object actualDataTable, Object expectedDataTable, VPCheckType type)
        {
            throw new NotImplementedException();
        }

        public virtual bool PerformTestObjectTest(Object testObj, Object expectedObject, VPCheckType type)
        {
            throw new NotImplementedException();
        }

        public virtual bool PerformPropertyTest(object testObj, string vpProperty, object expectResult, VPCheckType type, out object actualResult)
        {
            actualResult = null;

            try
            {
                TestObject obj = (TestObject)testObj;

                //get actual property value
                actualResult = obj.GetPropertyByName(vpProperty);

                if (actualResult != null && expectResult != null)
                {
                    if (expectResult is String)
                    {
                        return PerformStringTest(actualResult.ToString(), expectResult.ToString(), type);
                    }
                    else if (expectResult is Array)
                    {
                        return PerformArrayTest((object[])expectResult, (object[])actualResult, type);
                    }
                    else if (type == VPCheckType.Equal)
                    {
                        return actualResult.Equals(expectResult);
                    }
                    else if (type == VPCheckType.NotEqual)
                    {
                        return !actualResult.Equals(expectResult);
                    }
                }

            }
            catch
            {
                return false;
            }

            return false;
        }

        public virtual bool PerformFileTest(String actualFile, String expectedFile, VPCheckType type)
        {
            if (type == VPCheckType.Existent)
            {
                return File.Exists(expectedFile);
            }
            else if (type == VPCheckType.NotExistent)
            {
                return !File.Exists(expectedFile);
            }
            else
            {
                try
                {
                    if (File.Exists(actualFile) && File.Exists(expectedFile))
                    {
                        FileInfo actualInfo = new FileInfo(actualFile);
                        FileInfo expectedInfo = new FileInfo(expectedFile);

                        if (type == VPCheckType.Equal)
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
                        else if (type == VPCheckType.NotEqual)
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
                        else if (type == VPCheckType.Included)
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
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public virtual bool PerformNetworkTest(object testObj, object expectResult, VPCheckType type, out object actualNetwork)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion
    }
}
