using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Function
{
    public class TestObject
    {
        #region fields

        protected string _name;
        protected string _id;
        protected string _domain;
        protected string _class;
        protected object _parent;

        protected bool _isVisible;
        protected bool _isInteractive;

        protected Dictionary<string, object> _properties;

        #endregion

        #region properties
        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
            }
        }
        public string ID
        {
            get { return this._id; }
            set
            {
                this._id = value;
            }
        }
        public string Domain
        {
            get { return this._domain; }
            set
            {
                this._domain = value;
            }
        }
        public object Parent
        {
            get { return _parent; }
            set
            {
                if (value != null)
                {
                    _parent = value;
                }
            }
        }

        #endregion

        #region public methods

        #region public method
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
            return this._isVisible;
        }

        public virtual bool IsInteractive()
        {
            return this._isInteractive;
        }

        public virtual void HightLight()
        {

        }

        #endregion

        #region actions


        #endregion

        #region parent and children

        public virtual TestObject GetParent()
        {
            return null;
        }
        #endregion

        #endregion
    }
}
