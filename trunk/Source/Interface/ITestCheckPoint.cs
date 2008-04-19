/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestCheckPoint.cs
*
* Description: This interface define the check point methods provide 
*              by AutoTester, we can perform these kind of check point.  
*
* History:  2007/09/04 wan,yu Init version
*           2008/03/02 wan,yu update, add PerformTestObjectTest() and PerformArrayTest().
*           2008/04/07 wan,yu update, rename to ITestCheckPoint and rename Perfrom*Test() to Check*().           
*
*********************************************************************/

using System;

namespace Shrinerain.AutoTester.Interface
{
    [CLSCompliant(true)]
    public interface ITestCheckPoint
    {
        bool CheckString(String actualString, String expectResult, CheckType type);

        bool CheckArray(Object[] actualArray, Object[] expectArray, CheckType type);

        bool CheckRegex(Object testObj, String vpProperty, String expectReg, CheckType type, out String actualResult);

        bool CheckImage(String actualImgPath, String expectImgPath, CheckType type);

        bool CheckDataTable(Object actualDataTable, Object expectedDataTable, CheckType type);

        bool CheckTestObject(Object testObj, Object expectedListBox, CheckType type);

        bool CheckProperty(Object testObj, String vpProperty, Object expectResult, CheckType type, out object actualProperty);

        bool CheckFile(String actualFile, String expectedFile, CheckType type);

        bool CheckNetwork(object testObj, object expectResult, CheckType type, out object actualNetwork);

        void SetTestObjectPool(ITestObjectPool pool);
    }

    [CLSCompliant(true)]
    public enum CheckType
    {
        Small,         //actual < expected
        SmallOrEqual,  //actual <= expected
        Equal,         //actual == expected
        NotEqual,      //actual != expected
        Larger,        //actual > expected
        LargerOrEqual, //actual >= expected
        Included,      //actual should contain expected
        Excluded,      //actual should not contain expected
        Cross,         //two set contain some same elements, but NOT ALL.
        Existent,      //expected is existed.
        NotExistent,
        Text,          //actual file/database is text.
        Binary,        //actual file/database is binary.
        Connected,     //actual network status.
        Disconnected
    }
}
