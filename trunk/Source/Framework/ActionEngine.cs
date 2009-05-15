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

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public ActionEngine()
        {
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
                (obj as IClickable).Click();
            }
            else if (action == "INPUT")
            {
                if (isDataNull)
                {
                    throw new CannotPerformActionException("Input data can not be empty.");
                }

                (obj as IInputable).Input(data);
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
                    (obj as ISelectable).Select(data);
                }
            }
            else if (action == "SELECTINDEX")
            {
                if (isDataNull)
                {
                    data = "0";
                }

                (obj as ISelectable).SelectByIndex(int.Parse(data));
            }
            else if (action == "CHECK")
            {
                (obj as ICheckable).Check();
            }
            else if (action == "UNCHECK")
            {
                (obj as ICheckable).UnCheck();
            }
            else if (action == "CLEAR")
            {
                (obj as IInputable).Clear();
            }
            else if (action == "HOVER")
            {
                (obj as IVisible).Hover();
            }
            else
            {
                throw new CannotPerformActionException("Unsupported action: " + action);
            }

        }

        #endregion

        #region private methods

        #endregion

        #endregion

    }
}
