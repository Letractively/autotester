using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.MSAAUtility
{
    public class MSAATestEventHook : IDisposable
    {
        #region fields

        //handle for the event hook
        private int _processID;
        private uint _startEvent;
        private uint _endEvent;

        private IntPtr _eventHook = IntPtr.Zero;
        private Win32API.WinEventDelegate _eventHandler;

        //fire event when new windows event.
        public delegate void WindowsEventHandler(uint eventType, IntPtr hwnd, int dwObjectID, int dwChildID);
        public event WindowsEventHandler OnWindowsEvent;

        #endregion

        #region methods

        public MSAATestEventHook()
            : this(0, 0)
        {
        }

        public MSAATestEventHook(uint startEvent, uint endEvent)
            : this(Process.GetCurrentProcess().Id, startEvent, endEvent)
        {
        }

        public MSAATestEventHook(int processID, uint startEvent, uint endEvent)
        {
            _processID = processID;
            this._startEvent = startEvent;
            this._endEvent = endEvent;
        }

        ~MSAATestEventHook()
        {
            Dispose();
        }

        public bool InstallHook()
        {
            return InstallEventHook();
        }

        public void UninstallHook()
        {
            UninstallEventHook();
        }

        public bool InstallEventHook()
        {
            return InstallEventHook(this._processID, this._startEvent, this._endEvent);
        }

        //hook event, if the process id is 0, means hook whole windows.
        //startEvent and endEvent are the range we want to hook.
        private bool InstallEventHook(int processID, uint startEvent, uint endEvent)
        {
            if (_eventHook == IntPtr.Zero)
            {
                try
                {
                    if (OnWindowsEvent == null || endEvent <= 0 || startEvent > endEvent)
                    {
                        return true;
                    }

                    this._startEvent = startEvent;
                    this._endEvent = endEvent;

                    _eventHandler = this.WinEvent;
                    _eventHook = Win32API.SetWinEventHook(startEvent, endEvent, IntPtr.Zero, _eventHandler, processID, 0, 0x0000);

                    //install failed.
                    if (_eventHook != IntPtr.Zero)
                    {
                        GC.KeepAlive(_eventHook);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                }

                return false;
            }

            return true;
        }

        public void UninstallEventHook()
        {
            if (_eventHook != IntPtr.Zero)
            {
                try
                {
                    Win32API.UnhookWinEvent(_eventHook);
                    _eventHook = IntPtr.Zero;
                    _eventHandler = null;
                }
                catch (Exception ex)
                {
                }
            }
        }

        #region handler

        //callback function to handle event.
        private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (OnWindowsEvent != null && eventType >= this._startEvent && eventType <= this._endEvent)
            {
                try
                {
                    OnWindowsEvent(eventType, hWnd, idObject, idChild);
                }
                catch (Exception ex)
                {
                }
            }
        }
        #endregion

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            UninstallHook();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
