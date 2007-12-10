/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestMsgBox.cs
*
* Description: This class defines the actions provide by MessageBox.
*              MessageBox can have different buttons. It is a standard
*              Windows control. 
*              The Important actions include "Click".
* 
*
* History: 2007/12/10 wan,yu Init version.
*
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using mshtml;

using Shrinerain.AutoTester.Function;
using Shrinerain.AutoTester.Function.Interface;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    //the icon type of Message box, we may have Warn(a yellow triangle), Error(a red cross), Info(a white triangle.)
    public enum HTMLTestMsgBoxIcon
    {
        Warn,
        Info,
        Error,
        Custom
    }

    public class HTMLTestMsgBox : HTMLGuiTestObject, IWindows, IShowInfo, IClickable, IContainer
    {

        #region fields

        //icon of the messagebox
        protected HTMLTestMsgBoxIcon _icon;

        //information on the messagebox
        protected string _message;

        protected IntPtr _handle;

        //button groups, for different messagebox, we may have different buttons. eg: OK, Yes,No, Cancel.
        protected string[] _buttons;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestMsgBox(IHTMLElement element)
            : base(element)
        {
            //get windows handle of message box.
            try
            {
                _class = "#32770 (Dialog)";

                //get the handle, the text of MessageBox is "Windows Internet Explorer", we use this to find it.
                IntPtr msgBoxHandle = Win32API.FindWindowEx(TestBrowser.MainHandle, IntPtr.Zero, null, "Windows Internet Explorer");
                if (msgBoxHandle == IntPtr.Zero)
                {
                    throw new CanNotBuildObjectException("Can not get the windows handle of MessageBox.");
                }

            }
            catch (CanNotBuildObjectException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CanNotBuildObjectException("Can not build HTMLTestMsgBox: " + e.Message);
            }

            //get text information displayed.
            try
            {

                //firstly we need to get the handle.
                // the first handle is for the icon.
                IntPtr messageHandle = Win32API.FindWindowEx(this._handle, IntPtr.Zero, "Static", null);

                if (messageHandle == IntPtr.Zero)
                {
                    throw new CanNotBuildObjectException("Can not get icon handle.");
                }
                else
                {
                    //the 2nd handle is the actual handle for the text.
                    messageHandle = Win32API.FindWindowEx(this._handle, messageHandle, "Static", null);

                    if (messageHandle == IntPtr.Zero)
                    {
                        throw new CanNotBuildObjectException("Can not get text handle.");
                    }
                    else
                    {
                        StringBuilder text = new StringBuilder(50);
                        Win32API.GetWindowText(messageHandle, text, 50);

                        this._message = text.ToString();

                        if (String.IsNullOrEmpty(this._message))
                        {
                            throw new CanNotBuildObjectException("Can not Can not get the message text");
                        }
                    }

                }

            }
            catch (CanNotBuildObjectException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CanNotBuildObjectException("Can not get the message text: " + e.Message);
            }

            //get icon
            //java script window.alert can only generate info type.
            this._icon = HTMLTestMsgBoxIcon.Info;

            //get button groups
            //java script window.alert can only generate "OK" button.
            _buttons = new string[] { "OK" };


        }

        #endregion

        #region public methods

        /* Rectangle GetRectOnScreen()
         * Get the actual position of button on the message box.
         * HTMLTestMsgBox is NOT a HTML control but a standard Windows control.
         */
        public override Rectangle GetRectOnScreen()
        {
            //firstly, we need to find the button handle.
            IntPtr okHandle = Win32API.FindWindowEx(this._handle, IntPtr.Zero, "Button", null);

            if (okHandle == IntPtr.Zero)
            {
                throw new CanNotBuildObjectException("Can not get the OK button.");
            }
            else
            {
                //get the actual position.
                Win32API.Rect tmp = new Win32API.Rect();
                Win32API.GetClientRect(okHandle, ref tmp);

                Rectangle tmpRect = new Rectangle(tmp.left, tmp.top, tmp.Width, tmp.Height);
                return tmpRect;
            }
        }


        #region IWindows Members

        public IntPtr GetHandle()
        {
            return this._handle;
        }

        public string GetClass()
        {
            return this._class;
        }

        #endregion

        #region IShowInfo Members

        public string GetText()
        {
            return this._message;
        }

        public string GetFontStyle()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontFamily()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IClickable Members

        /* void Click()
         * Click the button of Message box.
         */
        public void Click()
        {
            try
            {
                _actionFinished.WaitOne();

                Point point = GetCenterPoint();

                MouseOp.Click(point.X, point.Y);

                _actionFinished.Set();
            }
            catch (Exception e)
            {
                throw new CanNotPerformActionException("Can not click on message box: " + e.Message);
            }
        }

        public void DoubleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RightClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void MiddleClick()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IInteractive Members

        public void Focus()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object GetDefaultAction()
        {
            return "Click";
        }

        public void PerformDefaultAction(object parameter)
        {
            Click();
        }

        #endregion

        #region IContainer Members

        public object[] GetChildren()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #endregion

        #region private methods

        protected String[] GetButtons()
        {
            return this._buttons;
        }


        #endregion

        #endregion
    }
}
