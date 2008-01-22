/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MFCTestObjectPool.cs
*
* Description: MFC test object pool provide methods to get a test object
*              from application under test. 
*
* History: 2008/01/14 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MFCUtility
{
    public sealed class MFCTestObjectPool : ITestObjectPool
    {

        #region fields

        private MFCTestApp _mfcTestApp;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public MFCTestObjectPool()
        {
        }

        public MFCTestObjectPool(ITestApp testApp)
        {
            this._mfcTestApp = (MFCTestApp)testApp;
        }

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
            throw new NotImplementedException();
        }

        public object GetObjectByAI(string value)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByPoint(int x, int y)
        {
            throw new NotImplementedException();
        }

        public object GetObjectByRect(int top, int left, int width, int height, string type)
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
            return null;
        }

        public void SetTestBrowser(ITestBrowser testBrowser)
        {
            throw new NotImplementedException();
        }

        public void SetTestApp(ITestApp testApp)
        {
            this._mfcTestApp = (MFCTestApp)testApp;
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion
    }
}
