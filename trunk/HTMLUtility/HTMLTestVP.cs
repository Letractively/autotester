using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;
using Shrinerain.AutoTester.Win32;


namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestVP : ITestVP
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

        public bool PerformStringTest(object testObj, string vpProperty, string expectResult, VPCheckType type)
        {
            bool result = false;

            try
            {
                HTMLTestObject obj = (HTMLTestObject)testObj;

                string actualResult = obj.GetPropertyByName(vpProperty).ToString();
                if (!String.IsNullOrEmpty(actualResult))
                {
                    actualResult = actualResult.Trim();
                }

                string expectTmpResult = expectResult;
                if (!String.IsNullOrEmpty(expectTmpResult))
                {
                    expectTmpResult = expectTmpResult.Trim();
                }

                if (type == VPCheckType.Equal)
                {
                    if (String.Compare(actualResult, expectTmpResult, false) == 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.Small)
                {
                    if (actualResult.CompareTo(expectTmpResult) < 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.Larger)
                {
                    if (actualResult.CompareTo(expectTmpResult) > 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.SmallOrEqual)
                {
                    if ((String.Compare(actualResult, expectTmpResult, false) == 0) || actualResult.CompareTo(expectTmpResult) < 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.LargerOrEqual)
                {
                    if ((String.Compare(actualResult, expectTmpResult, false) == 0) || actualResult.CompareTo(expectTmpResult) > 0)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;

        }

        public bool PerformRegexTest(object testObj, string vpProperty, string expectReg, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformImageTest(object testObj, string expectImgPath, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformDataTableTest(object testObj, object expectedDataTable, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformListBoxTest(object testObj, object expectedListBox, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformComboBoxTest(object testObj, object expectedComboBox, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformTreeTest(object testObj, object expectedTree, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformPropertyTest(object testObj, string vpProperty, object expectResult, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformFileTest(object testObj, string expectedFile, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformNetworkTest(object testObj, object expectResult, VPCheckType type)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion


    }
}
