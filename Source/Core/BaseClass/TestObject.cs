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

using System;
using System.Collections.Generic;


namespace Shrinerain.AutoTester.Core
{
    public class TestObject
    {
        #region fields

        //domain means the object type, eg: HTML
        protected string _domain;

        protected bool _visible;

        // this hashtable is used to store properties for a test object.
        // key is the property name.
        protected Dictionary<string, object> _properties;

        #endregion

        #region properties

        public string Domain
        {
            get { return this._domain; }
        }

        #endregion

        #region public methods

        #region public method

        /*  object GetPropertyByName(string propertyName)
         *  get the expected property value.
         */
        public virtual object GetPropertyByName(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("Error: Property can not be null.");
            }
            object res = new object();
            if (!this._properties.TryGetValue(propertyName, out res))
            {
                throw new Exception("Error: Can not find the property: " + propertyName);
            }
            return res;
        }

        /* SetPropertyByName(string propertyName, object value)
         * set the expected property, return true if successful.
         */
        public virtual bool SetPropertyByName(string propertyName, object value)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("Error: Property can not be null.");
            }
            object res = new object();
            try
            {
                if (this._properties.TryGetValue(propertyName, out res))
                {
                    this._properties.Remove(propertyName);
                }
                this._properties.Add(propertyName, value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool IsVisible()
        {
            return this._visible;
        }

        /* void HightLight()
         * highlight the object, we can see a red rectangle around the object.
         */
        public virtual void HighLight()
        {

        }

        #endregion

        #region actions


        #endregion

        #region parent and children

        #endregion

        #endregion
    }
}
