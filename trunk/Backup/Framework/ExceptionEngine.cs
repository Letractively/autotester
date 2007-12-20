/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ExceptionEngine.cs
*
* Description: ExceptionEngine will handle the exception of happend in 
*              framework, help CoreEngine how to deal with these exceptions. 
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class ExceptionEngine
    {

        #region fields

        //the exception happened 
        private TestException _currentException;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        /* bool HandleException(TestException e)
         * return true if we can just ignore the exception. 
         */
        public bool HandleException(TestException e)
        {
            _currentException = e;

            if (e is ItemNotFoundException)
            {
                return true;
            }
            else
            {

            }

            return false;
        }

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
