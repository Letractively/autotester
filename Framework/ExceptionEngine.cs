using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class ExceptionEngine
    {

        #region fields

        private TestException _exception;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        public bool HandleException(TestException e)
        {
            if (e is TestEngineException)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
