/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestObjectPool.cs
*
* Description: This interface define the methods provide by object pool.
*              Automation tester can get test object from object pool 
*              by these methods. 
*
* History:  2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestObjectPool
    {
        Object GetObjectByIndex(int index);

        Object GetObjectByRegex(string property, string regex);

        Object GetObjectByProperty(string property, string value);

        Object GetObjectByID(string id);

        Object GetObjectByName(string name);

        Object GetObjectByType(string type, string values, int index);

        Object GetObjectByAI(string value);

        Object GetObjectByPoint(int x, int y);

        Object GetObjectByRect(int top, int left, int width, int height);

        Object GetObjectByColor(string color); // color in 6 oct number, like FF0000

        Object GetObjectByCustomer(object value); // for future use.

        Object[] GetAllObjects();

        void SetTestBrowser(ITestBrowser testBrowser);
    }
}
