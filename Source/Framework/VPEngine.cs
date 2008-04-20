/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: VPEngine.cs
*
* Description: VPEngine perform check point. It call ITestVP interface
*              to perform actual check. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class VPEngine
    {

        #region fields

        //interface to perform actual check.
        private ITestCheckPoint _testVP;

        // private AutoConfig _autoConfig = AutoConfig.GetInstance();

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public VPEngine()
        {
            LoadPlugin();
        }

        #endregion

        #region public methods

        /* bool PerformVPCheck(TestObject obj, string action, string vpProperty, object expectResult, out object actualResult)
         * return the vp result, return true if pass, false if failed. 
         * NOTICE: Need update!!! we need consider about the action, and then choose the right check type.
         */
        public bool PerformVPCheck(TestObject obj, string action, string vpProperty, object expectResult, out object actualResult)
        {
            bool result = false;

            actualResult = null;

            try
            {
                //currently, just support string test.
                result = _testVP.CheckProperty(obj, vpProperty, expectResult.ToString(), CheckType.Equal, out actualResult);
            }
            catch
            {
            }

            return result;
        }

        #endregion

        #region private methods

        /*  void LoadPlugin()
         *  load interface from TestFactory.
         */
        private void LoadPlugin()
        {
            this._testVP = TestFactory.CreateTestVP();
        }

        #endregion

        #endregion

    }
}
