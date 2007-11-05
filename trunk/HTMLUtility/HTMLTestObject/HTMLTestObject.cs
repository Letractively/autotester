using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

using mshtml;

using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Function;

namespace Shrinerain.AutoTester.HTMLUtility
{

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
    public class HTMLTestObject : TestObject
    {
        #region fields

        protected string _tag;
        protected bool _visible;
        protected bool _enable;
        protected HTMLTestObjectType _type;
        protected IHTMLElement _sourceElement;

        protected AutoResetEvent _stateChanged = new AutoResetEvent(false);


        #endregion

        #region Properties
        public HTMLTestObjectType Type
        {
            get { return this._type; }
            set
            {
                this._type = value;
            }
        }
        public string Tag
        {
            get { return this._tag; }
            set
            {
                this._tag = value;
            }
        }
        #endregion

        #region methods

        #region ctor

        protected HTMLTestObject()
            : base()
        {

        }

        protected HTMLTestObject(IHTMLElement element)
        {
            if (element == null)
            {
                throw new CanNotBuildObjectException("Element is null.");
            }
            try
            {
                this._sourceElement = element;
            }
            catch
            {
                throw new CanNotBuildObjectException("Can not build HTML object.");
            }
            try
            {
                this._domain = "HTML";
                this._tag = element.tagName;
            }
            catch
            {
                throw new CanNotBuildObjectException("Can not find tag name.");
            }
            try
            {
                this._id = element.id;
            }
            catch
            {
                throw new PropertyNotFoundException("Property [ID] not found of element " + element.ToString());
            }
            try
            {
                this._name = (string)element.getAttribute("name", 0);
            }
            catch
            {
                this._name = "";
            }
            try
            {
                string isVisiable = (string)element.getAttribute("visibility", 0);
                if (isVisiable.ToUpper() == "HIDDEN")
                {
                    this._visible = false;
                }
                else
                {
                    this._visible = true;
                }
            }
            catch
            {
                this._visible = true;
            }
            try
            {
                string isEnable = (string)element.getAttribute("enabled", 0);
                if (isEnable.ToUpper() == "FALSE")
                {
                    this._enable = false;
                }
                else
                {
                    this._enable = true;
                }
            }
            catch
            {
                this._enable = true;
            }
            try
            {
                this._properties = null;// BuildProperties(element);
            }
            catch
            {
                throw new PropertyNotFoundException("Can not get the properties of object: " + element.toString());
            }
            try
            {
                this._type = GetObjectType(element);
            }
            catch
            {
                throw new CanNotBuildObjectException();
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

        public override object GetPropertyByName(string propertyName)
        {
            try
            {
                propertyName = propertyName.Replace(".", "");
                return this._sourceElement.getAttribute(propertyName, 0);
            }
            catch
            {
                throw new PropertyNotFoundException("Property " + propertyName + " not found.");
            }
        }
        public override bool SetPropertyByName(string propertyName, object value)
        {
            try
            {
                propertyName = propertyName.Replace(".", "");
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
        public static HTMLTestObjectType GetObjectType(IHTMLElement element)
        {
            string tag = element.tagName;

            if (string.IsNullOrEmpty(tag))
            {
                return HTMLTestObjectType.Unknow;
            }
            else if (tag == "A")
            {
                return HTMLTestObjectType.Link;
            }
            else if (tag == "IMG")
            {
                string clickMethods;

                try
                {
                    clickMethods = element.getAttribute("onclick", 0).ToString().ToUpper();
                    if (String.IsNullOrEmpty(clickMethods))
                    {
                        return HTMLTestObjectType.Image;
                    }
                    else
                    {
                        return HTMLTestObjectType.Button;
                    }
                }
                catch (PropertyNotFoundException)
                {
                    return HTMLTestObjectType.Image;
                }
                catch
                {
                    throw;
                }

            }
            else if (tag == "INPUT")
            {
                string inputType = element.getAttribute("type", 0).ToString().ToUpper();
                if (inputType == "TEXT" || inputType == "PASSWORD")
                {
                    return HTMLTestObjectType.TextBox;
                }
                else if (inputType == "BUTTON" || inputType == "SUBMIT" || inputType == "RESET"
                      || inputType == "FILE" || inputType == "IMAGE")
                {
                    return HTMLTestObjectType.Button;
                }
                else if (inputType == "CHECKBOX")
                {
                    return HTMLTestObjectType.CheckBox;
                }
                else if (inputType == "RADIO")
                {
                    return HTMLTestObjectType.RadioButton;
                }
            }
            else if (tag == "TEXTAERA")
            {
                return HTMLTestObjectType.TextBox;
            }
            else if (tag == "TABLE")
            {
                return HTMLTestObjectType.Table;
            }
            else if (tag == "SELECT")
            {
                int selectSize = int.Parse(element.getAttribute("size", 0).ToString().ToUpper());
                if (selectSize < 2)
                {
                    return HTMLTestObjectType.ComboBox;
                }
                else
                {
                    return HTMLTestObjectType.ListBox;
                }
            }

            return HTMLTestObjectType.Unknow;

        }

        #endregion

        #endregion
    }
    #endregion
}
