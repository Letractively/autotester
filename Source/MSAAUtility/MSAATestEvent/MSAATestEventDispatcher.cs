using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.Core.Interface;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestEventDispatcher : ITestEventDispatcher
    {
        #region fields

        private static MSAATestEventDispatcher _instance = new MSAATestEventDispatcher();

        //private static MouseHook _mouseHook;
        private static MSAATestEventHook _eventHook;
        private const uint StartEvent = (uint)Win32API.WindowsEvent.SYS_MENUPOPUPSTART;
        private const uint EndEvent = (uint)Win32API.WindowsEvent.OBJ_VALUECHANGE;

        //cache
        private MSAATestObject _lastMSAAObj;
        private IntPtr _lastHwnd;
        private int _lastidObject;
        private int _lastidChild;

        #endregion

        #region methods

        #region ctor

        private MSAATestEventDispatcher()
        {
        }

        public static MSAATestEventDispatcher GetInstance()
        {
            return _instance;
        }

        #endregion

        #region public

        public bool Start(ITestApp app)
        {
            if (app != null && (app as TestApp).Handle != IntPtr.Zero)
            {
                if (_eventHook == null)
                {
                    _eventHook = new MSAATestEventHook(StartEvent, EndEvent);
                    _eventHook.OnWindowsEvent += new MSAATestEventHook.WindowsEventHandler(HandleEvent);
                    _eventHook.InstallEventHook();
                }

                //if (_mouseHook == null)
                //{
                //    _mouseHook = MouseHook.GetInstance();
                //    //_mouseHook.InstallHook(app.GetProcess().MainWindowHandle);
                //    //_mouseHook.OnMouseEvent += new MouseHook.MouseEventHandler(HandleMouseEvent);
                //}
            }

            return app != null && _eventHook != null;// && _mouseHook != null;
        }

        public void Stop()
        {
            if (_eventHook != null)
            {
                _eventHook.UninstallHook();
            }

            //if (_mouseHook != null)
            //{
            //    _mouseHook = MouseHook.GetInstance();
            //    _mouseHook.UninstallHook();
            //}
        }

        #endregion

        #region private

        //fire mouse event.
        private void HandleMouseEvent(int key, int x, int y)
        {
            if (key == (int)Win32API.WM_MOUSE.WM_LBUTTONDOWN)
            {
                if (OnMouseDown != null)
                {
                    TestEventArgs args = new TestEventArgs("OnMouseDown", key.ToString(), new Point(x, y));
                    OnMouseDown(null, args);
                }
            }
            else if (key == (int)Win32API.WM_MOUSE.WM_LBUTTONUP)
            {
                if (OnMouseUp != null)
                {
                    TestEventArgs args = new TestEventArgs("OnMouseUp", key.ToString(), new Point(x, y));
                    OnMouseUp(null, args);
                }
            }
        }

        private void HandleEvent(uint eventType, IntPtr hwnd, int idObject, int idChild)
        {
            if (eventType == (uint)Win32API.WindowsEvent.OBJ_STATECHANGE)
            {
                MSAATestObject obj = GetMSAATestObjectFromEvent((IntPtr)hwnd, idObject, idChild);

                if (obj != null)
                {
                    if (CheckButtonClick(obj))
                    {
                        OnClick(obj, null);
                    }
                    else if (CheckCheckBox(obj))
                    {
                        OnCheck(obj, null);
                    }
                    else if (CheckRadioBox(obj))
                    {
                        OnCheck(obj, null);
                    }
                }
            }
            else if (eventType == (uint)Win32API.WindowsEvent.SYS_DIALOGSTART)
            {
                MSAATestObject obj = GetMSAATestObjectFromEvent((IntPtr)hwnd, idObject, idChild);

                if (obj != null)
                {
                    if (CheckNewWindowPopup(obj))
                    {
                        OnShow(obj, null);
                    }
                }
            }
            else if (eventType == (uint)Win32API.WindowsEvent.OBJ_VALUECHANGE)
            {
                MSAATestObject obj = GetMSAATestObjectFromEvent((IntPtr)hwnd, idObject, idChild);

                if (obj != null)
                {
                    if (CheckInputText(obj))
                    {
                        OnTextChange(obj, null);
                    }
                }
            }
        }

        private MSAATestObject GetMSAATestObjectFromEvent(IntPtr hwnd, int idObject, int idChild)
        {
            //try to get object from cache.
            if (hwnd != _lastHwnd || idObject != _lastidObject || idChild != _lastidChild || _lastMSAAObj == null)
            {
                _lastMSAAObj = new MSAATestObject(hwnd, idObject, idChild);
                _lastHwnd = hwnd;
                _lastidObject = idObject;
                _lastidChild = idChild;
            }
            else
            {
                _lastMSAAObj.Refresh();
            }

            return _lastMSAAObj;
        }

        #endregion

        #region check element

        private bool CheckRadioBox(MSAATestObject obj)
        {
            if (OnCheck != null)
            {
                if (obj.Role == MSAATestObject.RoleType.RadioButton)
                {
                    if (obj.State.IndexOf("Focused") >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckCheckBox(MSAATestObject obj)
        {
            if (OnCheck != null)
            {
                if (obj.Role == MSAATestObject.RoleType.CheckButton)
                {
                    if (obj.State.IndexOf("Focused") >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckButtonClick(MSAATestObject obj)
        {
            if (OnClick != null)
            {
                if (obj.Role == MSAATestObject.RoleType.PushButton)
                {
                    if (obj.State.IndexOf("Pressed") >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckInputText(MSAATestObject obj)
        {
            if (OnTextChange != null)
            {
                if (obj.Role == MSAATestObject.RoleType.Text)
                {
                    if (obj.State.IndexOf("Focused") >= 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckNewWindowPopup(MSAATestObject obj)
        {
            if (OnShow != null)
            {
                if (obj.Role == MSAATestObject.RoleType.Window || obj.Role == MSAATestObject.RoleType.Dialog)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region ITestEventDispatcher Members

        public event TestObjectEventHandler OnCheck;

        public event TestObjectEventHandler OnUncheck;

        public event TestObjectEventHandler OnClick;

        public event TestObjectEventHandler OnDrag;

        public event TestObjectEventHandler OnKeyDown;

        public event TestObjectEventHandler OnKeyUp;

        public event TestObjectEventHandler OnTextChange;

        public event TestObjectEventHandler OnFocus;

        public event TestObjectEventHandler OnSelectIndexChange;

        public event TestObjectEventHandler OnSelect;

        public event TestObjectEventHandler OnUnselect;

        public event TestObjectEventHandler OnStatusChange;

        public event TestObjectEventHandler OnShow;

        public event TestObjectEventHandler OnMouseDown;

        public event TestObjectEventHandler OnMouseUp;

        public event TestObjectEventHandler OnMouseClick;

        #endregion
    }
}
