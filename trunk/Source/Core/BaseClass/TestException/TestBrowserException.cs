/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestBrowserException.cs
*
* Description: This component defines exceptions used for web application.
*
* History: 2007/12/28 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core.TestExceptions
{
    public class TestBrowserException : TestException
    {
        public TestBrowserException()
            : this("TestBrowse exception.")
        {

        }

        public TestBrowserException(string message)
            : base(message)
        {

        }
    };

    public class BrowserNotFoundException : TestBrowserException
    {
        public BrowserNotFoundException()
            : this("Can not find test browser.")
        {

        }
        public BrowserNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotStartBrowserException : TestBrowserException
    {
        public CannotStartBrowserException()
            : this("Can not start test browser.")
        {

        }
        public CannotStartBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CannotAttachBrowserException : TestBrowserException
    {
        public CannotAttachBrowserException()
            : this("Can not hook test browser.")
        {
        }
        public CannotAttachBrowserException(string message)
            : base(message)
        {

        }
    };
    public class CannotActiveBrowserException : TestBrowserException
    {
        public CannotActiveBrowserException()
            : this("Can not active test browser.")
        {

        }
        public CannotActiveBrowserException(string message)
            : base(message)
        {

        }
    };

    public class CannotStopBrowserException : TestBrowserException
    {
        public CannotStopBrowserException()
            : this("Can not stop test browser.")
        {

        }

        public CannotStopBrowserException(string message)
            : base(message)
        {

        }
    }

    public class CannotLoadUrlException : TestBrowserException
    {
        public CannotLoadUrlException()
            : this("Can not load the url, please check the url")
        {
        }
        public CannotLoadUrlException(string message)
            : base(message)
        {

        }
    }
    public class CannotNavigateException : TestBrowserException
    {
        public CannotNavigateException()
            : this("Can not navigate.")
        {
        }
        public CannotNavigateException(string message)
            : base(message)
        {

        }
    }
    public class CannotGetBrowserInfoException : TestBrowserException
    {
        public CannotGetBrowserInfoException()
            : this("Can not get information from test browser.")
        {
        }

        public CannotGetBrowserInfoException(string message)
            : base(message)
        {

        }
    }

    public class CannotPrintException : TestBrowserException
    {
        public CannotPrintException()
            : this("Can not print current page.")
        {
        }

        public CannotPrintException(string message)
            : base(message)
        {
        }
    }

}
