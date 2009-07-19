using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.TestExceptions
{
    public class TestEventException : TestException
    {
        public TestEventException()
            : this("Test object exception.")
        {
        }
        public TestEventException(string message)
            : base(message)
        {
        }
    }
}
