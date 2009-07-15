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
*           2007/11/20 wan,yu update, add void SetTestApp() 
*           2008/01/22 wan,yu update, add GetLastObject()   
*           2008/01/26 wan,yu update, update GetObjectByRect()     
*
*********************************************************************/

namespace Shrinerain.AutoTester.Core
{
    using System;
    using System.Collections.Generic;

    public interface ITestObjectPool
    {
        event TestObjectEventHandler OnObjectFound;

        /* Object GetObjectByIndex(int index);
         * index means the 1st object, the 2nd object.
         */
        TestObject GetObjectByIndex(int index);

        /* Object GetObjectByProperty(string property, string value);
         * find object by an internal property, eg: find a button which .id is "btn1"
         */
        TestObject[] GetObjectsByProperties(TestProperty[] properties);

        /* Object GetObjectByID(string id);
         * find object by ".id"
         */
        TestObject GetObjectByID(string id);

        /* Object GetObjectByName(string name);
         * find object by ".name."
         */
        TestObject[] GetObjectsByName(string name);

        /* Object GetObjectByType(string type, string values, int index);
         * find object by "type", eg: GetObjectByType("button","OK",0)
         */
        TestObject[] GetObjectsByType(string type, TestProperty[] properties);


        /* GetObjectByPoint(int x, int y);
         * find object at specfic point.
         */
        TestObject GetObjectByPoint(int x, int y);

        /* Object GetObjectByRect(int top, int left, int width, int height, string typeStr, bool isPercent);
         * find object by specfic rect, type means what type of the object you want return, eg: button
         */
        TestObject GetObjectByRect(int top, int left, int width, int height, string type, bool isPercent);

        /* Object[] GetAllObjects();
         * return all objects in the object pool.
         */
        TestObject[] GetAllObjects();

        /* Object GetLastObject();
         * return the last test object we have got.
         */
        TestObject GetLastObject();

        /* void SetTestApp(ITestApp testApp);
         * set desktop application inder test.
         */
        void SetTestApp(ITestApp testApp);
        void SetTestBrowser(ITestBrowser browser);

        int SearchTimeout { set; get; }

        bool FuzzySearch{ set; get; }
    }
}
