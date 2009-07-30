using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestObjectType : TestObjectType
    {
        #region fileds

        public const String ActiveX = "ActiveX";
        public const String Dialog = "Dialog";
        public const String Form = "Form";

        #endregion

        #region methods

        static HTMLTestObjectType()
        {
            FieldInfo[] fields = typeof(HTMLTestObjectType).GetFields();
            SetValidType(fields);
        }

        public static List<String> GetValidTypes()
        {
            return TestObjectType.GetValidTypes();
        }

        public static bool IsValidType(String typeStr)
        {
            return TestObjectType.IsValidType(typeStr);
        }

        #endregion
    }
}
