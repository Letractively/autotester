using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    [ComVisible(true)]
    public class HTMLTestEventDispatcher : ITestEventDispatcher
    {
        #region fields

        private static HTMLTestEventDispatcher _instance = new HTMLTestEventDispatcher();
        private HTMLTestBrowser _browser;

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
            if (doc2 != null)
            {
                IHTMLDocument3 doc3 = doc2 as IHTMLDocument3;
                if (doc3 != null)
                {
                    doc3.attachEvent("onmousedown", this);
                    doc3.attachEvent("onmouseup", this);
                    doc3.attachEvent("onkeydown", this);
                    doc3.attachEvent("onkeyup", this);
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
                }
            }
        }

        #endregion

        #region private methods

        [DispId(0)]
        public void HandleEvent(IHTMLEventObj pEvtObj)
        {
            IHTMLElement src = pEvtObj.srcElement;

            HTMLTestObject obj = null;
            CheckMouseKeyEvent(pEvtObj, out obj);

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
        }

        private bool CheckMouseKeyEvent(IHTMLEventObj pEvtObj, out HTMLTestObject obj)
        {
            obj = null;
            IHTMLElement src = pEvtObj.srcElement;
            if (src != null)
            {
                String tp = pEvtObj.type;

                try
                {
                    if (OnMouseDown != null)
                    {
                        if (String.Compare(tp, "mousedown", true) == 0)
                        {
                            obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            OnMouseDown(obj, null);
                        }
                    }

                    if (OnMouseUp != null)
                    {
                        if (String.Compare(tp, "mouseup", true) == 0)
                        {
                            obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            OnMouseUp(obj, null);
                        }
                    }

                    if (OnKeyDown != null)
                    {
                        if (String.Compare(tp, "keydown", true) == 0)
                        {
                            obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
                            OnKeyDown(obj, null);
                        }
                    }

                    if (OnKeyUp != null)
                    {
                        if (String.Compare(tp, "keyup", true) == 0)
                        {
                            obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser);
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

            return obj != null;
        }

        private bool CheckClick(IHTMLEventObj pEvtObj)
        {
            if (OnClick != null)
            {
                IHTMLElement src = pEvtObj.srcElement;
                if (src != null)
                {
                    String tp = pEvtObj.type;

                    if (String.Compare("mousedown", tp, true) == 0)
                    {
                        _lastMouseDownElement = src;
                    }
                    else if (String.Compare("mouseup", tp, true) == 0)
                    {
                        if (_lastMouseDownElement == src)
                        {
                            _lastMouseDownElement = null;
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
                }
            }

            return false;
        }

        private bool CheckInput(IHTMLEventObj pEvtObj)
        {
            if (OnTextChange != null)
            {
                IHTMLElement src = pEvtObj.srcElement;
                if (src != null)
                {
                    String tp = pEvtObj.type;

                    if (String.Compare("keydown", tp, true) == 0)
                    {
                        _lastKeyDownElement = src;
                    }
                    else if (String.Compare("keyup", tp, true) == 0)
                    {
                        if (_lastKeyDownElement == src)
                        {
                            _lastKeyDownElement = null;
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
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region ITestEventDispatcher Members

        public bool Start(ITestApp app)
        {
            if (app != null && app is HTMLTestBrowser)
            {
                this._browser = app as HTMLTestBrowser;
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
