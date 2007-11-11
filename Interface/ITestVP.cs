using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Shrinerain.AutoTester.Interface
{
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

    public enum VPCheckType
    {
        Small,
        SmallOrEqual,
        Equal,
        Larger,
        LargerOrEqual,
        Contain,
        Exclude,
        Cross,
        Existed,
        Text,
        Binary,
        Connected,
        Disconnected
    }
}
