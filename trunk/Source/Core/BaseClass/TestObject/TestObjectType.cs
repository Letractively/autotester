using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

namespace Shrinerain.AutoTester.Core
{
    public class TestObjectType
    {
        #region fields

        public const String Unknown = "UnKnown";
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

        protected List<String> _validTypes = new List<string>();

        #endregion

        #region methods

        public TestObjectType()
        {
            FieldInfo[] fields = typeof(TestObjectType).GetFields();
            SetValidType(fields);
        }

        protected void SetValidType(FieldInfo[] fields)
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

        public virtual bool IsValidType(String typeStr)
        {
            if (!String.IsNullOrEmpty(typeStr) && _validTypes != null)
            {
                typeStr = typeStr.Trim().ToUpper();
                return _validTypes.Contains(typeStr);
            }

            return false;
        }

        #endregion
    }
}
