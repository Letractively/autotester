
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

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Helper;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestObjectPool : ITestObjectPool
    {

        #region fields


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
            throw new NotImplementedException();
        }

        public void SetTestApp(ITestApp testApp)
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
