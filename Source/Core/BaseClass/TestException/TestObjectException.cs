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

namespace Shrinerain.AutoTester.Core.TestExceptions
{

    public class TestObjectException : TestPageException
    {
        public TestObjectException()
            : this("Test object exception.")
        {

        }
        public TestObjectException(string message)
            : base(message)
        {

        }
        public TestObjectException(TestObject obj, string message)
            : base(message)
        {
            this._obj = obj;
        }
    };

    public class ObjectNotFoundException : TestObjectException
    {
        public ObjectNotFoundException()
            : this("Can not found test object.")
        {

        }
        public ObjectNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotBuildObjectException : TestObjectException
    {
        public CannotBuildObjectException()
            : this("Can not build test object.")
        {
        }
        public CannotBuildObjectException(string message)
            : base(message)
        {
        }
    }
    public class CannotGetObjectPositionException : TestObjectException
    {
        public CannotGetObjectPositionException()
            : this("Can not get the position of the object.")
        {
        }
        public CannotGetObjectPositionException(string message)
            : base(message)
        {

        }
    }
    public class ItemNotFoundException : TestObjectException
    {
        public ItemNotFoundException()
            : this("Can not found sub item.")
        {

        }
        public ItemNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class PropertyNotFoundException : TestObjectException
    {
        public PropertyNotFoundException()
            : this("Can not find the property.")
        {

        }
        public PropertyNotFoundException(string message)
            : base(message)
        {

        }
    };
    public class CannotSetPropertyException : TestObjectException
    {
        public CannotSetPropertyException()
            : this("Can not set the property.")
        {
        }
        public CannotSetPropertyException(string message)
            : base(message)
        {

        }
    }
    public class CannotPerformActionException : TestObjectException
    {
        public CannotPerformActionException()
            : this("Can not perform the action.")
        {

        }
        public CannotPerformActionException(string message)
            : base(message)
        {

        }
    };

    public class TimeoutException : TestObjectException
    {
        public TimeoutException()
            : this("Time is out.")
        {

        }
        public TimeoutException(string message)
            : base(message)
        {

        }
    };

    public class VerifyPointException : TestObjectException
    {
        public VerifyPointException()
            : this("Can not perform VP check.")
        {

        }
        public VerifyPointException(string message)
            : base(message)
        {

        }
    };

    public class CannotHighlightObjectException : TestObjectException
    {
        public CannotHighlightObjectException()
            : this("Can not highlight the object.")
        {

        }
        public CannotHighlightObjectException(string message)
            : base(message)
        {

        }
    };

    public class CannotSaveControlPrintException : TestObjectException
    {
        public CannotSaveControlPrintException()
            : this("Can not save control print.")
        {
        }

        public CannotSaveControlPrintException(string message)
            : base(message)
        {
        }
    }

    public class FuzzySearchException : TestObjectException
    {
        public FuzzySearchException()
            : this("Fuzzy Search Error.")
        {
        }

        public FuzzySearchException(string message)
            : base(message)
        {
        }
    }

    public class CannotAddMapObjectException : TestObjectException
    {
        public CannotAddMapObjectException()
            : this("Can not add test object to map.")
        {
        }

        public CannotAddMapObjectException(String message)
            : base(message)
        {
        }
    }

    public class CannotGetMapObjectException : TestObjectException
    {
        public CannotGetMapObjectException()
            : this("Can not get test object from map.")
        {
        }

        public CannotGetMapObjectException(string message)
            : base(message)
        {
        }
    }

    public class TestPropertyException : TestObjectException
    {
        public TestPropertyException()
            : this("Error of test property.")
        {
        }

        public TestPropertyException(string message)
            : base(message)
        {
        }
    }

    public class CannotParseTestPropertyException : TestPropertyException
    {
        public CannotParseTestPropertyException()
            : this("Can not parse test property.")
        {
        }
        public CannotParseTestPropertyException(string message)
            : base(message)
        {
        }
    }

    public class CannotGetChildrenException : TestObjectException
    {
        public CannotGetChildrenException()
            : this("Can not get children.")
        {
        }
        public CannotGetChildrenException(String message)
            : base(message)
        {
        }
    }
}
