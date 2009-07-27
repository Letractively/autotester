using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.Core
{
    public class TestWindow : ITestWindow
    {
        #region ITestWindow Members

        public string Caption
        {
            get { throw new NotImplementedException(); }
        }

        public string ClassName
        {
            get { throw new NotImplementedException(); }
        }

        public IntPtr Handle
        {
            get { throw new NotImplementedException(); }
        }

        public ITestApp App
        {
            get { throw new NotImplementedException(); }
        }

        public ITestObjectMap Objects
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
