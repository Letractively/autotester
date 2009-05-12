using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestEventDispatcher : ITestEventDispatcher
    {
        #region fields

        private static HTMLTestEventDispatcher _instance = new HTMLTestEventDispatcher();
        private HTMLTestBrowser _browser;

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

        public void RegisterEvents(IHTMLDocument2 doc)
        {
            if (doc != null)
            {
                HTMLTestEventWrapper eventWrapper = new HTMLTestEventWrapper(doc);
                eventWrapper.DHTMLEventHandler += new HTMLTestEventWrapper.DHTMLEvent(CheckClick);
                doc.onkeydown = eventWrapper;
                HTMLDocumentEvents2_Event event2 = doc as HTMLDocumentEvents2_Event;
                // event2.onkeydown += new HTMLDocumentEvents2_onkeydownEventHandler(CheckClick);
            }
        }

        #endregion

        #region private methods

        private void CheckClick(IHTMLEventObj pEvtObj)
        {
            if (OnClick != null)
            {
                IHTMLElement src = pEvtObj.srcElement;
                if (src is IHTMLInputElement || src is IHTMLAnchorElement
                    || src is IHTMLButtonElement || src is IHTMLImgElement)
                {
                    HTMLTestObject obj = HTMLTestObjectFactory.BuildHTMLTestObject(src, _browser, null);
                    OnClick(obj, null);
                }
            }
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

    public class HTMLTestEventWrapper
    {
        public delegate void DHTMLEvent(IHTMLEventObj e);
        public event DHTMLEvent DHTMLEventHandler;

        private IHTMLDocument2 m_Document;

        public HTMLTestEventWrapper(IHTMLDocument2 doc)
        {
            m_Document = doc;
        }

        [DispId(0)]
        public void FireEvent()
        {
            DHTMLEventHandler(m_Document.parentWindow.@event);
        }
    }
}
