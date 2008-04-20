/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestObjectException.cs
*
* Description: This component defines exceptions used for test object.
*
* History: 2007/12/28 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.Core
{
    public class TestInterfaceException : TestException
    {
        public TestInterfaceException()
            : this("Error of test interface.")
        {
        }

        public TestInterfaceException(string message)
            : base(message)
        {
        }
    }

    public class TestObjectPoolExcpetion : TestInterfaceException
    {
        public TestObjectPoolExcpetion()
            : this("Error of Test Object pool.")
        {
        }

        public TestObjectPoolExcpetion(String message)
            : base(message)
        {
        }
    }

    public class NullObjectPoolException : TestObjectPoolExcpetion
    {
        public NullObjectPoolException()
            : this("Test object pool is null.")
        {
        }

        public NullObjectPoolException(String message)
            : base(message)
        {
        }
    }
}
