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
using Shrinerain.AutoTester.Win32;

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
        protected int _childCount = 0;
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

        public String Value
        {
            get { return _value; }
        }

        public String State
        {
            get { return _state; }
        }

        public String Role
        {
            get { return _role; }
        }

        public int ChildCount
        {
            get { return _childCount; }
        }

        public IAccessible Parent
        {
            get { return _parent; }
        }

        public String Name
        {
            get { return _name; }
        }

        public String Description
        {
            get { return _description; }
        }

        public String DefaultAction
        {
            get { return _defAction; }
        }

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

        #region get msaa information

        public static String GetRole(IAccessible iAcc, int childID)
        {
            try
            {
                StringBuilder sb = new StringBuilder(64);
                Win32API.GetRoleText(Convert.ToUInt32(iAcc.get_accRole(childID)), sb, 64);
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetValue(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accValue(childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetName(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accName(childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetDefAction(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accDefaultAction(childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetDesc(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.get_accDescription(childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public static String GetState(IAccessible iAcc, int childID)
        {
            try
            {
                StringBuilder sb = new StringBuilder(64);
                Win32API.GetStateText(Convert.ToUInt32(iAcc.get_accState(childID)), sb, 64);
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        public static IAccessible GetParent(IAccessible iAcc, int childID)
        {
            if (childID > 0)
            {
                return iAcc;
            }
            else if (childID == 0)
            {
                return (IAccessible)iAcc.accParent;
            }
            else
            {
                return null;
            }
        }

        public static IAccessible GetChild(IAccessible iAcc, int childID)
        {
            if (childID > 0)
            {
                return (IAccessible)iAcc.get_accChild(childID);
            }
            else
            {
                return null;
            }
        }


        public static bool IsVisible(IAccessible iAcc, int childID)
        {
            string state = GetState(iAcc, childID);

            if (String.IsNullOrEmpty(state))
            {
                return true;
            }
            else
            {
                return state.IndexOf("invisible") < 0;
            }
        }

        public static int GetChildCount(IAccessible iAcc, int childID)
        {
            try
            {
                return iAcc.accChildCount;
            }
            catch
            {
                return 0;
            }
        }


        #endregion

        #endregion

        #region private methods

        /* void GetMSAAInfo()
         * Get MSAA information.
         */
        protected virtual void GetMSAAInfo()
        {

            try
            {
                _name = this._iAcc.get_accName(_selfID);
                _defAction = this._iAcc.get_accDefaultAction(_selfID);
                _description = this._iAcc.get_accDescription(_selfID);

                //if the self id is 0, means the current MSAA interface is belong to this object, or belong to it's parent.
                //then try to find the parent. 
                if (Convert.ToInt32(_selfID) == 0)
                {
                    _parent = (IAccessible)this._iAcc.accParent;
                    _childCount = this._iAcc.accChildCount;
                }

                _role = GetRole(this._iAcc, Convert.ToInt32(_selfID));
                _state = GetState(this._iAcc, Convert.ToInt32(_selfID));
                _value = this._iAcc.get_accValue(_selfID).ToString();
            }
            catch
            {
            }

        }

        #endregion

        #endregion

    }
}
