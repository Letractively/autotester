using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestVP
    {
        bool PerformStringTest(Object testObj, String vpProperty, String expectResult, VPCheckType type);

        bool PerformRegexTest(Object testObj, String vpProperty, String expectReg, VPCheckType type);

        bool PerformImageTest(Object testObj, String expectImgPath, VPCheckType type);

        bool PerformDataTableTest(Object testObj, Object expectedDataTable, VPCheckType type);

        bool PerformListBoxTest(Object testObj, Object expectedListBox, VPCheckType type);

        bool PerformComboBoxTest(Object testObj, Object expectedComboBox, VPCheckType type);

        bool PerformTreeTest(Object testObj, Object expectedTree, VPCheckType type);

        bool PerformPropertyTest(Object testObj, String vpProperty, Object expectResult, VPCheckType type);

        bool PerformFileTest(Object testObj, String expectedFile, VPCheckType type);

        bool PerformNetworkTest(object testObj, object expectResult, VPCheckType type);
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
