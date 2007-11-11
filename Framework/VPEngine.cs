using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class VPEngine
    {

        #region fields

        private ITestVP _testVP;

        private AutoConfig _autoConfig = AutoConfig.GetInstance();

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

        public bool PerformVPCheck(TestObject obj, string action, string vpProperty, object expectResult, out object actualResult)
        {
            bool result = false;

            actualResult = null;

            try
            {
                result = _testVP.PerformStringTest(obj, vpProperty, expectResult.ToString(), VPCheckType.Equal, out actualResult);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        #endregion

        #region private methods

        private void LoadPlugin()
        {
            this._testVP = TestFactory.CreateTestVP();
        }

        #endregion

        #endregion

    }
}
