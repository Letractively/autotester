using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.TestExceptions
{
    public class TestPageException : TestBrowserException
    {
        public TestPageException()
            : this("Test page error.")
        {
        }

        public TestPageException(string message)
            : base(message)
        {
        }
    }

    public class CannotGetTestPageException : TestPageException
    {
        public CannotGetTestPageException()
            : this("Can not get test page.")
        {
        }

        public CannotGetTestPageException(string message)
            : base(message)
        {
        }
    }
}
