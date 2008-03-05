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
*          2008/01/15 wan,yu update, add _browser field. 
*          2008/01/21 wan,yu update, add TryGetValueByProperty and 
*                                    TrySetValueByProperty. 
*          2008/01/21 wan,yu update, add _htmlObjPool field.
*
*********************************************************************/


using System;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    // HTMLTestObjectType defines the object type we used in HTML Testing.
    public enum HTMLTestObjectType
    {
        Unknow = 0,
        Label,
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
        ActiveX
    }

    #region html object base class
    public class HTMLTestObject : TestObject, IDisposable
    {
        #region fields

        //id and name for a HTML object, they are the most important property.
        protected string _id;
        protected string _name;

        //tag for this object, like "A" ,"INPUT"...
        protected string _tag;

        protected bool _isVisible;
        protected bool _isEnable;
        protected bool _isReadOnly;

        protected HTMLTestObjectType _type;
        protected IHTMLElement _sourceElement;

        //the host browser of this object.
        protected HTMLTestBrowser _browser;

        //object pool of this object.
        protected HTMLTestObjectPool _htmlObjPool;

        protected AutoResetEvent _stateChanged = new AutoResetEvent(false);

        #endregion

        #region Properties

        public string ID
        {
            get
            {
                return this._id;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        public HTMLTestObjectType Type
        {
            get { return this._type; }
        }

        public string Tag
        {
            get { return this._tag; }
        }

        public bool IsEnable
        {
            get { return _isEnable; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
        }

        public virtual HTMLTestBrowser Browser
        {
            get { return _browser; }
            set { _browser = value; }
        }

        public virtual HTMLTestObjectPool HtmlObjPool
        {
            get { return _htmlObjPool; }
            set { _htmlObjPool = value; }
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
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not convert element to IHTMLElement: " + ex.Message);
            }

            try
            {
                //get tag, like <A>, <Input>...
                this._domain = "HTML";
                this._tag = element.tagName;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not find tag name: " + ex.Message);
            }

            try
            {
                //get id, like <input id="btn1">
                this._id = element.id;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("ID is not found of element " + element.ToString() + ": " + ex.Message);
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
                this._isVisible = Visible();
            }
            catch
            {
                this._isVisible = true;
            }

            try
            {
                //check the "enable" property
                this._isEnable = Enable();

            }
            catch
            {
                this._isEnable = true;
            }

            try
            {
                //check the "readonly" property
                this._isReadOnly = ReadOnly();
            }
            catch
            {
                this._isReadOnly = false;
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
                //this._stateChanged.Close();
                //this._stateChanged = null;
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
        public override object GetProperty(string propertyName)
        {
            string value;

            if (TryGetProperty(this._sourceElement, propertyName, out value))
            {
                return value;
            }
            else
            {
                throw new PropertyNotFoundException("Property " + propertyName + " not found.");
            }

        }

        /* bool SetPropertyByName(string propertyName, object value)
         * set the property value.
         * return true if success.
         */
        public override bool SetProperty(string propertyName, object value)
        {
            return TrySetProperty(this._sourceElement, propertyName, value);
        }

        /* bool TryGetValueByProperty(IHTMLElement element, string properyName)
         * return true if the property value is not null or empty.
         */
        public static bool TryGetProperty(IHTMLElement element, string properyName)
        {
            string value;

            return TryGetProperty(element, properyName, out value);
        }

        public static bool TryGetProperty(object element, string propertyName, out string value)
        {
            value = null;

            if (element == null)
            {
                return false;
            }

            try
            {
                IHTMLElement tmpElement = (IHTMLElement)element;

                return TryGetProperty(tmpElement, propertyName, out value);
            }
            catch
            {
                return false;
            }

        }


        /* bool TryGetValueByProperty(IHTMLElement element, string propertyName, out object value)
         * return true if the property exist.
         */
        public static bool TryGetProperty(IHTMLElement element, string propertyName, out string value)
        {
            return TryGetProperty(element, propertyName, out value, false);
        }

        public static bool TryGetProperty(IHTMLElement element, string propertyName, out string value, bool acceptEmpty)
        {
            value = "";

            if (element == null || string.IsNullOrEmpty(propertyName.Trim()))
            {
                return false;
            }

            //property name not null
            propertyName = propertyName.Trim().Replace(".", "");

            try
            {
                //check some common properties, to improve performance.
                if (String.Compare(propertyName, "innerText", true) == 0)
                {
                    value = element.innerText;
                }
                else if (String.Compare(propertyName, "innerHTML", true) == 0)
                {
                    value = element.innerHTML;
                }
                else if (String.Compare(propertyName, "outerHTML", true) == 0)
                {
                    value = element.outerHTML;
                }
                else if (String.Compare(propertyName, "outerText", true) == 0)
                {
                    value = element.outerText;
                }
                else if (element.getAttribute(propertyName, 0) == null || element.getAttribute(propertyName, 0).GetType().ToString() == "System.DBNull")
                {
                    return false;
                }
                else
                {
                    //not common properties
                    value = element.getAttribute(propertyName, 0).ToString();
                }

                if (value == null)
                {
                    value = "";
                    return false;
                }
                else
                {
                    //if not blank string, trim.
                    if (!String.IsNullOrEmpty(value.Trim()))
                    {
                        value = value.Trim();
                    }
                }

                //if acceptEmpty is true, we will return true.
                //if acceptEnpty is false, then we will check the value, it it is empty, we will return false.
                return acceptEmpty || value != "";
            }
            catch
            {
                return false;
            }

        }

        /* bool TrySetValueByProperty(IHTMLElement element, string propertyName, object value)
         * return true if we set the property value successfully.
         */
        public static bool TrySetProperty(IHTMLElement element, string propertyName, object value)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return false;
            }

            propertyName = propertyName.Trim().Replace(".", "");

            try
            {
                element.setAttribute(propertyName, value, 0);

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
        protected virtual bool Visible()
        {
            string isVisible;

            if (this._sourceElement.offsetWidth < 1 || this._sourceElement.offsetHeight < 1)
            {
                return false;
            }

            if (!TryGetProperty(this._sourceElement, "visibility", out isVisible))
            {
                return true && IsDisplayed(this._sourceElement);
            }
            else
            {
                return String.Compare(isVisible, "HIDDEN", true) == 0;
            }
        }

        /* bool IsEnable()
         * return true if it is a enable object.
         */
        protected virtual bool Enable()
        {
            string isEnable;

            if (!TryGetProperty(this._sourceElement, "diabled", out isEnable))
            {
                return true;
            }
            else
            {
                return !(String.Compare(isEnable, "true", true) == 0);
            }
        }

        /* bool IsReadOnly()
         * return true if it is a readonly object.
         */
        protected virtual bool ReadOnly()
        {
            if (!Enable())
            {
                return true;
            }

            string isReadOnly;

            if (!TryGetProperty(this._sourceElement, "readOnly", out isReadOnly))
            {
                return false;
            }
            else
            {
                return String.Compare(isReadOnly, "TRUE", true) == 0;
            }
        }

        /* bool IsDisplay(IHTMLElement element)
        * Check the style
        */
        protected virtual bool IsDisplayed(IHTMLElement element)
        {
            if (element == null)
            {
                return false;
            }

            string isDisabled;
            if (HTMLTestObject.TryGetProperty(element, "disabled", out isDisabled))
            {
                if (String.Compare("true", isDisabled, true) == 0)
                {
                    return false;
                }
            }

            string isDisplayed;
            if (HTMLTestObject.TryGetProperty(element, "style", out isDisplayed))
            {
                isDisplayed = isDisplayed.Replace(" ", "");
                if (isDisplayed.IndexOf("display:none", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #endregion
    }
    #endregion
}
