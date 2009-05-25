using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    [ComVisible(true)]
    public class HTMLTestEventDispatcher : ITestEventDispatcher
    {
        #region fields

        private static HTMLTestEventDispatcher _instance = new HTMLTestEventDispatcher();
        private HTMLTestBrowser _browser;
        private static List<IHTMLDocument2> _registeredDocs = new List<IHTMLDocument2>(8);

        private IHTMLElement _lastMouseDownElement;
        private IHTMLElement _lastKeyDownElement;

        #endregion

        #region methods

        #region ctor

        private HTMLTestEventDispatcher()
        {
        }

        public static HTMLTestEventDispatcher GetInstance()
        {
            return _instance;
        }

        #endregion

        #region public

        public void RegisterEvents(IHTMLDocument2 doc2)
        {
            if (doc2 != null && !_registeredDocs.Contains(doc2))
            {
                IHTMLDocument3 doc3 = doc2 as IHTMLDocument3;
                if (doc3 != null)
                {
                    doc3.attachEvent("onmousedown", this);
                    doc3.attachEvent("onmouseup", this);
                    doc3.attachEvent("onkeydown", this);
                    doc3.attachEvent("onkeyup", this);
                    doc3.attachEvent("onchange", this);
                    doc3.attachEvent("onfocus", this);
                    _registeredDocs.Add(doc2);
                }
            }
        }

        public void UnregisterEvents(IHTMLDocument2 doc2)
        {
            if (doc2 != null)
            {
                IHTMLDocument3 doc3 = doc2 as IHTMLDocument3;
                if (doc3 != null)
                {
                    doc3.detachEvent("onmousedown", this);
                    doc3.detachEvent("onmouseup", this);
                    doc3.detachEvent("onkeydown", this);
                    doc3.detachEvent("onkeyup", this);
                    doc3.detachEvent("onchange", this);
                    doc3.detachEvent("onfocus", this);
                    if (_registeredDocs.Contains(doc2))
                    {
                        _registeredDocs.Remove(doc2);
                    }
                }
            }
        }

        #endregion

        #region private methods

        [DispId(0)]
        public void HandleEvent(IHTMLEventObj pEvtObj)
        {
            lock (this)
            {
                if (pEvtObj != null)
                {
                    string eventType = pEvtObj.type;
                    IHTMLElement src = pEvtObj.srcElement;
                    if (src != null)
                    {
                        HTMLTestObject obj = null;
                        if (!CheckMouseEvent(pEvtObj, out obj))
                        {
                            CheckKeyEvent(pEvtObj, out obj);
                        }

                        if (String.Compare(eventType, "mousedown", true) == 0)
                        {
                            _lastMouseDownElement = src;
                            return;
                        }
                        else if (String.Compare(eventType, "mouseup", true) == 0)
                        {
                            if (_lastMouseDownElement != src)
                            {
                                return;
                            }
                            else
                            {
                                _lastMouseDownElement = null;
                            }
                        }
                        else if (String.Compare(eventType, "keydown", true) == 0)
                        {
                            _lastKeyDownElement = src;
                            return;
                        }
                        else if (String.Compare(eventType, "keyup", true) == 0)
                        {
                            if (_lastKeyDownElement != src)
                            {
                                return;
                            }
                            else
                            {
                                _lastKeyDownElement = null;
                            }
                        }

                        if (CheckClick(pEvtObj))
                        {
                            if (obj == null)
                            {
                                obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            }
                            OnClick(obj, null);
                        }
                        else if (CheckInput(pEvtObj))
                        {
                            if (obj == null)
                            {
                                obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            }
                            OnTextChange(obj, null);
                        }
                        else if (CheckSelect(pEvtObj))
                        {
                            if (obj == null)
                            {
                                obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            }
                            OnSelectIndexChange(obj, null);
                        }
                        else if (CheckCheckAction(pEvtObj))
                        {
                            if (obj == null)
                            {
                                obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            }
                            OnCheck(obj, null);
                        }
                        else if (CheckUnCheckAction(pEvtObj))
                        {
                            if (obj == null)
                            {
                                obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            }
                            OnUncheck(obj, null);
                        }
                        else if (CheckFocus(pEvtObj))
                        {
                            if (obj == null)
                            {
                                obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            }
                            OnFocus(obj, null);
                        }
                    }
                }
            }
        }

        private bool CheckMouseEvent(IHTMLEventObj pEvtObj, out HTMLTestObject obj)
        {
            obj = null;
            if (OnMouseDown != null || OnMouseUp != null || OnMouseClick != null)
            {
                String tp = pEvtObj.type;
                if (tp.IndexOf("mouse") >= 0)
                {
                    try
                    {
                        IHTMLElement src = pEvtObj.srcElement;
                        obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                        TestEventArgs mouseEventArgs = BuildMouseEventArgs(pEvtObj);

                        if (String.Compare(tp, "mousedown", true) == 0)
                        {
                            if (OnMouseDown != null)
                            {
                                OnMouseDown(obj, mouseEventArgs);
                            }
                        }
                        else if (String.Compare(tp, "mouseup", true) == 0)
                        {
                            if (OnMouseUp != null)
                            {
                                OnMouseUp(obj, mouseEventArgs);
                            }

                            if (OnMouseClick != null)
                            {
                                if (_lastMouseDownElement == src)
                                {
                                    OnMouseClick(obj, mouseEventArgs);
                                }
                            }
                        }
                    }
                    catch (TestException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new TestEventException("Create event failed: " + ex.ToString());
                    }
                }
            }

            return obj != null;
        }

        private bool CheckKeyEvent(IHTMLEventObj pEvtObj, out HTMLTestObject obj)
        {
            obj = null;
            if (OnKeyDown != null || OnKeyUp != null)
            {
                String tp = pEvtObj.type;
                if (tp.IndexOf("key") >= 0)
                {
                    try
                    {
                        IHTMLElement src = pEvtObj.srcElement;
                        obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                        if (String.Compare(tp, "keydown", true) == 0)
                        {
                            if (OnKeyDown != null)
                            {
                                OnKeyDown(obj, null);
                            }
                        }
                        else if (String.Compare(tp, "keyup", true) == 0)
                        {
                            if (OnKeyUp != null)
                            {
                                OnKeyUp(obj, null);
                            }
                        }
                    }
                    catch (TestException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new TestEventException("Create event failed: " + ex.ToString());
                    }
                }
            }

            return obj != null;
        }

        private bool CheckClick(IHTMLEventObj pEvtObj)
        {
            if (OnClick != null)
            {
                IHTMLElement src = pEvtObj.srcElement;
                String tp = pEvtObj.type;

                if (String.Compare("mouseup", tp, true) == 0)
                {
                    if (src is IHTMLInputElement)
                    {
                        String type = null;
                        if (HTMLTestObject.TryGetProperty(src, "type", out type))
                        {
                            if (String.Compare(type, "button", true) == 0 || String.Compare(type, "submit", true) == 0 ||
                                String.Compare(type, "reset", true) == 0 || String.Compare(type, "image", true) == 0)
                            {
                                return true;
                            }
                        }
                    }
                    else if (src is IHTMLAnchorElement || src is IHTMLButtonElement || src is IHTMLImgElement)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckInput(IHTMLEventObj pEvtObj)
        {
            if (OnTextChange != null)
            {
                IHTMLElement src = pEvtObj.srcElement;
                String tp = pEvtObj.type;

                if (String.Compare("keyup", tp, true) == 0)
                {
                    if (src is IHTMLInputElement || src is IHTMLTextAreaElement)
                    {
                        String type = null;
                        if (!HTMLTestObject.TryGetProperty(src, "type", out type))
                        {
                            return true;
                        }
                        else if (String.Compare(type, "text", true) == 0 || String.Compare(type, "password", true) == 0)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        String contentEditable = null;
                        if (HTMLTestObject.TryGetProperty(src, "contentEditable", out contentEditable))
                        {
                            return String.Compare(contentEditable, "true", true) == 0;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckSelect(IHTMLEventObj pEvtObj)
        {
            if (OnSelectIndexChange != null)
            {
                IHTMLElement src = pEvtObj.srcElement;
                if (String.Compare(src.tagName.ToUpper(), "SELECT", true) == 0)
                {
                    String tp = pEvtObj.type;
                    if (String.Compare("change", tp, true) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckCheckAction(IHTMLEventObj pEvtObj)
        {
            if (OnCheck != null)
            {
                String tp = pEvtObj.type;
                if (String.Compare("mouseup", tp, true) == 0)
                {
                    IHTMLElement src = pEvtObj.srcElement;
                    if (String.Compare(src.tagName, "INPUT", true) == 0)
                    {
                        string type;
                        if (HTMLTestObject.TryGetProperty(src, "type", out type))
                        {
                            if (String.Compare(type, "radio", true) == 0 || String.Compare(type, "checkbox", true) == 0)
                            {
                                return !(src as IHTMLInputElement).@checked;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckUnCheckAction(IHTMLEventObj pEvtObj)
        {
            if (OnUncheck != null)
            {
                String tp = pEvtObj.type;
                if (String.Compare("mouseup", tp, true) == 0)
                {
                    IHTMLElement src = pEvtObj.srcElement;
                    if (String.Compare(src.tagName, "INPUT", true) == 0)
                    {
                        string type;
                        if (HTMLTestObject.TryGetProperty(src, "type", out type))
                        {
                            if (String.Compare(type, "radio", true) == 0 || String.Compare(type, "checkbox", true) == 0)
                            {
                                return (src as IHTMLInputElement).@checked;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckFocus(IHTMLEventObj pEvtObj)
        {
            if (OnFocus != null)
            {
                String tp = pEvtObj.type;
                return String.Compare("focus", tp, true) == 0;
            }

            return false;
        }

        private static TestEventArgs BuildMouseEventArgs(IHTMLEventObj pEvtObj)
        {
            String mouseEventType = pEvtObj.type;
            MouseButton button = MouseButton.Left;
            try
            {
                button = (MouseButton)pEvtObj.button;
            }
            catch
            {
            }
            return new TestEventArgs(mouseEventType, button.ToString(), MouseOp.GetMousePos(), button, null);
        }


        private void OnBrowserFound(TestApp application, TestEventArgs args)
        {
            if (application != null && application is HTMLTestBrowser)
            {
                this._browser = application as HTMLTestBrowser;
                RegisterEvents(this._browser.GetDocument() as IHTMLDocument2);
            }
        }

        private void OnBrowserStart(TestApp application, TestEventArgs args)
        {
            if (application != null && application is HTMLTestBrowser)
            {
                this._browser = application as HTMLTestBrowser;
                RegisterEvents(this._browser.GetDocument() as IHTMLDocument2);
            }
        }

        #endregion

        #endregion

        #region ITestEventDispatcher Members

        public bool Start(ITestApp app)
        {
            if (app != null && app is HTMLTestBrowser)
            {
                //register events.
                this._browser = app as HTMLTestBrowser;
                this._browser.OnAfterAppStart += new TestAppEventHandler(OnBrowserStart);
                this._browser.OnAfterAppFound += new TestAppEventHandler(OnBrowserFound);
                RegisterEvents(this._browser.GetDocument() as IHTMLDocument2);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Stop()
        {
            //unregister events.
            if (_registeredDocs.Count > 0)
            {
                foreach (IHTMLDocument2 doc2 in _registeredDocs)
                {
                    try
                    {
                        UnregisterEvents(doc2);
                    }
                    catch
                    {
                        continue;
                    }
                }

                _registeredDocs.Clear();
            }
        }

        public event TestObjectEventHandler OnCheck;

        public event TestObjectEventHandler OnUncheck;

        public event TestObjectEventHandler OnClick;

        public event TestObjectEventHandler OnDrag;

        public event TestObjectEventHandler OnKeyDown;

        public event TestObjectEventHandler OnKeyUp;

        public event TestObjectEventHandler OnMouseDown;

        public event TestObjectEventHandler OnMouseUp;

        public event TestObjectEventHandler OnMouseClick;

        public event TestObjectEventHandler OnTextChange;

        public event TestObjectEventHandler OnFocus;

        public event TestObjectEventHandler OnSelectIndexChange;

        public event TestObjectEventHandler OnSelect;

        public event TestObjectEventHandler OnUnselect;

        public event TestObjectEventHandler OnStatusChange;

        public event TestObjectEventHandler OnShow;

        #endregion
    }
}
