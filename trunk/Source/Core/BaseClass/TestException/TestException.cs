/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestException.cs
*
* Description: TestException defines the exception used in AutoTester.
*              The root exception is TestException class.
*              Then we have TestObjectException, TestBrowserException etc.
*
* History: 2007/09/04 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Shrinerain.AutoTester.Core
{
    [Serializable]
    public class TestException : ApplicationException
    {
        #region fields

        //domain means the kind of test object. for example, HTML is a domain.
        protected string _domain;

        //if web testing, we may have url
        protected string _url;

        //if desktop application, we may have app name.
        protected string _app;

        //the source object which throw the expcetion.
        protected TestObject _obj;
        #endregion

        #region Properties
        public string Domain
        {
            get { return this._domain; }
            set
            {
                this._domain = value;
            }
        }

        public string Url
        {
            get { return this._url; }
            set
            {
                this._url = value;
            }
        }

        public string App
        {
            get { return _app; }
            set { _app = value; }
        }

        public TestObject SourceObject
        {
            get
            {
                return this._obj;
            }
            set
            {
                this._obj = value;
            }
        }

        #endregion

        #region public methods
        public TestException()
            : base()
        {

        }
        public TestException(string message)
            : base(message)
        {

        }
        protected TestException(SerializationInfo sInfo, StreamingContext sc)
            : base(sInfo, sc)
        {

        }
        public TestException(string domain, string message)
            : base(message)
        {
            this._domain = domain;
        }
        public TestException(string message, Exception e)
            : base(message, e)
        {

        }
        public TestException(string domain, string url, string message)
            : base(message)
        {
            this._domain = domain;
            this._url = url;
        }
        public TestException(TestObject obj, string domain, string url, string message)
            : base(message)
        {
            this._obj = obj;
            this._url = url;
            this._domain = domain;
        }
        #endregion
    }

    #region sub exceptions


    #endregion
}
