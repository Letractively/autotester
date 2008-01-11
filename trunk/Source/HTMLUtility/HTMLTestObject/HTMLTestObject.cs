/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestObject.cs
*
* Description: The base class of test object for HTML testing.
* 
* History: 2007/09/04 wan,yu Init version.
*          2008/01/11 wan,yu update, add IsReadOnly property and method. 
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    // HTMLTestObjectType defines the object type we used in HTML Testing.
    public enum HTMLTestObjectType
    {
        Button,
        CheckBox,
        RadioButton,
        TextBox,
        ComboBox,
        ListBox,
        Table,
        Image,
        Link,
        MsgBox,
        FileDialog,
        ActiveX,
        Unknow
    }

    #region html object base class
    public class HTMLTestObject : TestObject, IDisposable
    {
        #region fields

        protected string _tag;

        protected bool _visible;
        protected bool _enable;
        protected bool _readOnly;

        protected HTMLTestObjectType _type;
        protected IHTMLElement _sourceElement;

        protected AutoResetEvent _stateChanged = new AutoResetEvent(false);


        #endregion

        #region Properties
        public HTMLTestObjectType Type
        {
            get { return this._type; }
        }

        public string Tag
        {
            get { return this._tag; }
        }

        public bool Enable
        {
            get { return _enable; }
        }

        public bool Visible
        {
            get { return _visible; }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
        }

        #endregion

        #region methods

        #region ctor

        protected HTMLTestObject()
            : base()
        {
            this._domain = "HTML";
        }

        protected HTMLTestObject(IHTMLElement element)
            : base()
        {
            if (element == null)
            {
                throw new CannotBuildObjectException("Element is null.");
            }


            try
            {
                this._sourceElement = element;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not convert element to IHTMLElement: " + e.Message);
            }


            try
            {
                //get tag, like <A>, <Input>...
                this._domain = "HTML";
                this._tag = element.tagName;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not find tag name: " + e.Message);
            }


            try
            {
                //get id, like <input id="btn1">
                this._id = element.id;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("ID is not found of element " + element.ToString() + ": " + e.Message);
            }


            try
            {
                //get name, like <input name="btn2">
                if (element.getAttribute("name", 0) == null || element.getAttribute("name", 0).GetType().ToString() == "System.DBNull")
                {
                    this._name = "";
                }
                else
                {
                    this._name = element.getAttribute("name", 0).ToString().Trim();
                }
            }
            catch
            {
                this._name = "";
            }


            try
            {
                //check the "visibility" property.
                this._visible = IsVisible();
            }
            catch
            {
                this._visible = true;
            }


            try
            {
                //check the "enable" property
                this._enable = IsEnable();

            }
            catch
            {
                this._enable = true;
            }


            try
            {
                //check the "readonly" property
                this._readOnly = IsReadOnly();
            }
            catch
            {
                this._readOnly = false;
            }

        }

        ~HTMLTestObject()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (this._stateChanged != null)
            {
                this._stateChanged.Close();
                this._stateChanged = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region public methods

        #region action

        #endregion


        /* object GetPropertyByName(string propertyName)
         * return the property value by name.
         */
        public override object GetPropertyByName(string propertyName)
        {
            try
            {
                if (String.IsNullOrEmpty(propertyName))
                {
                    throw new PropertyNotFoundException("Property name can not be empty.");
                }

                propertyName = propertyName.Replace(" ", "");

                while (propertyName.StartsWith("."))
                {
                    propertyName = propertyName.Replace(".", "");
                }

                if (this._sourceElement.getAttribute(propertyName, 0) == null || this._sourceElement.getAttribute(propertyName, 0).GetType().ToString() == "System.DBNull")
                {
                    throw new PropertyNotFoundException("Property " + propertyName + " not found.");
                }
                else
                {
                    return this._sourceElement.getAttribute(propertyName, 0);
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PropertyNotFoundException("Property " + propertyName + " not found: " + e.Message);
            }
        }

        /* bool SetPropertyByName(string propertyName, object value)
         * set the property value.
         * return true if success.
         */
        public override bool SetPropertyByName(string propertyName, object value)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            propertyName = propertyName.Trim();

            //if the propertyName is start with ".", we will remove it.
            while (propertyName.StartsWith("."))
            {
                propertyName = propertyName.Replace(".", "");
            }

            try
            {
                this._sourceElement.setAttribute(propertyName, value, 0);

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region private methods

        /* bool IsVisible()
         * return true if it is a visible object.
         */
        public override bool IsVisible()
        {
            if (_sourceElement.getAttribute("visibility", 0) == null || _sourceElement.getAttribute("visibility", 0).GetType().ToString() == "System.DBNull")
            {
                return true;
            }
            else
            {
                string isVisiable = _sourceElement.getAttribute("visibility", 0).ToString().Trim();

                return String.Compare(isVisiable, "HIDDEN", true) == 0;
            }
        }

        /* bool IsEnable()
         * return true if it is a enable object.
         */
        public virtual bool IsEnable()
        {
            if (_sourceElement.getAttribute("enabled", 0) == null || _sourceElement.getAttribute("enabled", 0).GetType().ToString() == "System.DBNull")
            {
                return true;
            }
            else
            {
                string isEnable = _sourceElement.getAttribute("enabled", 0).ToString().Trim();

                return String.Compare(isEnable, "FALSE", true) == 0;
            }
        }

        /* bool IsReadOnly()
         * return true if it is a readonly object.
         */
        public virtual bool IsReadOnly()
        {
            if (_sourceElement.getAttribute("readonly", 0) == null || _sourceElement.getAttribute("readonly", 0).GetType().ToString() == "System.DBNull")
            {
                return false;
            }
            else
            {
                string isReadonly = _sourceElement.getAttribute("readonly", 0).ToString().Trim();

                return String.Compare(isReadonly, "TRUE", true) == 0;
            }
        }


        #endregion

        #endregion
    }
    #endregion
}
