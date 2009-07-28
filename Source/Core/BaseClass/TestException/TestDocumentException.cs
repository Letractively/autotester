using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.TestExceptions
{
    public class TestDocumentException : TestPageException
    {
        public TestDocumentException()
            : this("Test document error.")
        {
        }

        public TestDocumentException(string message)
            : base(message)
        {
        }
    }

    public class CannotGetTestDocumentException : TestDocumentException
    {
        public CannotGetTestDocumentException()
            : this("Can not get test page.")
        {
        }

        public CannotGetTestDocumentException(string message)
            : base(message)
        {
        }
    }
}
