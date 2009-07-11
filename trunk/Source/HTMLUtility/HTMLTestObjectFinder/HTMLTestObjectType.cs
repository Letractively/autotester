using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    // HTMLTestObjectType defines the object type we used in HTML Testing.
    public enum HTMLTestObjectTypeEnum
    {
        Unknow = 0,
        Any,
        Label,
        Button,
        CheckBox,
        RadioBox,
        TextBox,
        DropList,
        ListBox,
        Table,
        Image,
        Link,
        ActiveX,
        Dialog
    }

    public class HTMLTestObjectType : TestObjectType
    {
        public const String ActiveX = "ActiveX";
        public const String Dialog = "Dialog";
    }
}
