using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Shrinerain.AutoTester.Interface
{
    public interface ITestVP
    {
        bool PerformStringTest(String expectResutl, String actualResult);

        bool PerformRegexTest(String expectRegex, String actualResult);

        bool PerformImageTest(String expectImg, String actualImg);

        bool PerformDataTableTest(Object expectTable, Object actualTable);

        bool PerformListBoxTest(Object expectListBox, Object actualList);

        bool PerformComboBoxTest(Object expectComboBox, Object actualComboBox);

        bool PerformTreeTest(Object expectTree, Object actualTree);

        bool PerformPropertyTest(Object obj, String property, Object expectResult);

        bool PerformFileTest(String expectFile, String actualFile);

        bool PerformNetworkTest(object network, object expectResult);
    }
}
