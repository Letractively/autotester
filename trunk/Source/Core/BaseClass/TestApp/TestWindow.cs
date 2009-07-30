using System;
using System.Collections.Generic;
using System.Text;

using Shrinerain.AutoTester.Core.Interface;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
    public class TestWindow : ITestWindow
    {
        #region fields

        protected TestApp _app;
        protected String _caption;
        protected String _className;
        protected IntPtr _handle;

        protected TestObjectMap _objMap;

        #endregion

        #region properties

        public virtual int Left
        {
            get
            {
                Win32API.Rect rect = new Win32API.Rect();
                Win32API.GetWindowRect(_handle, ref rect);

                return rect.left;
            }
        }

        public virtual int Top
        {
            get
            {
                Win32API.Rect rect = new Win32API.Rect();
                Win32API.GetWindowRect(_handle, ref rect);

                return rect.top;
            }
        }

        public virtual int Width
        {
            get
            {
                Win32API.Rect rect = new Win32API.Rect();
                Win32API.GetWindowRect(_handle, ref rect);

                return rect.Width;
            }
        }

        public virtual int Height
        {
            get
            {
                Win32API.Rect rect = new Win32API.Rect();
                Win32API.GetWindowRect(_handle, ref rect);

                return rect.Height;
            }
        }

        public string Caption
        {
            get
            {
                if (_handle != IntPtr.Zero)
                {
                    _caption = Win32API.GetWindowText(_handle);
                }

                return _caption;
            }
        }

        public string ClassName
        {
            get
            {
                if (_handle != IntPtr.Zero)
                {
                    _className = Win32API.GetClassName(_handle);
                }

                return _className;
            }
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public virtual ITestApp App
        {
            get { return _app; }
        }

        public virtual ITestObjectMap Objects
        {
            get
            {
                return GetObjectMap();
            }
        }

        #endregion

        #region methods

        public TestWindow(IntPtr handle, TestApp app)
        {
            if (handle == IntPtr.Zero || app == null)
            {
                throw new CannotGetTestWindowException("Handle and Application can not be 0.");
            }

            this._app = app;
            this._handle = handle;
        }

        public virtual ITestObjectPool GetObjectPool()
        {
            throw new TestObjectPoolExcpetion("Plugin must implement GetObjectPool");
        }

        public virtual ITestObjectMap GetObjectMap()
        {
            throw new TestObjectMapException("Plugin must implement GetObjectMap");
        }

        #endregion
    }
}
