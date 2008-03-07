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
*          2008/01/10 wan,yu update, modify Click() method, add Hover() method 
*          2008/01/12 wan,yu update, wait for 30s to find the MessageBox, like 
*                                    other normal HTMLTestObject.
*          2008/01/12 wan,yu update, build message box by expected handle.                                    
*
*********************************************************************/

using System;
using System.Drawing;

using System.Text;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    //the icon type of Message box, we may have Warn(a yellow triangle), Error(a red cross), Info(a white triangle.)
    //Use Java Script, the default is Warn window, but use VB Script, we can generate other types.
    public enum HTMLTestMsgBoxIcon
    {
        Warn,
        Info,
        Error,
        Custom
    }

    public class HTMLTestMsgBox : HTMLTestGUIObject, IWindows, IShowInfo, IClickable, IContainer
    {

        #region fields

        //icon of the messagebox
        protected HTMLTestMsgBoxIcon _icon = HTMLTestMsgBoxIcon.Warn;

        //information on the messagebox
        protected string _message;

        protected IntPtr _handle;
        protected string _className;

        protected IntPtr _btnHandle;

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

            this._type = HTMLTestObjectType.MsgBox;

            //get windows handle of message box.
            try
            {
                this._className = "#32770 (Dialog)";

                IntPtr msgBoxHandle = IntPtr.Zero;

                //we will try to find the message box in 30s.
                int times = 0;
                while (times < 30)
                {
                    times++;
                    System.Threading.Thread.Sleep(1000 * 1);

                    //get the handle, the text of MessageBox is "Windows Internet Explorer", we use this to find it.
                    msgBoxHandle = Win32API.FindWindowEx(_browser.MainHandle, IntPtr.Zero, null, "Windows Internet Explorer");

                    if (msgBoxHandle != IntPtr.Zero)
                    {
                        this._handle = msgBoxHandle;
                        break;
                    }
                }

                if (msgBoxHandle == IntPtr.Zero)
                {
                    throw new CannotBuildObjectException("Can not get the windows handle of MessageBox.");
                }

                GetMessageBoxInfo();

            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build HTMLTestMsgBox: " + ex.Message);
            }

        }

        /* HTMLTestMsgBox(IntPtr handle)
         * build message box by expected handle.
         */
        public HTMLTestMsgBox(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw new CannotBuildObjectException("Handle of message box can not be 0.");
            }

            this._handle = handle;

            this._className = "#32770 (Dialog)";

            try
            {
                GetMessageBoxInfo();
            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not build test message box: " + ex.Message);
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
            try
            {
                Win32API.Rect tmp = new Win32API.Rect();
                Win32API.GetWindowRect(this._handle, ref tmp);

                CalCenterPoint(tmp.left, tmp.top, tmp.Width, tmp.Height);

                return new Rectangle(tmp.left, tmp.top, tmp.Width, tmp.Height);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get position of message box: " + ex.Message);
            }
        }

        /*  public override void Hover()
         *  Move mouse the the control.
         *  MessageBox is a special contorl, it is a standard Windows control, not a HTML control.
         *  So we don't need ScrollIntoView().
         */
        public override void Hover()
        {
            try
            {

                MouseOp.MoveTo(_centerPoint);

                //sleep for 0.2s, make it looks like human action.
                System.Threading.Thread.Sleep(200 * 1);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not move mouse to message box: " + ex.Message);
            }
        }


        #region IWindows Members

        public IntPtr GetHandle()
        {
            return this._handle;
        }

        public string GetClass()
        {
            return this._className;
        }

        #endregion

        #region IShowInfo Members

        public string GetText()
        {
            return this._message;
        }

        public string GetFontFamily()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontSize()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontColor()
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
                Click("OK");
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click on message box: " + ex.Message);
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
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click on message box: " + ex.Message);
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

        /* void GetMessageBoxInfo()
         * get message/button/icon information.
         */
        protected virtual void GetMessageBoxInfo()
        {
            try
            {
                //get text information displayed.
                this._message = GetMessage();

            }
            catch (CannotBuildObjectException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get the message text: " + ex.Message);
            }

            try
            {
                //get icon
                this._icon = GetIcon();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get icon of MessageBox: " + ex.Message);
            }

            try
            {
                //get button groups
                _buttons = GetButtons();
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get Buttons of MessageBox: " + ex.Message);
            }
        }

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

                if (_sendMsgOnly && _btnHandle != IntPtr.Zero)
                {
                    MouseOp.Click(_btnHandle);
                }
                else
                {
                    Hover();
                    MouseOp.Click();
                }

            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click button: " + text + " with " + ex.Message);
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
                            _btnHandle = lastButtonHandle;

                            //get the actual position.
                            Win32API.Rect tmp = new Win32API.Rect();
                            Win32API.GetWindowRect(okHandle, ref tmp);

                            CalCenterPoint(tmp.left, tmp.top, tmp.Width, tmp.Height);

                            return new Rectangle(tmp.left, tmp.top, tmp.Width, tmp.Height);
                        }

                    }
                    else
                    {
                        throw new CannotBuildObjectException("Can not get button: " + text);
                    }
                }
            }

        }

        protected virtual string GetMessage()
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
                    StringBuilder text = new StringBuilder(256);
                    Win32API.GetWindowText(messageHandle, text, 256);

                    if (text.Length == 0)
                    {
                        throw new CannotBuildObjectException("Can not get the message text");
                    }

                    return text.ToString();
                }

            }
        }

        #endregion

        #endregion
    }
}
