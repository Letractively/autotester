using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Shrinerain.AutoTester.Function
{
    public class TestLog
    {
        #region fields

        protected string _template;

        #endregion

        #region properties

        public string Template
        {
            get { return this._template; }
            set
            {
                if (File.Exists(value))
                {
                    this._template = value;
                }
            }
        }

        #endregion
    }
}
