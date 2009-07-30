using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.TestExceptions
{
    public class TestWindowException : TestAppException
    {
        public TestWindowException()
            : this("Test window error.")
        {
        }

        public TestWindowException(string message)
            : base(message)
        {
        }
    }

    public class CannotGetTestWindowException : TestWindowException
    {
        public CannotGetTestWindowException()
            : this("Can not get test window.")
        {
        }

        public CannotGetTestWindowException(string message)
            : base(message)
        {
        }
    }
}
