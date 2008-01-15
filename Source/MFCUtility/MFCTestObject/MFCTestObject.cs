/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: MFCTestObject.cs
*
* Description: MFC test object is the root class for MFC testing.
*
* History: 2008/01/14 wan,yu Init version
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MFCUtility
{
    //defines the types of MFC control.
    public enum MFCTestObjectType : int
    {
        Unknow = 0,
        Label,
        Button,
        CheckBox,
        RadioButton,
        TextBox,
        ComboBox,
        ListBox,
        Tree,
        Table,
        Image,
        MsgBox,
        FileDialog,
    }

    public class MFCTestObject : TestObject, IWindows
    {

        #region fields

        protected MFCTestObjectType _type = MFCTestObjectType.Unknow;

        //basic information of a MFC control.
        protected IntPtr _handle;
        protected string _className;
        protected string _caption;

        //we often need parent handle to find a MFC control.
        protected List<IntPtr> _parentHandles = new List<IntPtr>(5);

        protected bool _isVisible;

        #endregion

        #region properties

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public string ClassName
        {
            get { return _className; }
        }

        public string Caption
        {
            get { return _caption; }
        }

        public IntPtr ParentHandle
        {
            get
            {
                if (_parentHandles.Count > 0)
                {
                    return _parentHandles[_parentHandles.Count - 1];
                }
                else
                {
                    return IntPtr.Zero;
                }
            }
        }

        public MFCTestObjectType Type
        {
            get { return _type; }
        }

        #endregion

        #region methods

        #region ctor

        public MFCTestObject()
        {
            this._domain = "MFC";
        }

        public MFCTestObject(IntPtr handle)
        {
            this._domain = "MFC";

            if (handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Handle can not be 0.");
            }

            try
            {
                this._className = GetClassNameByHandle();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get class name: " + e.Message);
            }

            try
            {
                this._caption = GetCaptionByHandle();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get caption: " + e.Message);
            }
        }

        public MFCTestObject(string className)
        {
            this._domain = "MFC";

            if (String.IsNullOrEmpty(className))
            {
                throw new CannotBuildObjectException("Class name can not be empty.");
            }

            try
            {
                this._handle = GetObjectHandle(className);

                if (this._handle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get handle.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get handle: " + e.Message);
            }

            try
            {
                this._caption = GetCaptionByHandle();
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get caption: " + e.Message);
            }


        }

        public MFCTestObject(string caption, string className)
        {
            this._domain = "MFC";

            if (String.IsNullOrEmpty(caption) && String.IsNullOrEmpty(className))
            {
                throw new CannotBuildObjectException("Caption and Class name can not be both empty.");
            }

            try
            {
                this._handle = GetObjectHandle(className, caption);

                if (this._handle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get handle.");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get handle: " + e.Message);
            }

        }

        #endregion

        #region public methods

        #region IWindows Members

        public IntPtr GetHandle()
        {
            return this._handle;
        }

        public string GetClass()
        {
            return this._className;
        }

        public override bool IsVisible()
        {
            if (this._handle == IntPtr.Zero)
            {
                return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region private methods

        /* IntPtr GetObjectHandle(string className)
         * return the windows handle by expected class
         * use FindWindow and FindWindowEx.
         */
        protected virtual IntPtr GetObjectHandle(string className)
        {
            if (String.IsNullOrEmpty(className))
            {
                throw new CannotBuildObjectException("Can not get windows handle: class name can not be empty.");
            }

            try
            {
                IntPtr handle = Win32API.FindWindow(className, null);

                //not found, try use caption.
                if (handle == IntPtr.Zero)
                {
                    handle = Win32API.FindWindow(null, className);
                }

                if (handle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get object handle by: " + className);
                }

                return handle;
            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get windows handle: " + e.Message);
            }


        }

        protected virtual IntPtr GetObjectHandle(string className, string caption)
        {
            if (String.IsNullOrEmpty(className) && String.IsNullOrEmpty(caption))
            {
                throw new CannotBuildObjectException("Can not get windows handle: class name and caption can not be both empty.");
            }

            try
            {
                IntPtr handle = Win32API.FindWindow(className, caption);

                if (handle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get handle.");
                }

                return handle;
            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get windows handle: " + e.Message);
            }

        }

        /* string GetClassNameByHandle()
         * return the class name of current object.
         */
        protected virtual string GetClassNameByHandle()
        {
            if (this._handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Can not get class name.");
            }

            try
            {
                StringBuilder sb = new StringBuilder(50);

                Win32API.GetClassName(this._handle, sb, sb.Capacity);

                if (sb.Length > 0)
                {
                    return sb.ToString().Trim();
                }
                else
                {
                    throw new CannotBuildObjectException("Can not get class name.");
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get class name: " + e.Message);
            }
        }


        /* string GetCaptionByHandle()
         * get the text(caption) of this object.
         */
        protected virtual string GetCaptionByHandle()
        {
            if (this._handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Handle can not be 0");
            }

            try
            {
                StringBuilder sb = new StringBuilder(50);

                Win32API.GetWindowText(this._handle, sb, sb.Capacity);

                return sb.ToString().Trim();
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get caption: " + e.Message);
            }
        }

        #endregion

        #endregion

    }
}
