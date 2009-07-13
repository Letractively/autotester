using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestObjectType : TestObjectType
    {
        #region fields

        public const String Tab = "Tab";
        public const String ComboBox = "ComboBox";
        public const String MenuBar = "MenuBar";
        public const String ScrollBar = "ScrollBar";
        public const String Window = "Window";
        public const String MenuPopup = "MenuPopup";
        public const String MenuItem = "MenuItem";
        public const String ToolTip = "ToolTip";
        public const String Document = "Document";
        public const String Pane = "Pane";
        public const String Border = "Border";
        public const String Separator = "Separator";
        public const String Cell = "Cell";
        public const String HelpBalloon = "HelpBallon";
        public const String PageTab = "PageTab";
        public const String ProgressBar = "ProgressBar";
        public const String Clock = "Clock";

        #endregion

        #region methods

        public MSAATestObjectType()
            : base()
        {
            FieldInfo[] fields = typeof(MSAATestObjectType).GetFields();
            SetValidType(fields);
        }


        #endregion

    }
}
