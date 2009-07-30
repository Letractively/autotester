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
    public class TestObject : IProperty
    {
        #region fields

        protected TestApp _parentApp;

        //domain means the object type, eg: Win32
        protected string _domain;
        protected string _type;

        protected List<String> _idenProperties = new List<String>();

        #endregion

        #region properties

        public string Domain
        {
            get { return this._domain; }
        }

        public String Type
        {
            get { return _type; }
        }

        public TestApp ParentApp
        {
            get { return _parentApp; }
            set { _parentApp = value; }
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

            SetIdenProperties();
        }

        #region public method

        /*  object GetProperty(string propertyName)
         *  get the expected property value.
         */
        public virtual object GetProperty(string propertyName)
        {
            return null;
        }

        public virtual bool HasProperty(string propertyName)
        {
            object val;
            return TryGetProperty(propertyName, out val);
        }

        public virtual bool TryGetProperty(string propertyName, out object value)
        {
            try
            {
                value = GetProperty(propertyName);
                return value != null;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        /* bool SetProperty(string propertyName, object value)
         * set the expected property, return true if successful.
         */
        public virtual bool SetProperty(string propertyName, object value)
        {
            return false;
        }

        //identification properties is used to identify a test object.
        //you can find an object by these properties.
        //when recoding, test object will return these properties.
        protected virtual void SetIdenProperties()
        {
            _idenProperties.Add(TestConstants.PROPERTY_DOMAIN);
        }

        public virtual void SetIdenProperties(String[] idenProperties)
        {
            if (idenProperties != null && idenProperties.Length > 0)
            {
                for (int i = 0; i < idenProperties.Length; i++)
                {
                    string tpName = idenProperties[i].ToUpper();
                    if (!_idenProperties.Contains(tpName))
                    {
                        _idenProperties.Add(tpName);
                    }
                }
            }
        }

        //these properties is used to identify an object.
        //we will record these properties, and when playing back, use these properties to find an object.
        public virtual List<TestProperty> GetIdenProperties()
        {
            List<TestProperty> properties = new List<TestProperty>();
            if (_idenProperties.Count > 0)
            {
                foreach (String tpName in _idenProperties)
                {
                    if (tpName == TestConstants.PROPERTY_DOMAIN)
                    {
                        properties.Add(new TestProperty(TestConstants.PROPERTY_DOMAIN, _domain));
                    }
                    else
                    {
                        object tpValue;
                        if (TryGetProperty(tpName, out tpValue))
                        {
                            properties.Add(new TestProperty(tpName, tpValue.ToString()));
                        }
                    }
                }
            }
            return properties;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            List<TestProperty> properties = GetIdenProperties();
            if (properties != null && properties.Count > 0)
            {
                foreach (TestProperty tp in properties)
                {
                    sb.Append(tp.Name + "=" + tp.Value + TestProperty.PropertySeparator);
                }
            }

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
