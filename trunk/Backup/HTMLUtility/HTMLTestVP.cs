/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestVP.cs
*
* Description: This class implements ITestVP
*              It provide the check point actions for HTML testing.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.Interface;
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

        /* bool PerformStringTest(object testObj, string vpProperty, string expectResult, VPCheckType type, out object actualResult)
         * return true if the expected string is the same with the expected property.
         * 
         */
        public bool PerformStringTest(object testObj, string vpProperty, string expectResult, VPCheckType type, out object actualResult)
        {

            bool result = false;
            actualResult = null;

            try
            {
                HTMLTestObject obj = (HTMLTestObject)testObj;

                //get actual property value
                actualResult = obj.GetPropertyByName(vpProperty).ToString();

                if (!String.IsNullOrEmpty(actualResult.ToString()))
                {
                    actualResult = actualResult.ToString().Trim();
                }

                string expectTmpResult = expectResult;
                if (!String.IsNullOrEmpty(expectTmpResult))
                {
                    expectTmpResult = expectTmpResult.Trim();
                }

                if (type == VPCheckType.Equal)
                {
                    if (String.Compare(actualResult.ToString(), expectTmpResult, false) == 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.Small)
                {
                    if (actualResult.ToString().CompareTo(expectTmpResult) < 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.Larger)
                {
                    if (actualResult.ToString().CompareTo(expectTmpResult) > 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.SmallOrEqual)
                {
                    if ((String.Compare(actualResult.ToString(), expectTmpResult, false) == 0) || actualResult.ToString().CompareTo(expectTmpResult) < 0)
                    {
                        result = true;
                    }
                }
                else if (type == VPCheckType.LargerOrEqual)
                {
                    if ((String.Compare(actualResult.ToString(), expectTmpResult, false) == 0) || actualResult.ToString().CompareTo(expectTmpResult) > 0)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
            }

            return result;

        }

        public bool PerformRegexTest(object testObj, string vpProperty, string expectReg, VPCheckType type, out object actualResult)
        {
            actualResult = null;

            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformImageTest(object testObj, string expectImgPath, VPCheckType type, out object actualImg)
        {
            actualImg = null;
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformDataTableTest(object testObj, object expectedDataTable, VPCheckType type, out object actualTable)
        {
            actualTable = null;
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformListBoxTest(object testObj, object expectedListBox, VPCheckType type, out object actualListBox)
        {
            actualListBox = null;
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformComboBoxTest(object testObj, object expectedComboBox, VPCheckType type, out object actualComboBox)
        {
            actualComboBox = null;
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformTreeTest(object testObj, object expectedTree, VPCheckType type, out object actualTree)
        {
            actualTree = null;
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformPropertyTest(object testObj, string vpProperty, object expectResult, VPCheckType type, out object actualProperty)
        {
            actualProperty = null;
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformFileTest(object testObj, string expectedFile, VPCheckType type, out object actualFile)
        {
            actualFile = null;
            throw new Exception("The method or operation is not implemented.");
        }

        public bool PerformNetworkTest(object testObj, object expectResult, VPCheckType type, out object actualNetwork)
        {
            actualNetwork = null;
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion


    }
}
