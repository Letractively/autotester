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

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    //the icon type of Message box, we may have Warn(a yellow triangle), Error(a red cross), Info(a white triangle.)
    //Use Java Script, we can only generate Warn window, but use VB Script, we can generate other types.
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
        protected HTMLTestMsgBoxIcon _icon = HTMLTestMsgBoxIcon.Warn;

        //information on the messagebox
        protected string _message;

        protected IntPtr _handle;

        //button groups, for different messagebox, we may have different buttons. eg: OK, Yes,No, Cancel.
        protected string[] _buttons = new string[] { "OK" };

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestMsgBox()
            : base()
        {
            //get windows handle of message box.
            try
            {
                _class = "#32770 (Dialog)";

                //get the handle, the text of MessageBox is "Windows Internet Explorer", we use this to find it.
                IntPtr msgBoxHandle = Win32API.FindWindowEx(TestBrowser.MainHandle, IntPtr.Zero, null, "Windows Internet Explorer");
                if (msgBoxHandle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get the windows handle of MessageBox.");
                }

            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not build HTMLTestMsgBox: " + e.Message);
            }

            //get text information displayed.
            try
            {

                //firstly we need to get the handle.
                // the first handle is for the icon.
                IntPtr messageHandle = Win32API.FindWindowEx(this._handle, IntPtr.Zero, "Static", null);

                if (messageHandle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get icon handle.");
                }
                else
                {
                    //the 2nd handle is the actual handle for the text.
                    messageHandle = Win32API.FindWindowEx(this._handle, messageHandle, "Static", null);

                    if (messageHandle == IntPtr.Zero)
                    {
                        throw new CannotBuildObjectException("Can not get text handle.");
                    }
                    else
                    {
                        StringBuilder text = new StringBuilder(50);
                        Win32API.GetWindowText(messageHandle, text, 50);

                        this._message = text.ToString();

                        if (String.IsNullOrEmpty(this._message))
                        {
                            throw new CannotBuildObjectException("Can not Can not get the message text");
                        }
                    }

                }

            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get the message text: " + e.Message);
            }

            try
            {
                //get icon
                this._icon = GetIcon();
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get icon of MessageBox: " + e.Message);
            }

            try
            {
                //get button groups
                _buttons = GetButtons();
            }
            catch (Exception e)
            {
                throw new CannotBuildObjectException("Can not get Buttons of MessageBox: " + e.Message);
            }

        }

        #endregion

        #region public methods

        /* Rectangle GetRectOnScreen()
         * Get the actual position of button on the message box.
         * HTMLTestMsgBox is NOT a HTML control but a standard Windows control.
         * So we need to overrider this method.
         */
        public override Rectangle GetRectOnScreen()
        {
            //Get the "OK" button position
            //It is OK for JavaScript.
            return GetButtonPosition("OK");
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
         * Click the "OK" button.
         * It is OK for JavaScript Window.Alert messagebox.
         */
        public void Click()
        {
            try
            {
                _actionFinished.WaitOne();

                Point okPos = GetCenterPoint();

                MouseOp.Click(okPos.X, okPos.Y);

                _actionFinished.Set();
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not click on message box: " + e.Message);
            }
        }

        /* void Click(string text)
         * Click on the expected button.
         */
        public void Click(string text)
        {
            try
            {
                _actionFinished.WaitOne();

                ClickButton(text);

                _actionFinished.Set();
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not click on message box: " + e.Message);
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
            Click();
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

        /* String[] GetButtons()
         * Return the button groups on the MessageBox.
         * eg: OK, Yes, No, Cancel.
         */
        protected virtual String[] GetButtons()
        {
            //java script window.alert can only generate "OK" button.
            //we can use VBScript to generate other type.
            return new string[] { "OK" };
        }

        /*  HTMLTestMsgBoxIcon GetIcon()
         *  Return the Icon type on the MessageBox.
         * 
         */
        protected virtual HTMLTestMsgBoxIcon GetIcon()
        {
            //java script window.alert can only generate info type.
            //we can use VBScript to generate other type.
            return HTMLTestMsgBoxIcon.Warn;
        }

        /*  void ClickButton(string text)
         *  Click on expect button.
         */
        protected virtual void ClickButton(string text)
        {
            try
            {
                this._rect = GetButtonPosition(text);

                Point btnPos = GetCenterPoint();

                MouseOp.Click(btnPos.X, btnPos.Y);

            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CannotPerformActionException("Can not click button: " + text + " with " + e.Message);
            }
        }

        /*  Rectangle GetButtonPosition(string text)
         *  Get the actual position on the screen of the expected button.
         */
        protected virtual Rectangle GetButtonPosition(string text)
        {
            IntPtr lastButtonHandle = IntPtr.Zero;

            while (true)
            {
                //firstly, we need to find the button handle.
                IntPtr okHandle = Win32API.FindWindowEx(this._handle, lastButtonHandle, "Button", null);

                //0 handle, means we can not find it.
                if (okHandle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get the button: " + text);
                }
                else
                {
                    lastButtonHandle = okHandle;

                    StringBuilder textSB = new StringBuilder(10);

                    //get the text on the button.
                    Win32API.GetWindowText(okHandle, textSB, 10);

                    if (textSB.Length > 0)
                    {
                        if (String.Compare(textSB.ToString(), text, true) == 0)
                        {
                            //get the actual position.
                            Win32API.Rect tmp = new Win32API.Rect();
                            Win32API.GetClientRect(okHandle, ref tmp);

                            Rectangle tmpRect = new Rectangle(tmp.left, tmp.top, tmp.Width, tmp.Height);
                            return tmpRect;
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else
                    {
                        throw new CannotBuildObjectException("Can not get button: " + text);
                    }
                }
            }

        }

        #endregion

        #endregion
    }
}
