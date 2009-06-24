/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestCheckPoint.cs
*
* Description: This class implements ITestVP
*              It provide the check point actions for HTML testing.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;


namespace Shrinerain.AutoTester.HTMLUtility
{
    public sealed class HTMLTestCheckPoint : TestCheckPoint
    {
        #region fields

        private static Regex _htmlTagReg = new Regex(@"<.*?>", RegexOptions.Singleline | RegexOptions.Compiled);

        private HTMLTestObjectPool _htmlObjPool;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        #endregion

        #region public methods

        #region ITestVP Members

        public override bool CheckString(string actualResult, string expectResult, CheckType type)
        {
            if (String.IsNullOrEmpty(actualResult))
            {
                try
                {
                    actualResult = this._htmlObjPool.TestBrower.GetAllHTMLContent();

                    if (String.IsNullOrEmpty(actualResult))
                    {
                        return false;
                    }
                    else
                    {
                        actualResult = _htmlTagReg.Replace(actualResult, "");
                    }
                }
                catch
                {
                    return false;
                }
            }

            return base.CheckString(actualResult, expectResult, type);
        }

        public override bool CheckTestObject(object testObj, object expectedObject, CheckType type)
        {
            if (type == CheckType.Existent)
            {
                if (expectedObject != null && expectedObject is HTMLTestGUIObject)
                {
                    //  int oriWaitSeconds = 0;

                    try
                    {
                        //if (this._htmlObjPool != null)
                        //{
                        //    oriWaitSeconds = this._htmlObjPool.MaxWaitSeconds;
                        //    this._htmlObjPool.MaxWaitSeconds = 3;
                        //}

                        HTMLTestGUIObject obj = (HTMLTestGUIObject)expectedObject;

                        String outerHTML = obj.GetProperty("outerHTML").ToString();

                        if (String.IsNullOrEmpty(outerHTML))
                        {
                            return false;
                        }

                        String curHTML = obj.Browser.GetAllHTMLContent();

                        curHTML = _htmlTagReg.Replace(curHTML, "");

                        return curHTML.IndexOf(outerHTML) > 0;
                    }
                    catch
                    {
                        return false;
                    }
                    finally
                    {
                        //if (oriWaitSeconds > 0 && this._htmlObjPool != null)
                        //{
                        //    this._htmlObjPool.MaxWaitSeconds = oriWaitSeconds;
                        //}
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return base.CheckTestObject(testObj, expectedObject, type);
            }
        }

        public override void SetTestObjectPool(ITestObjectPool pool)
        {
            if (pool != null)
            {
                try
                {
                    this._htmlObjPool = (HTMLTestObjectPool)pool;
                }
                catch (Exception ex)
                {
                    throw new TestObjectPoolExcpetion(ex.ToString());
                }
            }
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion
    }
}
