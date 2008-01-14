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
        protected IntPtr _parentHandle;

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
            get { return _parentHandle; }
            set { _parentHandle = value; }
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
        }

        public MFCTestObject(string className)
        {
            this._domain = "MFC";

            if (String.IsNullOrEmpty(className))
            {
                throw new CannotBuildObjectException("Class name can not be empty.");
            }
        }

        public MFCTestObject(string caption, string className)
        {
            this._domain = "MFC";

            if (String.IsNullOrEmpty(caption) && String.IsNullOrEmpty(className))
            {
                throw new CannotBuildObjectException("Caption and Class name can not be both empty.");
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

        #endregion

        #endregion

    }
}
