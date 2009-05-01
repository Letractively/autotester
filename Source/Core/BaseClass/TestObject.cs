/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: TestObject.cs
*
* Description: TestObject class is the base class in AutoTester.
*              TestObject defines some standard properties and methods
*              for testing.
*              The actual test object must inherit TestObject.
*
* History:  2007/09/04 wan,yu Init version
*           2008/01/14 wan,yu update, remove id,name,handle,class from TestObject 
*
*********************************************************************/


namespace Shrinerain.AutoTester.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.Serialization;

    [Serializable]
    public class TestObject
    {
        #region fields

        protected TestApp _parentApp;

        //domain means the object type, eg: Win32
        protected string _domain;

        protected const string _visibleProperty = "VisibleProperty";

        #endregion

        #region properties

        public string Domain
        {
            get { return this._domain; }
        }

        public TestApp ParentApp
        {
            get { return _parentApp; }
            set { _parentApp = value; }
        }

        public static string VisibleProperty
        {
            get { return _visibleProperty; }
        }

        #endregion

        #region public methods

        public TestObject()
            : this(null)
        {
        }

        public TestObject(TestApp app)
            : this(app, "Unknow")
        {
        }

        public TestObject(TestApp app, String domain)
        {
            this._parentApp = app;
            this._domain = domain;
        }

        #region public method

        /*  object GetProperty(string propertyName)
         *  get the expected property value.
         */
        public virtual object GetProperty(string propertyName)
        {
            return null;
        }

        /* bool SetProperty(string propertyName, object value)
         * set the expected property, return true if successful.
         */
        public virtual bool SetProperty(string propertyName, object value)
        {
            return false;
        }

        //these properties is used to identify an object.
        //we will record these properties, and when playing back, use these properties to find an object.
        public virtual List<TestProperty> GetIdenProperties()
        {
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{Domain=" + this._domain);

            List<TestProperty> properties = GetIdenProperties();
            if (properties != null && properties.Count > 0)
            {
                foreach (TestProperty tp in properties)
                {
                    sb.Append("," + tp.Name + "=" + tp.Value);
                }
            }

            sb.Append("}");

            return sb.ToString();
        }

        #endregion

        #region actions


        #endregion

        #region parent and children

        #endregion

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }
}
