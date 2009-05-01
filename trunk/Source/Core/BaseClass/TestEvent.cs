using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.Core
{
        public delegate void TestAppEventHandler(TestApp application, TestEventArgs args);
        public delegate void TestObjectEventHandler(TestObject sender, TestEventArgs args);

        public class TestEventArgs : EventArgs
        {
            #region fields

            protected string _eventName;
            protected object _eventValue;
            protected Point _mousePos;
            //keyboard status, ascii code of the key.
            protected int[] _keyCodes;

            public int[] KeyCodes
            {
                get { return _keyCodes; }
                set { _keyCodes = value; }
            }

            public Point MousePos
            {
                get { return _mousePos; }
            }

            public object EventValue
            {
                get { return _eventValue; }
            }

            public string EventName
            {
                get { return _eventName; }
            }

            #region methods

            public TestEventArgs(String eventName, String eventValue)
            {
                this._eventName = eventName;
                this._eventValue = eventValue;
                this._mousePos = MouseOp.GetMousePos();
            }

            public TestEventArgs(String eventName, String eventValue, Point mousePos)
                : this(eventName, eventValue, mousePos, null)
            {
            }

            public TestEventArgs(String eventName, String eventValue, int[] keyCodes)
            {
                this._eventName = eventName;
                this._eventValue = eventValue;
                this._mousePos = MouseOp.GetMousePos();
                this._keyCodes = keyCodes;
            }

            public TestEventArgs(String eventName, String eventValue, Point mousePos, int[] keyCodes)
            {
                this._eventName = eventName;
                this._eventValue = eventValue;
                this._mousePos = mousePos;
                this._keyCodes = keyCodes;
            }

            #endregion

            #endregion
        }
    }
