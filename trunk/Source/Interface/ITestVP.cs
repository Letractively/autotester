/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: ITestVP.cs
*
* Description: This interface define the check point methods provide 
*              by AutoTester, we can perform these kind of check point.  
*
* History:  2007/09/04 wan,yu Init version
*           2008/03/02 wan,yu update, add PerformTestObjectTest() and PerformArrayTest().
*
*********************************************************************/

using System;

namespace Shrinerain.AutoTester.Interface
{
    [CLSCompliant(true)]
    public interface ITestVP
    {
        bool PerformStringTest(String actualString, String expectResult, VPCheckType type);

        bool PerformArrayTest(Object[] expectArray, Object[] actualArray, VPCheckType type);

        bool PerformRegexTest(Object testObj, String vpProperty, String expectReg, VPCheckType type, out object actualResult);

        bool PerformImageTest(Object testObj, String expectImgPath, VPCheckType type, out object actualImg);

        bool PerformDataTableTest(Object testObj, Object expectedDataTable, VPCheckType type, out object actualTable);

        bool PerformTestObjectTest(Object testObj, Object expectedListBox, VPCheckType type);

        bool PerformPropertyTest(Object testObj, String vpProperty, Object expectResult, VPCheckType type, out object actualProperty);

        bool PerformFileTest(Object testObj, String expectedFile, VPCheckType type, out object actualFile);

        bool PerformNetworkTest(object testObj, object expectResult, VPCheckType type, out object actualNetwork);
    }

    [CLSCompliant(true)]
    public enum VPCheckType
    {
        Small,         //actual < expected
        SmallOrEqual,  //actual <= expected
        Equal,         //actual == expected
        NotEqual,      //actual != expected
        Larger,        //actual > expected
        LargerOrEqual, //actual >= expected
        Contain,       //actual is a super set of expected
        Cross,         //two set contain some same elements, but NOT ALL.
        Existed,       //expected is existed.
        Text,          //actual file/database is text.
        Binary,        //actual file/database is binary.
        Connected,     //actual network status.
        Disconnected
    }
}
