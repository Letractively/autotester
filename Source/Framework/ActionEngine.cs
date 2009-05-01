/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ActionEngine.cs
*
* Description: This class defines the actions of Framework.
*              ActionEngine recieve object and action parameters from
*              CoreEngine, perform actual action.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.Framework
{
    public sealed class ActionEngine
    {

        #region fields

        //interface to perform real action.
        private ITestAction _testAction;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public ActionEngine()
        {
            LoadPlugin();
        }

        #endregion

        #region public methods

        /*  void PerformAction(TestObject obj, string action, string data)
         *  perform actions on object.
         *  receive test object, action and data from CoreEngine.
         *  call ITestAction interface to perform real action.
         */
        public void PerformAction(TestObject obj, string action, string data)
        {
            if (obj == null || String.IsNullOrEmpty(action))
            {
                throw new CannotPerformActionException("TestObject and Action can not be null.");
            }

            bool isDataNull = String.IsNullOrEmpty(data);

            action = action.ToUpper();

            if (action == "CLICK")
            {
                _testAction.Click(obj);
            }
            else if (action == "INPUT")
            {
                if (isDataNull)
                {
                    throw new CannotPerformActionException("Input data can not be empty.");
                }

                _testAction.Input(obj, data);
            }
            else if (action == "INPUTKEYS")
            {
                if (isDataNull)
                {
                    throw new CannotPerformActionException("Inputkeys can not input empty data.");
                }

                _testAction.InputKeys(obj, data);
            }
            else if (action == "SELECT")
            {
                if (isDataNull)
                {
                    action = "SELECTINDEX";
                    data = "0";
                }
                else
                {
                    _testAction.Select(obj, data);
                }
            }
            else if (action == "SELECTINDEX")
            {
                if (isDataNull)
                {
                    data = "0";
                }
                _testAction.SelectIndex(obj, int.Parse(data));
            }
            else if (action == "CHECK")
            {
                _testAction.Check(obj);
            }
            else if (action == "UNCHECK")
            {
                _testAction.UnCheck(obj);
            }
            else if (action == "CLEAR")
            {
                _testAction.Clear(obj);
            }
            else if (action == "HOVER")
            {
                _testAction.Hover(obj);
            }
            else
            {
                throw new CannotPerformActionException("Unsupported action: " + action);
            }

        }

        #endregion

        #region private methods

        /* void LoadPlugin()
         * Load ITestAction interface to perform actions.
         * 
         */
        private void LoadPlugin()
        {
            _testAction = TestFactory.CreateTestAction();
        }

        #endregion

        #endregion

    }
}
