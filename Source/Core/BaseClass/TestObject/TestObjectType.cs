using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Core
{
    //defines the supported types.
    public class TestObjectType
    {
        #region fields

        public const String Unknown = "UnKnown";
        public const String AnyType = "AnyType";
        public const String Button = "Button";
        public const String TextBox = "TextBox";
        public const String Label = "Label";
        public const String CheckBox = "CheckBox";
        public const String RadioBox = "RadioBox";
        public const String DropList = "DropList";
        public const String ListBox = "ListBox";
        public const String Link = "Link";
        public const String Image = "Image";
        public const String Table = "Table";

        protected static List<String> _validTypes = new List<string>();

        #endregion

        #region methods

        static TestObjectType()
        {
            FieldInfo[] fields = typeof(TestObjectType).GetFields();
            SetValidType(fields);
        }

        protected static void SetValidType(FieldInfo[] fields)
        {
            foreach (FieldInfo fi in fields)
            {
                if (fi.IsLiteral && fi.IsPublic && fi.IsStatic)
                {
                    String fieldValue = fi.GetValue(null).ToString().Trim().ToUpper();
                    if (!_validTypes.Contains(fieldValue))
                    {
                        _validTypes.Add(fieldValue);
                    }
                }
            }
        }

        public static List<String> GetValidTypes()
        {
            return _validTypes;
        }

        public static bool IsValidType(String typeStr)
        {
            if (!String.IsNullOrEmpty(typeStr))
            {
                typeStr = typeStr.Trim().ToUpper();
                return _validTypes.Contains(typeStr);
            }

            return false;
        }

        #endregion
    }
}
