/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestGUIObject.cs
*
* Description: This class defines the methods for HTML GUI testing.
*              It will calculate the screen position for GUI object. 
*
* History: 2007/09/04 wan,yu Init version
*          2008/01/10 wan,yu update, move the calculate logic of center point
*                            from GetCenterPoint to  GetRectOnScreen
*          2008/01/14 wan,yu update, modify class name to HTMLTestGUIObject.
*          2008/01/15 wan,yu update, move GetRectOnScreen from creator to
*                                    Browser proeprty.   
*          2008/01/22 wan,yu update, add _labelText and GetAroundText() 
*          2008/01/28 wan,yu update, modify GetAroundText, return left and right string.  
*          2008/02/02 wan,yu update, add X,Y,Width,Height properties. 
*          2008/02/18 wan,yu update, add _isDelayAfterAction and _delayTime.
* 
*********************************************************************/


using System;
using System.Drawing;
using System.Threading;
using System.Text.RegularExpressions;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Core.TestExceptions;
using Shrinerain.AutoTester.Core.Helper;
using Shrinerain.AutoTester.Win32;
using Shrinerain.AutoTester.MSAAUtility;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestGUIObject : HTMLTestObject, IVisible
    {
        #region fields

        //the rectangle on screen of the object.
        protected Rectangle _rect;
        protected MSAATestObject _rectObj;

        protected IntPtr _ieServerHandle;

        //the center point of the object, this is very useful for GUI testing.
        //lot's of our actions we need move the mouse to the center point.
        protected Point _centerPoint;

        //sync event to ensure action is finished before next step.
        protected static AutoResetEvent _actionFinished = new AutoResetEvent(true);

        //reg to match <>, html tag.
        protected static Regex _regHtml = new Regex("<[^>]+?>", RegexOptions.Compiled);

        //label splitter
        protected const string _labelSplitter = "__shrinerain__";
        protected string _label;

        //when finish action, sleep for a moment.
        protected bool _isDelayAfterAction = false;
        protected const int ActionTimeout = 3 * 1000;
        protected const int ActionDelayTime = 200;

        //if set the flag to ture, we will not control the actual mouse and keyboard, just send windows message.
        //then we will not see the mouse move.
        protected bool _sendMsgOnly = true;

        //status of gui object.
        protected bool _isVisible = true;
        protected bool _isEnable = true;
        protected bool _isReadonly = false;

        #endregion

        #region properties

        public virtual Rectangle Rect
        {
            get
            {
                return GetRectOnScreen();
            }
        }

        public virtual Point CenterPoint
        {
            get
            {
                GetRectOnScreen();
                return _centerPoint;
            }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestGUIObject()
            : base()
        {
        }

        public HTMLTestGUIObject(IHTMLElement element)
            : this(element, null)
        {
        }

        public HTMLTestGUIObject(IHTMLElement element, HTMLTestPage page)
            : base(element, page)
        {
            if (!_sendMsgOnly)
            {
                GetRectOnScreen();
            }
            this._isEnable = IsEnable();
            this._isReadonly = IsReadonly();
            this._isVisible = IsVisible();
        }

        #endregion

        #region public methods

        public override object GetProperty(string propertyName)
        {
            if (String.Compare(TestConstants.PROPERTY_VISIBLE, propertyName, true) == 0)
            {
                return GetLabel();
            }
            else
            {
                return base.GetProperty(propertyName);
            }
        }

        public override bool TryGetProperty(string propertyName, out object value)
        {
            if (String.Compare(TestConstants.PROPERTY_VISIBLE, propertyName, true) == 0)
            {
                value = GetLabel();
                return true;
            }
            else
            {
                return base.TryGetProperty(propertyName, out value);
            }
        }

        /* Point GetCenterPoint()
         * Get the center point of the object.
         * Some actions like click, we need to find the center point, and move mouse to the point.
         */
        public virtual Point GetCenterPoint()
        {
            GetRectOnScreen();
            return _centerPoint;
        }

        /* Rectangle GetRectOnScreen()
         * Get the rectangle on screen of the object.
         * we use HTML dom to calculate the rect.
         */
        public virtual Rectangle GetRectOnScreen()
        {
            try
            {
                if (_rectObj == null)
                {
                    IHTMLElement tmp = this._sourceElement;
                    while (tmp != null)
                    {
                        MSAATestObject obj = new MSAATestObject(tmp);
                        if (obj.GetIAccInterface() != null)
                        {
                            _rectObj = obj;
                            break;
                        }
                        else
                        {
                            tmp = tmp.parentElement;
                        }
                    }
                }

                if (_rectObj != null)
                {
                    this._rect = _rectObj.GetRect();
                    this._centerPoint = _rectObj.GetCenterPoint();
                }

                return this._rect;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get object position: " + ex.ToString());
            }
        }

        public virtual Rectangle GetRectOnPage()
        {
            try
            {
                if (_rectObj == null)
                {
                    IHTMLElement tmp = this._sourceElement;
                    while (tmp != null)
                    {
                        MSAATestObject obj = new MSAATestObject(tmp);
                        if (obj.GetIAccInterface() != null)
                        {
                            _rectObj = obj;
                            break;
                        }
                        else
                        {
                            tmp = tmp.parentElement;
                        }
                    }
                }

                Rectangle objRect = _rectObj.GetRect();

                IntPtr ieServerHandle = GetIEServerHandle();
                Win32API.Rect pageRect = new Win32API.Rect();
                Win32API.GetWindowRect(ieServerHandle, ref pageRect);

                int offsetLeft = objRect.Left - pageRect.left;
                int offsetTop = objRect.Top - pageRect.top;

                return new Rectangle(offsetLeft, offsetTop, objRect.Width, objRect.Height);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get object position: " + ex.ToString());
            }
        }

        /* Bitmap GetControlPrint()
         * return the image of the object.
         */
        public virtual Bitmap GetControlPrint()
        {
            try
            {
                GetRectOnScreen();

                return new Bitmap(ScreenCaptruer.CaptureScreenArea(_rect.Left + 2, _rect.Top + 2, _rect.Width, _rect.Height));
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotSaveControlPrintException("Can not get control print: " + ex.ToString());
            }
        }

        /* GetControlPrint(string fileName)
         * save control print to a jpg file.
         */
        public virtual void GetControlPrint(string fileName)
        {
            try
            {
                Bitmap img = GetControlPrint();
                img.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotSaveControlPrintException("Can not save control print to a jpg file: " + ex.ToString());
            }
        }

        public virtual String GetLabel()
        {
            return _label;
        }

        /* GetAroundText(IHTMLElement element)
       * return the text around the control.
       */
        protected static string GetAroundText(IHTMLElement element)
        {
            if (element != null)
            {
                try
                {
                    string aroundText = null;
                    IHTMLElement parent = element.parentElement;
                    if (parent != null)
                    {
                        string tag = parent.tagName;
                        if (tag == "SPAN" || tag == "TD" || tag == "DIV" || tag == "LABEL" || tag == "FONT" || tag == "LI")
                        {
                            if (HTMLTestObject.TryGetProperty(parent, "innerText", out aroundText) && !String.IsNullOrEmpty(aroundText.Trim()))
                            {
                                IHTMLElementCollection brotherElements = (IHTMLElementCollection)parent.children;
                                if (brotherElements.length > 1)
                                {
                                    int currentObjIndex = 0;

                                    string myHTML = element.outerHTML;
                                    string parentHTML = parent.innerHTML;

                                    //we guess the position of the control by checking it's HTML code index.
                                    int curPos = parentHTML.IndexOf(myHTML);
                                    if (curPos > parentHTML.Length / 2)
                                    {
                                        for (int i = brotherElements.length - 1; i >= 0; i--)
                                        {
                                            if (element == brotherElements.item((object)i, (object)i))
                                            {
                                                currentObjIndex = i;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < brotherElements.length; i++)
                                        {
                                            if (element == brotherElements.item((object)i, (object)i))
                                            {
                                                currentObjIndex = i;
                                                break;
                                            }
                                        }
                                    }

                                    //try to get the left element.
                                    IHTMLElement prevElement = null;
                                    if (currentObjIndex > 0)
                                    {
                                        int prevIndex = currentObjIndex;
                                        while (true)
                                        {
                                            prevIndex--;
                                            if (prevIndex < 0)
                                            {
                                                prevElement = null;
                                                break;
                                            }

                                            prevElement = (IHTMLElement)brotherElements.item((object)prevIndex, (object)prevIndex);
                                            if (prevElement.tagName == "FONT" || prevElement.tagName == "SPAN")
                                            {
                                                continue;
                                            }

                                            if (ShouldBreak(prevElement))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    //try to get the right element.
                                    IHTMLElement nextElement = null;
                                    if (brotherElements.length - 1 > currentObjIndex)
                                    {
                                        int nextIndex = currentObjIndex;
                                        while (true)
                                        {
                                            nextIndex++;
                                            if (nextIndex >= brotherElements.length)
                                            {
                                                nextElement = null;
                                                break;
                                            }

                                            nextElement = (IHTMLElement)brotherElements.item((object)nextIndex, (object)nextIndex);
                                            if (nextElement.tagName == "FONT" || nextElement.tagName == "SPAN")
                                            {
                                                continue;
                                            }

                                            if (ShouldBreak(nextElement))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    string leftStr = "";
                                    int posLeft = 0;
                                    if (prevElement != null)
                                    {
                                        string prevHTML = prevElement.outerHTML;
                                        posLeft = parentHTML.IndexOf(prevHTML) + prevHTML.Length;
                                    }

                                    string leftHTML = parentHTML.Substring(posLeft, curPos - posLeft);
                                    //remove html tag and decode html code, eg: &nbsp; to a blank.
                                    leftStr = System.Web.HttpUtility.HtmlDecode(_regHtml.Replace(leftHTML, ""));

                                    string rightStr = "";
                                    int posRight = parentHTML.Length;
                                    if (nextElement != null)
                                    {
                                        string nextHTML = nextElement.outerHTML;
                                        posRight = parentHTML.IndexOf(nextHTML);
                                    }

                                    string rightHTML = parentHTML.Substring(curPos + myHTML.Length, posRight - curPos - myHTML.Length);
                                    //remove html tag.
                                    rightStr = System.Web.HttpUtility.HtmlDecode(_regHtml.Replace(rightHTML, ""));

                                    aroundText = leftStr.Trim() + _labelSplitter + rightStr.Trim();
                                }
                                else
                                {
                                    string selfText;
                                    if (HTMLTestObject.TryGetProperty(element, "innerText", out selfText) && !String.IsNullOrEmpty(selfText.Trim()))
                                    {
                                        aroundText = aroundText.Replace(selfText, "");
                                    }
                                }

                                return aroundText.Trim();
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return "";
        }

        private static bool ShouldBreak(IHTMLElement element)
        {
            string type = null;
            HTMLTestObject.TryGetProperty(element, "type", out type);
            if (element == null || element.tagName == "A" || (element.tagName == "INPUT" && String.Compare(type, "hidden", true) != 0)
                || element.tagName == "BUTTON" || !HTMLTestObject.HasProperty(element, "innerText"))
            {
                return true;
            }

            return false;
        }

        private static bool ShouldCheckCell(IHTMLElement element)
        {
            IHTMLElementCollection childrenElements = (IHTMLElementCollection)element.children;
            for (int i = 0; i < childrenElements.length; i++)
            {
                IHTMLElement curEle = (IHTMLElement)childrenElements.item((object)i, (object)i);
                if (ShouldBreak(curEle))
                {
                    return false;
                }
            }

            return true;
        }

        /* string GetAroundCellText(int direction)
         * if the element is in a cell"<td></td>", check the cell text around it.
         * direction:
         * 0 means up, 1 means right, 2 means down, 3 means left.
         */
        public static string GetAroundCellText(IHTMLElement element, int direction)
        {
            return GetAroundCellText(element, direction, 1);
        }

        //deepth: the deepth between current cell and searched cell.
        public static string GetAroundCellText(IHTMLElement element, int direction, int deepth)
        {
            //default, we will get the left cell text.
            if (direction < 0 || direction > 3)
            {
                direction = 3;
            }

            if (deepth < 1)
            {
                deepth = 1;
            }

            try
            {
                string label = null;

                IHTMLElement parentElement = element.parentElement;
                if (parentElement != null && parentElement.tagName == "TD")
                {
                    //current cell, <td>
                    IHTMLTableCell parentCellElement = (IHTMLTableCell)parentElement;
                    int cellId = parentCellElement.cellIndex;

                    //current row, <tr>
                    IHTMLTableRow parentRowElement = (IHTMLTableRow)parentElement.parentElement;
                    int rowId = parentRowElement.rowIndex;

                    //current table <table>
                    IHTMLTableSection tableSecElement = (IHTMLTableSection)parentElement.parentElement.parentElement;

                    IHTMLElement searchedElement = null;

                    object index = null;
                    if (direction == 0)
                    {
                        if (rowId - deepth >= 0)
                        {
                            for (int i = 1; i <= deepth; i++)
                            {
                                //get the up cell.
                                index = (object)(rowId - i);
                                IHTMLTableRow upRowElement = (IHTMLTableRow)tableSecElement.rows.item(index, index);

                                index = (object)cellId;
                                IHTMLElement upCellElement = (IHTMLElement)upRowElement.cells.item(index, index);

                                searchedElement = upCellElement;

                                if (!ShouldCheckCell(searchedElement))
                                {
                                    searchedElement = null;
                                    break;
                                }
                            }
                        }
                    }
                    else if (direction == 1)
                    {
                        if (cellId + deepth < parentRowElement.cells.length)
                        {
                            for (int i = 1; i <= deepth; i++)
                            {
                                index = (object)(cellId + i);

                                //get the right cell.
                                IHTMLElement rightElement = (IHTMLElement)parentRowElement.cells.item(index, index);
                                searchedElement = rightElement;

                                if (!ShouldCheckCell(searchedElement))
                                {
                                    searchedElement = null;
                                    break;
                                }
                            }
                        }
                    }
                    else if (direction == 2)
                    {
                        if (rowId + deepth < tableSecElement.rows.length)
                        {
                            for (int i = 1; i <= deepth; i++)
                            {
                                //get the down cell.
                                index = (object)(rowId + i);
                                IHTMLTableRow downRowElement = (IHTMLTableRow)tableSecElement.rows.item(index, index);

                                index = (object)cellId;
                                IHTMLElement downCellElement = (IHTMLElement)downRowElement.cells.item(index, index);

                                searchedElement = downCellElement;

                                if (!ShouldCheckCell(searchedElement))
                                {
                                    searchedElement = null;
                                    break;
                                }
                            }
                        }
                    }
                    else if (direction == 3)
                    {
                        if (cellId - deepth >= 0)
                        {
                            for (int i = 1; i <= deepth; i++)
                            {
                                index = (object)(cellId - i);

                                //get the left cell.
                                IHTMLElement leftElement = (IHTMLElement)parentRowElement.cells.item(index, index);
                                searchedElement = leftElement;

                                if (!ShouldCheckCell(searchedElement))
                                {
                                    searchedElement = null;
                                    break;
                                }
                            }
                        }
                    }

                    //try get the "innerText" property, it is the text in the cell.
                    if (searchedElement != null && HTMLTestObject.TryGetProperty(searchedElement, "innerText", out label) && !String.IsNullOrEmpty(label.Trim()))
                    {
                        return label;
                    }
                }
            }
            catch
            {
            }

            return "";
        }

        /* void Hover()
         * Move the mouse to the object.
         * This is a very important methods.
         * All the GUI actions like click, input, we need to move the mouse to the object first.
         */
        public virtual void Hover()
        {
            try
            {
                if (!IsVisible())
                {
                    throw new CannotPerformActionException("Object is not visible.");
                }

                if (_sendMsgOnly)
                {
                    FireEvent("onmouseover");
                }
                else
                {
                    this.ParentPage.Browser.Active();

                    //if the object is not visible, then move it.
                    ScrollIntoView();

                    //get the center point of the object, and move mouse to it.
                    MouseOp.MoveTo(_centerPoint);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform Hover action:" + ex.ToString());
            }
        }

        public virtual void KeyboardInput(String value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                try
                {
                    BeforeAction();
                    KeyboardOp.SendChars(value);
                }
                catch (TestException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new CannotPerformActionException(String.Format("Can not input {0} : {1}", value, ex.ToString()));
                }
                finally
                {
                    AfterAction();
                }
            }
        }

        public virtual void MouseClick(int x, int y)
        {
            try
            {
                BeforeAction();

                if (x <= 0 && y <= 0)
                {
                    Thread t = new Thread(new ThreadStart(PerformClick));
                    t.Start();
                    t.Join(ActionTimeout);
                }
                else
                {
                    GetRectOnScreen();
                    int actualX = this._rect.Left + x;
                    int actualY = this._rect.Top + y;

                    MouseOp.Click(actualX, actualY);
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform mouse click: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        /*  void HighLight()
         *  Highlight the object, we will see a red rect around the object.
         */
        public virtual void HighLight()
        {
            try
            {
                BeforeAction();

                Rectangle pageRect = GetRectOnPage();
                IntPtr ieServerHandle = GetIEServerHandle();
                ScreenCaptruer.HighlightScreenRect(ieServerHandle, pageRect, 600);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotHighlightObjectException("Can not highlight the object: " + ex.ToString());
            }
            finally
            {
                AfterAction();
            }
        }

        /* bool IsVisible()
       * return true if it is a visible object.
       */
        public virtual bool IsVisible()
        {

            if (this._sourceElement.offsetWidth < 1 || this._sourceElement.offsetHeight < 1)
            {
                return false;
            }

            string isDisabled;
            if (HTMLTestObject.TryGetProperty(_sourceElement, "disabled", out isDisabled))
            {
                if (String.Compare("true", isDisabled, true) == 0)
                {
                    return false;
                }
            }

            string isDisplayed;
            if (HTMLTestObject.TryGetProperty(_sourceElement, "style", out isDisplayed))
            {
                isDisplayed = isDisplayed.Replace(" ", "");
                if (isDisplayed.IndexOf("display:none", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            string isVisible;
            if (TryGetProperty(this._sourceElement, "visibility", out isVisible))
            {
                if (String.Compare(isVisible, "HIDDEN", true) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /* bool IsEnable()
         * return true if it is a enable object.
         */
        public virtual bool IsEnable()
        {
            string isEnable;

            if (!TryGetProperty(this._sourceElement, "diabled", out isEnable))
            {
                return true;
            }
            else
            {
                return !(String.Compare(isEnable, "true", true) == 0);
            }
        }

        /* bool IsReadOnly()
         * return true if it is a readonly object.
         */
        public virtual bool IsReadonly()
        {
            if (!IsEnable())
            {
                return true;
            }

            string isReadOnly;

            if (!TryGetProperty(this._sourceElement, "readOnly", out isReadOnly))
            {
                return false;
            }
            else
            {
                return String.Compare(isReadOnly, "TRUE", true) == 0;
            }
        }

        public virtual bool IsReadyForAction()
        {
            return IsVisible() && IsEnable() && IsReadonly() && IsReady();
        }

        public virtual bool IsFocused()
        {
            if (_sourceElement != null)
            {
                try
                {
                    MSAATestGUIObject obj = new MSAATestGUIObject(_sourceElement);
                    return obj.IsFocused();
                }
                catch
                {
                }
            }
            return false;
        }

        public virtual void Focus()
        {
            try
            {
                (this._sourceElement as IHTMLElement2).focus();
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not focus on element: " + ex.ToString());
            }
        }

        #endregion

        #region private methods

        protected override void SetIdenProperties()
        {
            base.SetIdenProperties();
            this._idenProperties.Add(TestConstants.PROPERTY_VISIBLE);
        }

        protected virtual void PerformClick()
        {
            Hover();
            if (_sendMsgOnly && this._sourceElement != null)
            {
                this._sourceElement.click();
                FireEvent("onmousedown");
                FireEvent("onmouseup");
                FireEvent("onclick");
            }
            else if (_centerPoint != null && _centerPoint.X > 0 && _centerPoint.Y > 0)
            {
                MouseOp.Click(this._centerPoint);
            }
        }

        protected virtual void PerformRightClick()
        {
            Hover();
            if (_sendMsgOnly && this._sourceElement != null)
            {
                IHTMLElement3 element3 = _sourceElement as IHTMLElement3;
                IHTMLDocument4 doc4 = _sourceElement.document as IHTMLDocument4;
                object dummy = null;
                object eventObj = doc4.CreateEventObject(ref dummy);
                element3.FireEvent("oncontextmenu", ref eventObj);
            }
            else if (_centerPoint != null && _centerPoint.X > 0 && _centerPoint.Y > 0)
            {
                MouseOp.RightClick(this._centerPoint);
            }
        }

        protected virtual void BeforeAction()
        {
            if (IsReadyForAction())
            {
                throw new CannotPerformActionException("Object is not ready.");
            }
            _actionFinished.WaitOne();
        }

        protected virtual void AfterAction()
        {
            if (_isDelayAfterAction)
            {
                System.Threading.Thread.Sleep(ActionDelayTime);
            }
            _actionFinished.Set();
        }

        /* void ScrollIntoView(bool toTop)
         * if the object is out of page view, scroll it to make it is visible.
         * Like some very long page, we can not see everything, we need to move the scrollbar to see
         * the bottom object.
         */
        protected virtual void ScrollIntoView()
        {
            //first, get the current position.
            int right = this._rect.X + this._rect.Width;
            int buttom = this._rect.Y + this._rect.Height;

            HTMLTestBrowser browser = this.ParentPage.Browser as HTMLTestBrowser;
            int currentWidth = browser.ClientLeft + browser.ClientWidth;
            int currentHeight = browser.ClientTop + browser.ClientHeight;

            //if the object is not visible, then move the scrollbar.
            if (right > currentWidth || buttom > currentHeight)
            {
                this._sourceElement.scrollIntoView(false);
            }
            else if (this._rect.Top < browser.ScrollTop || this._rect.Left < browser.ScrollLeft)
            {
                this._sourceElement.scrollIntoView(true);
            }

            //re-calculate the position, because we had move it.
            GetRectOnScreen();
        }

        protected IntPtr GetIEServerHandle()
        {
            if (_ieServerHandle == IntPtr.Zero)
            {
                int browserVersion = this.ParentPage.Browser.GetBrowserMajorVersion();
                IntPtr shellHandle = WindowsAsstFunctions.GetShellDocHandle(this.ParentPage.Handle, browserVersion);
                _ieServerHandle = WindowsAsstFunctions.GetIEServerHandle(shellHandle, browserVersion);
            }

            return _ieServerHandle;
        }

        #endregion

        #endregion
    }
}
