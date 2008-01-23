/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ActionHelper.cs
*
* Description: ActionHelper help us to simplify object conversion.
*              Because ObjectPool will return an "Object" each time.
*              we neet to convert Object to other TestObject to perform
*              actions. eg: Convert Object to "HTMLTestButton". ActionHleper
*              let us don't to need to convert explicit every time. 
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
    public sealed class ActionHelper
    {
        #region fields

        private ITestObjectPool _objPool;
        private ITestAction _actionPool;

        TestObject _testObj;
        // IInteractive _interObj;

        #endregion

        #region properties

        public ITestObjectPool TestObjectPool
        {
            get { return _objPool; }
            set { _objPool = value; }
        }

        public ITestAction TestAction
        {
            get { return _actionPool; }
            set { _actionPool = value; }
        }


        #endregion

        #region methods

        #region ctor

        public ActionHelper()
        {
        }

        public ActionHelper(ITestObjectPool objPool, ITestAction action)
        {
            this._objPool = objPool;
            this._actionPool = action;
        }

        #endregion

        #region public methods

        /* void PerformAction(object data)
         * perform default action of an object.
         * Convert the object to IInteractive interface.
         */
        public void PerformAction()
        {
            PerformAction(null);
        }

        public void PerformAction(object data)
        {
            try
            {
                //get object in the pool.
                _testObj = (TestObject)_objPool.GetLastObject();

                if (_testObj == null)
                {
                    throw new ObjectNotFoundException();
                }

                //cast to IInteractive interface.
                (_testObj as IInteractive).PerformDefaultAction(data);
            }
            catch (ObjectNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform action: " + ex.Message);
            }

        }

        #endregion

        #region private methods

        #endregion

        #endregion
    }
}
