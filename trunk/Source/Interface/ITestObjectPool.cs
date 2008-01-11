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
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Interface
{
    [CLSCompliant(true)]
    public interface ITestObjectPool
    {
        /* Object GetObjectByIndex(int index);
         * index means the 1st object, the 2nd object.
         */
        Object GetObjectByIndex(int index);

        /* Object GetObjectByProperty(string property, string value);
         * find object by an internal property, eg: find a button which .id is "btn1"
         */
        Object GetObjectByProperty(string property, string value);

        /* Object GetObjectByRegex(string property, string regex);
         * find object by an internal property, but the value is a regex, eg: "btn\d", you can find "btn1", "btn2"... 
         */
        Object GetObjectByRegex(string property, string regex);

        /* Object GetObjectBySimilarProperties(string[] properties, string[] values, int[] similarity, bool useAll);
         * find object by some internal properties, and  the value don't need to match the actual value 100%, you
         * can specify the similarity.
         * eg: you want to find a button, it's ".id" is "btn1", but actually, the button's id is "btn2",
         * if you use GetObjectByProperty(".id","btn1"), you can not find the button. but if you use 
         * GetObjectBySimilarProperties(new string[]{".id"}, new string[]{"btn1"}, new int[]{50}, true).
         * you can find the object.
         * the 3rd parameter means the similarity between expected value and actual value.
         * 0< similarity < 100, means the 0% to 100% match.
         */
        Object GetObjectBySimilarProperties(string[] properties, string[] values, int[] similarity, bool useAll);

        /* Object GetObjectByID(string id);
         * find object by ".id"
         */
        Object GetObjectByID(string id);

        /* Object GetObjectByName(string name);
         * find object by ".name."
         */
        Object GetObjectByName(string name);

        /* Object GetObjectByType(string type, string values, int index);
         * find object by "type", eg: GetObjectByType("button","OK",0)
         * index means if there are more than one object, which you want to return.
         * nomrally, we will specify 0.
         */
        Object GetObjectByType(string type, string values, int index);

        /* GetObjectByAI(string value);
         * find object a single string
         */
        Object GetObjectByAI(string value);

        /* GetObjectByPoint(int x, int y);
         * find object at specfic point.
         */
        Object GetObjectByPoint(int x, int y);

        /* Object GetObjectByRect(int top, int left, int width, int height, string type);
         * find object by specfic rect, type means what type of the object you want return, eg: button
         */
        Object GetObjectByRect(int top, int left, int width, int height, string type);

        /* Object GetObjectByColor(string color);  
         * color in 6 oct number, like FF0000
         */
        Object GetObjectByColor(string color);

        /* Object GetObjectByCustomer(object value);
         * for future use.
         */
        Object GetObjectByCustomer(object value);

        /* Object[] GetAllObjects();
         * return all objects in the object pool.
         */
        Object[] GetAllObjects();

        /* void SetTestBrowser(ITestBrowser testBrowser);
         * set the browser under test.
         */
        void SetTestBrowser(ITestBrowser testBrowser);

        /* void SetTestApp(ITestApp testApp);
         * set desktop application inder test.
         */
        void SetTestApp(ITestApp testApp);
    }
}
