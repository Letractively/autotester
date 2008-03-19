/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MSAATestObject.cs
*
* Description: This class define the base class of MSAA test object.
*
* History: 2008/03/19 wan,yu Init version.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;

namespace Shrinerain.AutoTester.MSAAUtility
{
    // MSAATestObjectType defines the object type we used in HTML Testing.
    public enum MSAATestObjectType
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
        Menu,
        Tab,
        ScrollBar
    }

    public class MSAATestObject : TestObject, IMSAA
    {

        #region fields

        //MSAA interface.
        protected IAccessible _iAcc;

        //MSAA property 
        protected String _defAction;
        protected String _description;
        protected String _name;
        protected IAccessible _parent;
        protected String _role;
        protected String _state;
        protected String _value;
        protected int _id;
        protected IntPtr _window;
        protected String _msaaObjType;
        protected object _selfID = 0;

        protected MSAATestObjectType _type;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public MSAATestObject(IAccessible iAcc)
            : base()
        {
            if (iAcc == null)
            {
                throw new CannotBuildObjectException("MSAA interface can not be null.");
            }

            try
            {
                this._domain = "MSAA";
                this._iAcc = iAcc;

                GetMSAAInfo();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.Message);
            }

        }

        public MSAATestObject(IAccessible parentAcc, int childID)
            : base()
        {
            if (parentAcc == null)
            {
                throw new CannotBuildObjectException("MSAA interface can not be null.");
            }

            if (childID < 0)
            {
                throw new CannotBuildObjectException("MSAA child id can not be < 0");
            }

            try
            {
                this._domain = "MSAA";
                this._selfID = childID;
                this._iAcc = parentAcc;

                GetMSAAInfo();

            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.Message);
            }

        }

        #endregion

        #region public methods

        #region IMSAA Members

        public IAccessible GetIAccInterface()
        {
            return this._iAcc;
        }

        public int GetSelfID()
        {
            return Convert.ToInt32(this._selfID);
        }

        #endregion

        /*  object GetProperty(string propertyName)
         *  get proerty of MSAA object.
         */
        public override object GetProperty(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new PropertyNotFoundException("Property name can not be null.");
            }

            if (propertyName.StartsWith("."))
            {
                propertyName.Replace(".", "");
            }

            try
            {
                if (String.Compare(propertyName, "name", true) == 0)
                {
                    return this._iAcc.get_accName(_selfID);
                }
                else if (String.Compare(propertyName, "value", true) == 0)
                {
                    return this._iAcc.get_accValue(_selfID);
                }
                else if (String.Compare(propertyName, "role", true) == 0)
                {
                    return this._iAcc.get_accRole(_selfID);
                }
                else if (String.Compare(propertyName, "state", true) == 0)
                {
                    return this._iAcc.get_accState(_selfID);
                }
                else if (String.Compare(propertyName, "defAction", true) == 0 || propertyName.IndexOf("action", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    return this._iAcc.get_accDefaultAction(_selfID);
                }
                else if (String.Compare(propertyName, "description", true) == 0)
                {
                    return this._iAcc.get_accDescription(_selfID);
                }
                else if (String.Compare(propertyName, "help", true) == 0)
                {
                    return this._iAcc.get_accHelp(_selfID);
                }
                else if (String.Compare(propertyName, "rect", true) == 0 || String.Compare(propertyName, "location", true) == 0)
                {
                    //get location.
                    int left, top, width, height;
                    this._iAcc.accLocation(out left, out top, out width, out height, _selfID);

                    return new Rectangle(left, top, width, height);
                }
                else if (String.Compare(propertyName, "childCount", true) == 0)
                {
                    return this._iAcc.accChildCount;
                }
                else if (String.Compare(propertyName, "selection", true) == 0)
                {
                    return this._iAcc.accSelection;
                }
                else if (String.Compare(propertyName, "focus", true) == 0)
                {
                    return this._iAcc.accFocus;
                }
                else
                {
                    throw new PropertyNotFoundException("Can not get property: " + propertyName);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get property [" + propertyName + "]: " + ex.Message);
            }
        }

        /* bool SetProperty(string propertyName, object value)
         * set property of MSAA object.
         * we can JUST set ".value" property for MSAA object.
         */
        public override bool SetProperty(string propertyName, object value)
        {
            try
            {
                this._iAcc.set_accValue(_selfID, value.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }


        #endregion

        #region private methods

        /* void GetMSAAInfo()
         * Get MSAA information.
         */
        protected virtual void GetMSAAInfo()
        {

            _name = this._iAcc.get_accName(_selfID);
            _defAction = this._iAcc.get_accDefaultAction(_selfID);
            _description = this._iAcc.get_accDescription(_selfID);

            //if the self id is 0, means the current MSAA interface is belong to this object, or belong to it's parent.
            //then try to find the parent. 
            if (Convert.ToInt32(_selfID) == 0)
            {
                _parent = (IAccessible)this._iAcc.accParent;
            }

            _role = this._iAcc.get_accRole(_selfID).ToString();
            _state = this._iAcc.get_accState(_selfID).ToString();
            _value = this._iAcc.get_accValue(_selfID).ToString();

        }

        #endregion

        #endregion

    }
}
