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
using System.Collections.Generic;
using System.Threading;
using System.Web;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    // HTMLTestObjectType defines the object type we used in HTML Testing.
    public enum HTMLTestObjectType
    {
        Unknow = 0,
        Any,
        Label,
        Button,
        CheckBox,
        RadioBox,
        TextBox,
        ComboBox,
        ListBox,
        Table,
        Image,
        Link,
        ActiveX
    }

    #region html object base class
    public class HTMLTestObject : TestObject, IStatus
    {
        #region fields

        //id and name for a HTML object, they are the most important property.
        protected string _id;
        protected string _name;

        //tag for this object, like "A" ,"INPUT"...
        protected string _tag;
        protected HTMLTestObjectType _type;
        protected IHTMLElement _sourceElement;

        //the host browser of this object.
        protected HTMLTestBrowser _browser;

        #endregion

        #region Properties

        public IHTMLElement HTMLElement
        {
            get { return _sourceElement; }
        }

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

        public HTMLTestBrowser Browser
        {
            get { return _browser; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestObject()
            : base()
        {
            this._domain = "HTML";
        }

        public HTMLTestObject(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestObject(IHTMLElement element, HTMLTestBrowser browser)
            : base()
        {
            if (element == null)
            {
                throw new CannotBuildObjectException("Element can not be null.");
            }

            try
            {
                this._sourceElement = element;
                //get tag, like <A>, <Input>...
                this._domain = "HTML";
                this._tag = element.tagName;
                this._id = element.id;
                //get name, like <input name="btn2">
                if (!TryGetProperty(element, "name", out this._name))
                {
                    this._name = "";
                }

                this._browser = browser;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build HTML test object: " + ex.ToString());
            }
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

        public override bool TryGetProperty(string propertyName, out object value)
        {
            value = null;
            string strValue = null;
            if (TryGetProperty(this._sourceElement, propertyName, out strValue))
            {
                value = strValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool HasProperty(string propertyName)
        {
            return HasProperty(this._sourceElement, propertyName);
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
        public static bool HasProperty(IHTMLElement element, string properyName)
        {
            string value;
            return TryGetProperty(element, properyName, out value);
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
                else if (String.Compare(propertyName, "tag", true) == 0 || String.Compare(propertyName, "tagname", true) == 0)
                {
                    value = element.tagName;
                }
                else
                {
                    //not common properties
                    object valueObj = element.getAttribute(propertyName, 0);
                    value = (valueObj == null ? "" : valueObj.ToString());
                }

                if (value == null)
                {
                    value = "";
                }
                else
                {
                    value = (value.Trim() == "" ? value : value.Trim());
                    value = HttpUtility.HtmlDecode(value);
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

        public override List<TestProperty> GetIdenProperties()
        {
            try
            {
                List<TestProperty> properties = new List<TestProperty>();
                properties.Add(new TestProperty(TestConstants.PROPERTY_DOMAIN, TestConstants.PROPERTY_HTML));
                properties.Add(new TestProperty(TestConstants.PROPERTY_ID, _id));
                properties.Add(new TestProperty(TestConstants.PROPERTY_NAME, _name));
                properties.Add(new TestProperty(TestConstants.PROPERTY_TAG, _tag));
                properties.Add(new TestProperty(TestConstants.PROPERTY_TYPE, _type.ToString()));
                string outerHTML;
                if (!TryGetProperty(this._sourceElement, "outerHTML", out outerHTML))
                {
                    outerHTML = "NO HTML";
                }
                else if (outerHTML.Length > 32)
                {
                    outerHTML = outerHTML.Substring(0, 32) + "...";
                }
                properties.Add(new TestProperty(TestConstants.PROPERTY_OUTERHTML, outerHTML));

                return properties;
            }
            catch
            {
            }

            return null;
        }

        #region IStatus Members

        /* object GetCurrentStatus()
         * get the readystate of element. 
         */
        public virtual object GetCurrentStatus()
        {
            try
            {
                if (_sourceElement != null)
                {
                    return ((IHTMLElement2)_sourceElement).readyState;
                }
                else
                {
                    throw new CannotPerformActionException("Can not get status: Element can not be null.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not get status: " + ex.ToString());
            }
        }

        /* bool IsReady()
         * return true if the object is ready.
         */
        public virtual bool IsReady()
        {
            try
            {
                if (_sourceElement != null)
                {
                    string readyS = ((IHTMLElement2)_sourceElement).readyState.ToString();
                    return readyS == null || readyS == "interactive" || readyS == "complete";
                }
                else
                {
                    throw new CannotPerformActionException("Can not get ready status: InputElement can not be null.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not get ready status: " + ex.ToString());
            }
        }

        #endregion

        #endregion

        #region private methods

        //fire correct event after action.
        protected virtual void FireEvent(IHTMLElement3 element, string eventName)
        {
            if (element != null && !String.IsNullOrEmpty(eventName))
            {
                try
                {
                    object eventObject = null;
                    (this._sourceElement.document as IHTMLDocument4).CreateEventObject(ref eventObject);
                    element.FireEvent(eventName, ref eventObject);
                }
                catch
                {
                }
            }
        }

        #endregion

        #endregion
    }
    #endregion
}
