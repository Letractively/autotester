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

        public bool PerformStringTest(object testObj, string vpProperty, string expectResult, VPCheckType type, out object actualString)
        {
            throw new NotImplementedException();
        }

        public bool PerformRegexTest(object testObj, string vpProperty, string expectReg, VPCheckType type, out object actualResult)
        {
            throw new NotImplementedException();
        }

        public bool PerformImageTest(object testObj, string expectImgPath, VPCheckType type, out object actualImg)
        {
            throw new NotImplementedException();
        }

        public bool PerformDataTableTest(object testObj, object expectedDataTable, VPCheckType type, out object actualTable)
        {
            throw new NotImplementedException();
        }

        public bool PerformListBoxTest(object testObj, object expectedListBox, VPCheckType type, out object actualListBox)
        {
            throw new NotImplementedException();
        }

        public bool PerformComboBoxTest(object testObj, object expectedComboBox, VPCheckType type, out object actualComboBox)
        {
            throw new NotImplementedException();
        }

        public bool PerformTreeTest(object testObj, object expectedTree, VPCheckType type, out object actualTree)
        {
            throw new NotImplementedException();
        }

        public bool PerformPropertyTest(object testObj, string vpProperty, object expectResult, VPCheckType type, out object actualProperty)
        {
            throw new NotImplementedException();
        }

        public bool PerformFileTest(object testObj, string expectedFile, VPCheckType type, out object actualFile)
        {
            throw new NotImplementedException();
        }

        public bool PerformNetworkTest(object testObj, object expectResult, VPCheckType type, out object actualNetwork)
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
