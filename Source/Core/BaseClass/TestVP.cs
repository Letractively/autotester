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
                    else if (type == VPCheckType.Contain)
                    {
                        return actualResult != "" && actualResult.IndexOf(expectResult) >= 0;
                    }
                    else if (type == VPCheckType.Cross)
                    {
                        return PerformArrayTest(actualResult.Split(' '), expectResult.Split(' '), VPCheckType.Cross);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public virtual bool PerformArrayTest(Object[] actualArray, Object[] expectArray, VPCheckType type)
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
                    else if (type == VPCheckType.Contain)
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

        public virtual bool PerformRegexTest(object testObj, string vpProperty, string expectReg, VPCheckType type, out object actualResult)
        {
            throw new NotImplementedException();
        }

        public virtual bool PerformImageTest(object testObj, string expectImgPath, VPCheckType type, out object actualImg)
        {
            throw new NotImplementedException();
        }

        public virtual bool PerformDataTableTest(Object testObj, Object expectedDataTable, VPCheckType type, out object actualTable)
        {
            throw new NotImplementedException();
        }

        public virtual bool PerformTestObjectTest(Object testObj, Object expectedListBox, VPCheckType type)
        {
            throw new NotImplementedException();
        }

        public virtual bool PerformPropertyTest(object testObj, string vpProperty, object expectResult, VPCheckType type, out object actualResult)
        {
            bool result = false;
            actualResult = null;

            try
            {
                TestObject obj = (TestObject)testObj;

                //get actual property value
                actualResult = obj.GetPropertyByName(vpProperty);

                if (actualResult != null && expectResult != null)
                {
                    if (actualResult is String && expectResult is String)
                    {
                        result = PerformStringTest(actualResult.ToString(), expectResult.ToString(), type);
                    }

                }

            }
            catch
            {
            }

            return result;
        }

        public virtual bool PerformFileTest(object testObj, string expectedFile, VPCheckType type, out object actualFile)
        {
            throw new NotImplementedException();
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
