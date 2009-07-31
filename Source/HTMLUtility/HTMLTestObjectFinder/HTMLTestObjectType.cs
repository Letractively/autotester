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

        public HTMLTestObjectType()
        {
        }

        protected override void SetValidType()
        {
            base.SetValidType();
            FieldInfo[] fields = typeof(HTMLTestObjectType).GetFields();
            SetValidType(fields);
        }

        #endregion
    }
}
