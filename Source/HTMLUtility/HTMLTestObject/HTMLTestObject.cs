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
using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;

namespace Shrinerain.AutoTester.HTMLUtility
{
    #region html object base class
    public class HTMLTestObject : TestObject, IStatus, IHierarchy, IMSAA
    {
        #region fields

        public const string DOMAIN = HTMLTestConstants.DOMAIN_HTML;

        //tag for this object, like "A" ,"INPUT"...
        protected string _tag;
        protected string _id;
        protected string _name;
        protected string _className;
        protected string _innerText;
        protected string _innerHTML;
        protected string _outterText;
        protected string _outterHTML;

        protected IHTMLElement _sourceElement;

        protected IAccessible _pAcc;

        protected static Dictionary<string, bool> _commonProperties = new Dictionary<string, bool>(17);

        #endregion

        #region Properties

        public IHTMLElement HTMLElement
        {
            get { return _sourceElement; }
        }

        public string Tag
        {
            get { return _tag; }
        }

        public string ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string InnerHTML
        {
            get { return _innerHTML; }
        }

        public string InnerText
        {
            get { return _innerText; }
        }

        public string OutterText
        {
            get { return _outterText; }
        }

        public string OutterHTML
        {
            get { return _outterHTML; }
        }

        public string ClassName
        {
            get { return _className; }
        }

        public new HTMLTestPage ParentPage
        {
            get { return _parentPage as HTMLTestPage; }
        }

        public IHTMLDocument Document
        {
            get { return _sourceElement.document as IHTMLDocument; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestObject()
            : base()
        {
            this._domain = DOMAIN;
        }

        public HTMLTestObject(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestObject(IHTMLElement element, HTMLTestPage page)
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
                this._domain = DOMAIN;
                this._tag = element.tagName;
                this._id = element.id;
                this._className = element.className;
                this._innerText = element.innerText;
                this._innerHTML = element.innerHTML;
                this._outterText = element.outerText;
                this._outterHTML = element.outerHTML;

                //get name, like <input name="btn2">
                if (!TryGetProperty(element, "name", out this._name))
                {
                    this._name = "";
                }

                this._parentPage = page;
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
            if (String.Compare(propertyName, HTMLTestConstants.PROPERTY_ID, true) == 0)
            {
                if (!String.IsNullOrEmpty(_id))
                {
                    value = _id;
                    return true;
                }
            }
            else if (String.Compare(propertyName, HTMLTestConstants.PROPERTY_NAME, true) == 0)
            {
                if (!String.IsNullOrEmpty(_name))
                {
                    value = _name;
                    return true;
                }
            }
            else if (String.Compare(propertyName, HTMLTestConstants.PROPERTY_TAG, true) == 0)
            {
                if (!String.IsNullOrEmpty(_tag))
                {
                    value = _tag;
                    return true;
                }
            }
            else if (String.Compare(propertyName, HTMLTestConstants.PROPERTY_TYPE, true) == 0)
            {
                if (!String.IsNullOrEmpty(_type.ToString()))
                {
                    value = _type.ToString();
                    return true;
                }
            }

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

        protected override void SetIdenProperties()
        {
            base.SetIdenProperties();

            this._idenProperties.Add(HTMLTestConstants.PROPERTY_TYPE);
            this._idenProperties.Add(HTMLTestConstants.PROPERTY_TAG);
            this._idenProperties.Add(HTMLTestConstants.PROPERTY_ID);
            this._idenProperties.Add(HTMLTestConstants.PROPERTY_NAME);
            this._idenProperties.Add(HTMLTestConstants.PROPERTY_CLASS);
            this._idenProperties.Add(HTMLTestConstants.PROPERTY_OUTERHTML);
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
            propertyName = propertyName.Trim().Replace(".", "").ToUpper();

            try
            {
                bool found = false;
                //check some common properties, to improve performance.
                if (IsCommonProperties(propertyName))
                {
                    acceptEmpty = true;
                    if (String.Compare(propertyName, "TAG") == 0 || String.Compare(propertyName, "TAGNAME") == 0)
                    {
                        found = true;
                        value = element.tagName;
                    }
                    else if (String.Compare(propertyName, "CLASS") == 0 || String.Compare(propertyName, "CLASSNAME") == 0)
                    {
                        found = true;
                        value = element.className;
                    }
                }

                if (!found)
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

        #region IContainer Members

        public virtual object GetParent()
        {
            if (this._sourceElement != null)
            {
                try
                {
                    IHTMLElement ele = this._sourceElement.parentElement;
                    HTMLTestObject obj = HTMLTestObjectFactory.BuildHTMLTestObject(ele, this._parentPage as HTMLTestPage);
                    return obj;
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ObjectNotFoundException("Can not get parent: " + ex.ToString());
                }
            }

            return null;
        }

        public virtual int GetChildCount()
        {
            if (this._sourceElement != null)
            {
                try
                {
                    IHTMLElementCollection childrenElements = this._sourceElement.children as IHTMLElementCollection;
                    return childrenElements == null ? 0 : childrenElements.length;
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotGetChildrenException(ex.ToString());
                }
            }

            return 0;
        }

        public virtual object[] GetChildren()
        {
            if (this._sourceElement != null)
            {
                try
                {
                    IHTMLElementCollection childrenElements = this._sourceElement.children as IHTMLElementCollection;
                    if (childrenElements != null && childrenElements.length > 0)
                    {
                        List<HTMLTestObject> children = new List<HTMLTestObject>(childrenElements.length);
                        for (int i = 0; i < childrenElements.length; i++)
                        {
                            try
                            {
                                IHTMLElement ele = childrenElements.item(i, i) as IHTMLElement;
                                HTMLTestGUIObject obj = HTMLTestObjectFactory.BuildHTMLTestObject(ele, this._parentPage as HTMLTestPage);
                                if (obj != null)
                                {
                                    children.Add(obj);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        return children.ToArray();
                    }
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotGetChildrenException(ex.ToString());
                }
            }

            return null;
        }

        public virtual object GetChild(int childIndex)
        {
            if (this._sourceElement != null)
            {
                try
                {
                    IHTMLElementCollection childrenElements = this._sourceElement.children as IHTMLElementCollection;
                    if (childrenElements != null && childIndex < childrenElements.length)
                    {
                        IHTMLElement ele = childrenElements.item(childIndex, childIndex) as IHTMLElement;
                        return HTMLTestObjectFactory.BuildHTMLTestObject(ele, this._parentPage as HTMLTestPage);
                    }
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotGetChildrenException(ex.ToString());
                }
            }

            return null;
        }

        #endregion

        #region IMSAA Members

        public virtual IAccessible GetIAccInterface()
        {
            if (this._pAcc == null)
            {
                _pAcc = COMUtil.IHTMLElementToMSAA(this._sourceElement);
            }

            return _pAcc;
        }

        public virtual int GetChildID()
        {
            return 0;
        }

        #endregion

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
                    throw new CannotPerformActionException("Can not get ready status: element can not be null.");
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
        protected virtual void FireEvent(string eventName)
        {
            FireEvent(this._sourceElement, eventName);
        }

        //fire correct event after action.
        protected static void FireEvent(IHTMLElement element, string eventName)
        {
            if (element != null && !String.IsNullOrEmpty(eventName))
            {
                try
                {
                    object eventObject = null;
                    (element.document as IHTMLDocument4).CreateEventObject(ref eventObject);
                    (element as IHTMLElement3).FireEvent(eventName, ref eventObject);
                }
                catch
                {
                }
            }
        }

        protected static bool IsCommonProperties(string propertyName)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                if (_commonProperties.Count == 0)
                {
                    _commonProperties.Add("ID", true);
                    _commonProperties.Add("NAME", true);
                    _commonProperties.Add("INNERTEXT", true);
                    _commonProperties.Add("INNERHTML", true);
                    _commonProperties.Add("OUTERHTML", true);
                    _commonProperties.Add("OUTERTEXT", true);
                    _commonProperties.Add("TAG", true);
                    _commonProperties.Add("TAGNAME", true);
                    _commonProperties.Add("CLASSNAME", true);
                    _commonProperties.Add("CLASS", true);
                }

                return _commonProperties.ContainsKey(propertyName);
            }

            return false;
        }

        #endregion

        #endregion

    }
    #endregion
}
