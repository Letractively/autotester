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

using mshtml;
using Accessibility;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public partial class MSAATestObject : TestObject, IMSAA, IHierarchy, IWindows
    {
        #region fields

        public const string DOMAIN = "MSAA";

        //MSAA interface.
        protected IAccessible _iAcc;
        protected int _childID = 0;

        //MSAA property 
        protected String _defAction;
        protected String _description;
        protected String _name;
        protected IAccessible _parent;
        protected int _childCount = 0;
        protected RoleType _role;
        protected String _state;
        protected String _value;
        protected IntPtr _handle;
        protected Type _type;

        #endregion

        #region properties

        public IAccessible IAcc
        {
            get { return _iAcc; }
        }

        public int ChildID
        {
            get { return _childID; }
        }

        public RoleType Role
        {
            get { return _role; }
        }

        public String State
        {
            get { return _state; }
        }

        public int ChildCount
        {
            get { return _childCount; }
        }

        public IAccessible Parent
        {
            get { return _parent; }
        }

        #endregion

        #region methods

        #region ctor

        public MSAATestObject()
            : base()
        {
        }

        public MSAATestObject(IHTMLElement element)
            : base()
        {
            if (element == null)
            {
                throw new CannotBuildObjectException("HTML element can not be null.");
            }
            try
            {
                this._domain = DOMAIN;
                this._iAcc = COMUtil.IHTMLElementToMSAA(element);
                GetMSAAInfo();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.ToString());
            }
        }

        public MSAATestObject(IAccessible iAcc)
            : base()
        {
            if (iAcc == null)
            {
                throw new CannotBuildObjectException("MSAA interface can not be null.");
            }

            try
            {
                this._domain = DOMAIN;
                this._iAcc = iAcc;

                GetMSAAInfo();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.ToString());
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
                this._domain = DOMAIN;
                this._childID = childID;
                this._iAcc = parentAcc;

                GetMSAAInfo();

            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.ToString());
            }

        }

        public MSAATestObject(int x, int y)
            : base()
        {
            try
            {
                this._domain = DOMAIN;

                Win32API.POINT p = new Win32API.POINT(x, y);
                object childID = new object();
                Win32API.AccessibleObjectFromPoint(p, out this._iAcc, out childID);
                this._childID = (int)childID;

                GetMSAAInfo();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.ToString());
            }
        }

        public MSAATestObject(IntPtr handle)
            : base()
        {

            if (handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: handle can not be 0.");
            }

            try
            {
                this._domain = DOMAIN;
                Win32API.AccessibleObjectFromWindow(handle, -4, ref Win32API.IACCUID, ref this._iAcc);
                GetMSAAInfo();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.ToString());
            }
        }

        //get msaa object from windows event.
        public MSAATestObject(IntPtr hwnd, int dwObjectID, int dwChildID)
            : base()
        {
            if (hwnd != IntPtr.Zero)
            {
                try
                {
                    this._domain = DOMAIN;

                    object var = new object();
                    Win32API.AccessibleObjectFromEvent(hwnd, (uint)dwObjectID, (uint)dwChildID, out this._iAcc, out var);

                    if (this._iAcc != null)
                    {
                        this._handle = hwnd;
                        this._childID = (int)var;

                        GetMSAAInfo();
                    }
                }
                catch (Exception ex)
                {
                    throw new CannotBuildObjectException("Can not build MSAA test object: " + ex.ToString());
                }
            }
        }


        #endregion

        #region public methods

        #region IMSAA Members

        public IAccessible GetIAccInterface()
        {
            return this._iAcc;
        }

        public int GetChildID()
        {
            return Convert.ToInt32(this._childID);
        }

        #endregion


        /*  object GetProperty(string propertyName)
         *  get proerty of MSAA object.
         */
        public override object GetProperty(string propertyName)
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

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
                    return this._iAcc.get_accName(_childID);
                }
                else if (String.Compare(propertyName, "value", true) == 0)
                {
                    return this._iAcc.get_accValue(_childID);
                }
                else if (String.Compare(propertyName, "role", true) == 0)
                {
                    return this._iAcc.get_accRole(_childID);
                }
                else if (String.Compare(propertyName, "state", true) == 0)
                {
                    return this._iAcc.get_accState(_childID);
                }
                else if (String.Compare(propertyName, "defAction", true) == 0 || propertyName.IndexOf("action", StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    return this._iAcc.get_accDefaultAction(_childID);
                }
                else if (String.Compare(propertyName, "description", true) == 0)
                {
                    return this._iAcc.get_accDescription(_childID);
                }
                else if (String.Compare(propertyName, "help", true) == 0)
                {
                    return this._iAcc.get_accHelp(_childID);
                }
                else if (String.Compare(propertyName, "rect", true) == 0 || String.Compare(propertyName, "location", true) == 0)
                {
                    //get location.
                    int left, top, width, height;
                    this._iAcc.accLocation(out left, out top, out width, out height, _childID);

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
                throw new PropertyNotFoundException("Can not get property [" + propertyName + "]: " + ex.ToString());
            }
        }

        /* bool SetProperty(string propertyName, object value)
         * set property of MSAA object.
         * we can JUST set ".value" property for MSAA object.
         */
        public override bool SetProperty(string propertyName, object value)
        {
            if (this._iAcc != null && !String.IsNullOrEmpty(propertyName) && value != null)
            {
                try
                {
                    this._iAcc.set_accValue(_childID, value.ToString());
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }


        //get identify properties for MSAA test object.
        public override List<TestProperty> GetIdenProperties()
        {
            List<TestProperty> res = new List<TestProperty>();

            TestProperty domain = new TestProperty("Domain", this._domain);
            TestProperty name = new TestProperty("Name", this.GetName());
            TestProperty value = new TestProperty("Value", this.GetValue());
            TestProperty role = new TestProperty("Role", this.GetRole().ToString());
            TestProperty childID = new TestProperty("ChildID", this.GetChildID().ToString());
            TestProperty childCount = new TestProperty("ChildCount", this.GetChildCount().ToString());
            TestProperty desc = new TestProperty("Description", this.GetDesc());

            res.Add(name);
            res.Add(value);
            res.Add(role);
            res.Add(childID);
            res.Add(childCount);
            res.Add(desc);

            return res;
        }

        #region get msaa information

        public Rectangle GetRect()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                //get location.
                int left, top, width, height;
                this._iAcc.accLocation(out left, out top, out width, out height, _childID);

                return new Rectangle(left, top, width, height);
            }
            catch
            {
                return new Rectangle(0, 0, 0, 0);
            }
        }

        public virtual Point GetCenterPoint()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                //get location.
                int left, top, width, height;
                this._iAcc.accLocation(out left, out top, out width, out height, _childID);

                int x = left + width / 2;
                int y = top + height / 2;

                return new Point(x, y);
            }
            catch
            {
                return new Point(0, 0);
            }
        }

        public RoleType GetRole()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                int roleType = (int)this._iAcc.get_accRole(_childID);
                return GetRole(roleType);
            }
            catch
            {
                return RoleType.None;
            }
        }

        public String GetValue()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                return this._iAcc.get_accValue(_childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public String GetName()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                return this._iAcc.get_accName(_childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public String GetDefAction()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                return this._iAcc.get_accDefaultAction(_childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public String GetDesc()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                return this._iAcc.get_accDescription(_childID).ToString();
            }
            catch
            {
                return "";
            }
        }

        public String GetState()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                int stateID = (int)this._iAcc.get_accState(_childID);
                return GetStateText(stateID);
            }
            catch
            {
                return "";
            }
        }

        public virtual Object[] GetChildren()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            List<MSAATestObject> res = new List<MSAATestObject>();
            IAccessible[] childrenObj = GetChildrenIAcc(this._iAcc);
            if (childrenObj != null)
            {
                for (int i = 0; i < childrenObj.Length; i++)
                {
                    try
                    {
                        MSAATestObject childObj = new MSAATestObject(childrenObj[i]);
                        if (childObj != null)
                        {
                            res.Add(childObj);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            return res.ToArray();
        }

        public virtual Object GetChild(int childID)
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                IAccessible child = GetChildIAcc(this._iAcc, childID);
                IAccessible tmp;
                if (GetValidObjectFromWindow(child, out tmp))
                {
                    child = tmp;
                }
                return new MSAATestObject(child);
            }
            catch
            {
            }

            return null;
        }

        public virtual Object GetParent()
        {
            if (this._iAcc != null)
            {
                try
                {
                    IAccessible parent = this._iAcc.accParent as IAccessible;
                    return new MSAATestObject(parent);
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotBuildObjectException("Can not get parent: " + ex.ToString());
                }
            }

            return null;
        }

        public int GetChildCount()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            try
            {
                if (this._childID == 0)
                {
                    return this._iAcc.accChildCount;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        #region IWindows Members

        public IntPtr GetHandle()
        {
            if (this._iAcc == null)
            {
                throw new PropertyNotFoundException("IAcc interface can not be null.");
            }

            if (this._handle == IntPtr.Zero)
            {
                try
                {
                    Win32API.WindowFromAccessibleObject(this._iAcc, ref this._handle);
                }
                catch
                {
                }
            }

            return this._handle;
        }

        public string GetClass()
        {
            IntPtr handle = GetHandle();
            if (handle != IntPtr.Zero)
            {
                return Win32API.GetClassName(handle);
            }

            return "";
        }

        public string GetCaption()
        {
            IntPtr handle = GetHandle();
            if (handle != IntPtr.Zero)
            {
                return Win32API.GetWindowText(handle);
            }

            return "";
        }

        #endregion

        //refresh , force to get new property.
        public void Refresh()
        {
            if (this._iAcc != null)
            {
                _name = GetName();
                _defAction = GetDefAction();
                _state = GetState();
                _value = GetValue();
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
            if (this._iAcc != null)
            {
                _name = GetName();
                _defAction = GetDefAction();
                _description = GetDesc();

                //if the self id is 0, means the current MSAA interface is belong to this object, or belong to it's parent.
                //then try to find the parent. 
                if (Convert.ToInt32(_childID) == 0)
                {
                    _parent = (IAccessible)this._iAcc.accParent;
                    _childCount = this._iAcc.accChildCount;
                }
                else
                {
                    _parent = this._iAcc;
                    _childCount = 0;
                }

                _role = GetRole();
                _state = GetState();
                _value = GetValue();
            }
        }

        #endregion

        #endregion

    }
}
