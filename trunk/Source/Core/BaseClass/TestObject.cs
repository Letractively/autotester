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
    public class TestObject
    {
        #region fields

        //domain means the object type, eg: HTML
        protected string _domain;

        #endregion

        #region properties

        public string Domain
        {
            get { return this._domain; }
        }

        #endregion

        #region public methods

        #region public method

        /*  object GetProperty(string propertyName)
         *  get the expected property value.
         */
        public virtual object GetProperty(string propertyName)
        {
            throw new PropertyNotFoundException("Can not find the property: " + propertyName);
        }

        /* bool SetProperty(string propertyName, object value)
         * set the expected property, return true if successful.
         */
        public virtual bool SetProperty(string propertyName, object value)
        {
            throw new PropertyNotFoundException("Can not find the property: " + propertyName);
        }

        #endregion

        #region actions


        #endregion

        #region parent and children

        #endregion

        #endregion
    }
}
