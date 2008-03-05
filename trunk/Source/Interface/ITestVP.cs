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

        bool PerformArrayTest(Object[] actualArray, Object[] expectArray, VPCheckType type);

        bool PerformRegexTest(Object testObj, String vpProperty, String expectReg, VPCheckType type, out String actualResult);

        bool PerformImageTest(String actualImgPath, String expectImgPath, VPCheckType type, out object actualImg);

        bool PerformDataTableTest(Object actualDataTable, Object expectedDataTable, VPCheckType type);

        bool PerformTestObjectTest(Object testObj, Object expectedListBox, VPCheckType type);

        bool PerformPropertyTest(Object testObj, String vpProperty, Object expectResult, VPCheckType type, out object actualProperty);

        bool PerformFileTest(String actualFile, String expectedFile, VPCheckType type);

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
