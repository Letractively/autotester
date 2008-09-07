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
        Any,
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
        Tree,
        ScrollBar
    }

    public class MSAATestObject : TestObject, IMSAA, IWindows
    {

        #region fields

        //MSAA interface.
        protected IAccessible _iAcc;
        protected int _selfID = 0;

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

        //windows info.
        protected String _class;
        protected String _caption;
        protected IntPtr _windowHandle;

        protected String _msaaObjType;
        protected MSAATestObjectType _type;

        #endregion

        #region properties

        public IAccessible IAcc
        {
            get { return _iAcc; }
        }

        public int SelfID
        {
            get { return _selfID; }
        }

        public String DefAction
        {
            get { return _defAction; }
        }

        public String Description
        {
            get { return _description; }
        }

        public String Name
        {
            get { return _name; }
        }

        public IAccessible Parent
        {
            get { return _parent; }
        }

        public int ChildCount
        {
            get { return _childCount; }
        }

        public String Role
        {
            get { return _role; }
        }

        public String State
        {
            get { return _state; }
        }

        public String Value
        {
            get { return _value; }
        }

        public MSAATestObjectType Type
        {
            get { return _type; }
        }

        public String WindowsClass
        {
            get { return _class; }
        }

        public IntPtr WindowHandle
        {
            get { return GetHandle(); }
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

        public MSAATestObject(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Handle can not be 0.");
            }

            try
            {
                this._domain = "MSAA";
                this._selfID = 0;
                this._windowHandle = handle;

                Win32API.AccessibleObjectFromWindow(handle, (int)Win32API.IACC.OBJID_CLIENT, ref Win32API.IACCUID, ref this._iAcc);

                if (this._iAcc != null)
                {
                    GetMSAAInfo();
                }
                else
                {
                    throw new CannotBuildObjectException("Can not get MSAA interface from handle: " + handle);
                }
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build object from handle: " + ex.Message);
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
            return this._selfID;
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
                    return this._iAcc.get_accName((object)_selfID);
                }
                else if (String.Compare(propertyName, "value", true) == 0)
                {
                    return this._iAcc.get_accValue((object)_selfID);
                }
                else if (String.Compare(propertyName, "role", true) == 0)
                {
                    return this._iAcc.get_accRole((object)_selfID);
                }
                else if (String.Compare(propertyName, "state", true) == 0)
                {
                    return this._iAcc.get_accState((object)_selfID);
                }
                else if (String.Compare(propertyName, "defAction", true) == 0 || propertyName.IndexOf("action", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    return this._iAcc.get_accDefaultAction((object)_selfID);
                }
                else if (String.Compare(propertyName, "description", true) == 0)
                {
                    return this._iAcc.get_accDescription((object)_selfID);
                }
                else if (String.Compare(propertyName, "help", true) == 0)
                {
                    return this._iAcc.get_accHelp((object)_selfID);
                }
                else if (String.Compare(propertyName, "rect", true) == 0 || String.Compare(propertyName, "location", true) == 0)
                {
                    return GetRect();
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
                this._iAcc.set_accValue((object)_selfID, value.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region get msaa information

        public String GetRole()
        {
            return GetRole(this._iAcc, _selfID);
        }

        public static String GetRole(IAccessible iAcc, int childID)
        {
            if (iAcc != null && childID >= 0)
            {
                int childCount = iAcc.accChildCount;
                if (childID <= childCount)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder(64);
                        Win32API.GetRoleText(Convert.ToUInt32(iAcc.get_accRole(childID)), sb, 64);
                        return sb.ToString();
                    }
                    catch
                    {
                    }
                }
            }

            return "";
        }

        public String GetValue()
        {
            return GetValue(this._iAcc, _selfID);
        }

        public static String GetValue(IAccessible iAcc, int childID)
        {
            if (iAcc != null && childID >= 0)
            {
                int childCount = iAcc.accChildCount;
                if (childID <= childCount)
                {
                    try
                    {
                        return iAcc.get_accValue((object)childID).ToString();
                    }
                    catch
                    {
                    }
                }
            }

            return "";
        }

        public String GetName()
        {
            return GetName(_iAcc, _selfID);
        }

        public static String GetName(IAccessible iAcc, int childID)
        {
            if (iAcc != null && childID >= 0)
            {
                int childCount = iAcc.accChildCount;
                if (childID <= childCount)
                {
                    try
                    {
                        return iAcc.get_accName((object)childID).ToString();
                    }
                    catch
                    {
                    }
                }
            }

            return "";
        }

        public String GetDefAction()
        {
            return GetDefAction(_iAcc, _selfID);
        }

        public static String GetDefAction(IAccessible iAcc, int childID)
        {
            if (iAcc != null && childID >= 0)
            {
                int childCount = iAcc.accChildCount;
                if (childID <= childCount)
                {
                    try
                    {
                        return iAcc.get_accDefaultAction((object)childID).ToString();
                    }
                    catch
                    {
                    }
                }
            }

            return "";
        }

        public String GetDesc()
        {
            return GetDesc(_iAcc, _selfID);
        }

        public static String GetDesc(IAccessible iAcc, int childID)
        {
            if (iAcc != null && childID >= 0)
            {
                int childCount = iAcc.accChildCount;
                if (childID <= childCount)
                {
                    try
                    {
                        return iAcc.get_accDescription((object)childID).ToString();
                    }
                    catch
                    {
                    }
                }
            }

            return "";
        }

        public String GetState()
        {
            return GetState(_iAcc, _selfID);
        }

        public static String GetState(IAccessible iAcc, int childID)
        {
            if (iAcc != null && childID >= 0)
            {
                int childCount = iAcc.accChildCount;
                if (childID <= childCount)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder(32);
                        Win32API.GetStateText(Convert.ToUInt32(iAcc.get_accState(childID)), sb, 32);
                        return sb.ToString();
                    }
                    catch
                    {
                    }
                }
            }

            return "";

        }

        public IAccessible GetParent()
        {
            return GetParent(_iAcc, _selfID);
        }

        public static IAccessible GetParent(IAccessible iAcc, int childID)
        {
            if (iAcc != null)
            {
                if (childID > 0)
                {
                    return iAcc;
                }
                else if (childID == 0)
                {
                    try
                    {
                        return (IAccessible)iAcc.accParent;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        public virtual Object[] GetChildren()
        {
            try
            {
                int c = GetChildCount();
                if (c > 0)
                {
                    Object[] children = new Object[c];
                    for (int i = 0; i < c; i++)
                    {
                        children[i] = GetChild(i);
                    }

                    return children;
                }

                return null;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not get MSAA children: " + ex.Message);
            }


        }

        /*  MSAATestObject GetChild(int childID)
         *  return the child MSAA test object by index.
         *  the childID start from 0.
         */
        public virtual Object GetChild(int childID)
        {
            try
            {
                if (childID >= 0 && this._iAcc != null && this._selfID == 0)
                {
                    IAccessible childIAcc = GetChildIAcc(childID);

                    if (childIAcc != null)
                    {
                        return new MSAATestObject(childIAcc);
                    }
                    else
                    {
                        return new MSAATestObject(this._iAcc, childID + 1);
                    }
                }

                throw new ObjectNotFoundException("Can not find MSAA child by index: " + childID);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not find MSAA child: " + ex.Message);
            }
        }

        public IAccessible GetChildIAcc(int childID)
        {
            return GetChildIAcc(_iAcc, childID);
        }

        public static IAccessible GetChildIAcc(IAccessible iAcc, int childID)
        {
            if (childID >= 0 && iAcc != null)
            {
                int childCount = iAcc.accChildCount;

                if (childID < childCount)
                {
                    int newChildID = childID + 1;

                    IAccessible childIAcc = null;

                    try
                    {
                        object childIDObj = newChildID;

                        childIDObj = iAcc.get_accChild(childIDObj);

                        if (childIDObj is IAccessible)
                        {
                            childIAcc = (IAccessible)childIDObj;
                        }
                    }
                    catch
                    {
                    }

                    if (childIAcc == null)
                    {
                        try
                        {
                            object[] childrenObj = new object[1];
                            int count = 0;
                            Win32API.AccessibleChildren(iAcc, childID, 1, childrenObj, out count);

                            if (count == 1)
                            {
                                childIAcc = (IAccessible)childrenObj[0];
                            }
                        }
                        catch
                        {
                        }
                    }

                    return childIAcc;
                }
            }

            return null;
        }

        public int GetChildCount()
        {
            return GetChildCount(_iAcc, _selfID);
        }

        public static int GetChildCount(IAccessible iAcc, int childID)
        {
            try
            {
                return childID == 0 ? iAcc.accChildCount : 0;
            }
            catch
            {
                return 0;
            }
        }

        public Rectangle GetRect()
        {
            return GetRect(_iAcc, _selfID);
        }

        public static Rectangle GetRect(IAccessible iAcc, int childID)
        {
            try
            {
                //get location.
                int left, top, width, height;
                iAcc.accLocation(out left, out top, out width, out height, (object)childID);

                return new Rectangle(left, top, width, height);
            }
            catch
            {
                return new Rectangle(0, 0, 0, 0);
            }
        }

        #endregion

        #region IWindows Members

        public IntPtr GetHandle()
        {
            if (_windowHandle == IntPtr.Zero)
            {
                _windowHandle = GetWindowsHandle(this._iAcc, this._selfID);
            }

            return _windowHandle;
        }

        public string GetClass()
        {
            try
            {
                if (this._windowHandle != IntPtr.Zero)
                {
                    StringBuilder sb = new StringBuilder(128);
                    Win32API.GetClassName(_windowHandle, sb, 128);

                    return sb.ToString();
                }

                return "";
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get windows class name: " + ex.Message);
            }
        }

        public String GetCaption()
        {
            try
            {
                if (this._windowHandle != IntPtr.Zero)
                {
                    StringBuilder sb = new StringBuilder(128);
                    Win32API.GetWindowText(_windowHandle, sb, 128);

                    return sb.ToString();
                }

                return "";
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get windows caption: " + ex.Message);
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

                _role = GetRole();
                _state = GetState();
                _value = GetValue();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get MSAA property: " + ex.Message);
            }

        }

        private static IntPtr GetWindowsHandle(IAccessible iAcc, int childID)
        {
            try
            {
                Rectangle rect = GetRect(iAcc, childID);

                if (rect.Width > 0 && rect.Height > 0)
                {
                    int x = rect.Left + rect.Width / 2;
                    int y = rect.Top + rect.Height / 2;

                    return Win32API.WindowFromPoint(x, y);
                }
            }
            catch
            {
            }

            return IntPtr.Zero;
        }

        #endregion

        #endregion
    }
}
