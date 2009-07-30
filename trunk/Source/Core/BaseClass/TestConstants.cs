using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{
    public struct TestConstants
    {
        #region class

        public const string IE_Server_Class = "Internet Explorer_Server";
        public const string IE_Dialog_Class = "Internet Explorer_TridentDlgFrame";
        public const string IE_TabWindow_Class = "TabWindowClass";
        public const string IE_ShellDocView_Class = "Shell DocObject View";
        public const string IE_FrameTab_Class = "Frame Tab";
        public const string IE_IEframe = "IEFrame";

        public const string WIN_Dialog_Class = "#32770";

        #endregion

        #region names

        public const string IE_Browser_Name = "Internet Explorer";
        public const string IE_EXE = "iexplore.exe";
        public const string IE_Process_Name = "iexplore";
        public const string IE_Reg_Path = @"Software\Microsoft\Internet Explorer";
        public const string IE_BlankPage_Url = "about:blank";
        public const string IE_ALREADY_REGISTERED = "IE_AlreadyRegistered";

        #endregion

        #region properties

        public const string PROPERTY_DOMAIN = "Domain";
        public const string PROPERTY_HTML = "HTML";
        public const string PROPERTY_MSAA = "MSAA";
        public const string PROPERTY_URL = "URL";

        public const string PROPERTY_ID = "ID";
        public const string PROPERTY_NAME = "Name";
        public const string PROPERTY_TAG = "Tag";
        public const string PROPERTY_TYPE = "Type";
        public const string PROPERTY_INNERTEXT = "InnerText";
        public const string PROPERTY_INNERHTML = "InnerHTML";
        public const string PROPERTY_OUTERHTML = "OuterHTML";
        public const string PROPERTY_OUTERTEXT = "OuterText";
        public const string PROPERTY_CAPTION = "Caption";
        public const string PROPERTY_LABEL = "Label";
        public const string PROPERTY_VALUE = "Value";
        public const string PROPERTY_ISCHECKED = "IsChecked";
        public const string PROPERTY_CLASS = "Class";
        public const string PROPERTY_VISIBLE = "VisibleText";


        #endregion

    }
}
