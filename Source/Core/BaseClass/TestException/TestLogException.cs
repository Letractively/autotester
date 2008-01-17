/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestLogException.cs
*
* Description: This component defines exceptions used for Log.
*
* History: 2007/12/28 wan,yu Init version
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinerain.AutoTester.Core
{

    public class TestLogException : TestException
    {
        public TestLogException()
            : this("Log expcetion.")
        {

        }
        public TestLogException(string message)
            : base(message)
        {

        }
    }

    public class CannotSaveScreenPrintException : TestLogException
    {
        public CannotSaveScreenPrintException()
            : this("Can not save screen print.")
        {

        }
        public CannotSaveScreenPrintException(string message)
            : base(message)
        {

        }
    };

    public class CannotWriteLogException : TestLogException
    {
        public CannotWriteLogException()
            : this("Can not write log.")
        {
        }
        public CannotWriteLogException(string message)
            : base(message)
        {

        }
    }

    public class CannotOpenTemplateException : TestLogException
    {
        public CannotOpenTemplateException()
            : this("Can not open log template file.")
        {

        }

        public CannotOpenTemplateException(string message)
            : base(message)
        {

        }
    };

    public class CannotSendMailExpcetion : TestLogException
    {
        public CannotSendMailExpcetion()
            : this("Can not send email.")
        {
        }

        public CannotSendMailExpcetion(string message)
            : base(message)
        {

        }
    }
}
