using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.HTMLUtility;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.SpySharp
{
    public partial class SpyForm : Form
    {
        #region fields

        private const int WM_LBUTTONUP = 0x202;
        private const int WM_MOUSEMOVE = 0x0200;

        private Image _FinderHome;
        private Image _FinderGone;
        private Cursor _DefaultCursor;
        private Cursor _FinderCursor;
        private bool _capturing;
        private IntPtr _hPreviousWindow;

        private HTMLTestPlugin _html;
        private HTMLTestSession _ts;
        private IntPtr _lastHandle;
        private bool _isSupported;
        private TestObject _lastObj;

        #endregion

        #region methods

        #region ctor

        public SpyForm()
        {
            InitializeComponent();
            InitImages();
        }

        private void InitImages()
        {
            try
            {
                string path = "../../";
                this._FinderHome = new Bitmap(path + "Resources/FinderHome.bmp");
                this._FinderGone = new Bitmap(path + "Resources/FinderGone.bmp");
                this._FinderCursor = new Cursor(path + "Resources/finder.cur");
                this._DefaultCursor = this.DefaultCursor;
                this.pictureBox1.Image = this._FinderHome;
                this.pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            }
            catch
            {
                MessageBox.Show("Error when loading images.");
            }
        }

        #endregion

        #region private

        [DllImport("user32.dll")]
        private static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseCapture();

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                /*
                 * stop capturing events as soon as the user releases the left mouse button
                 * */
                case WM_LBUTTONUP:
                    this.CaptureMouse(false);
                    break;
                /*
                 * handle all the mouse movements
                 * */
                case WM_MOUSEMOVE:
                    this.HandleMouseMovements();
                    break;
            };

            base.WndProc(ref m);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.CaptureMouse(true);
        }

        private void CaptureMouse(bool captured)
        {
            if (captured)
            {
                SetCapture(this.Handle);
                Cursor.Current = this._FinderCursor;
                this.pictureBox1.Image = this._FinderGone;
            }
            else
            {
                ReleaseCapture();
                Cursor.Current = this._DefaultCursor;
                this.pictureBox1.Image = this._FinderHome;
                if (_hPreviousWindow != IntPtr.Zero)
                {
                    _hPreviousWindow = IntPtr.Zero;
                }

            }
            _capturing = captured;
        }


        private void HandleMouseMovements()
        {
            if (!_capturing)
                return;

            try
            {
                Point p = MouseOp.GetMousePos();

                if (_html == null)
                {
                    _html = new HTMLTestPlugin();
                    _ts = _html.CreateTestSession() as HTMLTestSession;
                }

                IntPtr curHandle = Win32API.WindowFromPoint(p.X, p.Y);
                if (curHandle != _lastHandle)
                {
                    _lastHandle = curHandle;
                    _isSupported = _html.IsSupportedApp(curHandle);
                    if (_isSupported)
                    {
                        _ts.Browser.Find(curHandle);
                        //_ts.Event.OnMouseOver += new TestObjectEventHandler(Event_OnMouseOver);
                    }
                }

                if (_isSupported)
                {
                    TestObject obj = _ts.Objects.ObjectPool.GetObjectByPoint(p.X, p.Y, true);

                    if (!obj.Equals(_lastObj))
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(SetText));
                        t.Start(obj);
                        _lastObj = obj;
                    }
                    else
                    {
                        IVisible v = obj as IVisible;
                        v.HighLight(1000);
                    }
                }
            }
            catch
            {
            }
        }

        void Event_OnMouseOver(TestObject sender, TestEventArgs e)
        {
            if (_isSupported)
            {
                if (!sender.Equals(_lastObj))
                {
                    Thread t = new Thread(new ParameterizedThreadStart(SetText));
                    t.Start(sender);
                    _lastObj = sender;
                }
                else
                {
                    IVisible v = sender as IVisible;
                    v.HighLight(1000);
                }
            }
        }

        private void SetText(Object obj)
        {
            TestObject testobj = obj as TestObject;
            SetText(testobj.ToString());
        }

        delegate void SetTextCallback(string text);

        private void SetText(String text)
        {
            if (richTextBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                richTextBox1.Text = text;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #endregion
    }
}