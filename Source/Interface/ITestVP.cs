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
*
*********************************************************************/

using System;

namespace Shrinerain.AutoTester.Interface
{
    [CLSCompliant(true)]
    public interface ITestVP
    {
        bool PerformStringTest(Object testObj, String vpProperty, String expectResult, VPCheckType type, out object actualString);

        bool PerformRegexTest(Object testObj, String vpProperty, String expectReg, VPCheckType type, out object actualResult);

        bool PerformImageTest(Object testObj, String expectImgPath, VPCheckType type, out object actualImg);

        bool PerformDataTableTest(Object testObj, Object expectedDataTable, VPCheckType type, out object actualTable);

        bool PerformListBoxTest(Object testObj, Object expectedListBox, VPCheckType type, out object actualListBox);

        bool PerformComboBoxTest(Object testObj, Object expectedComboBox, VPCheckType type, out object actualComboBox);

        bool PerformTreeTest(Object testObj, Object expectedTree, VPCheckType type, out object actualTree);

        bool PerformPropertyTest(Object testObj, String vpProperty, Object expectResult, VPCheckType type, out object actualProperty);

        bool PerformFileTest(Object testObj, String expectedFile, VPCheckType type, out object actualFile);

        bool PerformNetworkTest(object testObj, object expectResult, VPCheckType type, out object actualNetwork);
    }

    [CLSCompliant(true)]
    public enum VPCheckType
    {
        Small,         //<
        SmallOrEqual,  //<=
        Equal,         //==
        Larger,        //>
        LargerOrEqual, //>=
        Contain,       // some container object, like ListBox, one listbox may contains all the elements of another listbox
        Exclude,       // two container have NO same element.
        Cross,         // two container contain some same elements, but NOT ALL.
        Existed,       // check something whether it is exist.
        Text,          // check if the file/field is text.
        Binary,        // check if the file/field is binary.
        Connected,     // check network/software status.
        Disconnected    
    }
}
