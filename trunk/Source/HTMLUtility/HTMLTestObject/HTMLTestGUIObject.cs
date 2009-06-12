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
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestGUIObject : HTMLTestObject, IVisible
    {
        #region fields

        //the rectangle on screen of the object.
        protected Rectangle _rect;

        //the center point of the object, this is very useful for GUI testing.
        //lot's of our actions we need move the mouse to the center point.
        protected Point _centerPoint;

        //sync event to ensure action is finished before next step.
        protected static AutoResetEvent _actionFinished = new AutoResetEvent(true);

        //reg to match <>, html tag.
        protected static Regex _regHtml = new Regex("<[^>]+?>", RegexOptions.Compiled);

        //label splitter
        protected const string _labelSplitter = "__shrinerain__";

        //when finish action, sleep for a moment.
        protected bool _isDelayAfterAction = false;
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

        public virtual int X
        {
            get
            {
                GetRectOnScreen();
                return _rect.X;
            }
        }

        public virtual int Y
        {
            get
            {
                GetRectOnScreen();
                return _rect.Y;
            }
        }

        public virtual int Width
        {
            get
            {
                GetRectOnScreen();
                return _rect.Width;
            }
        }

        public virtual int Height
        {
            get
            {
                GetRectOnScreen();
                return _rect.Height;
            }
        }

        public bool SendMsgOnly
        {
            get { return _sendMsgOnly; }
            set { _sendMsgOnly = value; }
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

        public HTMLTestGUIObject(IHTMLElement element, HTMLTestBrowser browser)
            : base(element, browser)
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
                // get it's position offset to it's parent object.
                int top = this._sourceElement.offsetTop;
                int left = this._sourceElement.offsetLeft;
                int width = this._sourceElement.offsetWidth;
                int height = this._sourceElement.offsetHeight;

                //if width or height is 0, error.
                if (width <= 0 || height <= 0)
                {
                    throw new CannotGetObjectPositionException("width and height of object can not be 0.");
                }

                //find parent object, calculate 
                //the offsetTop/offsetLeft... is the distance between current object and it's parent object.
                //so we need a loop to get the actual position on the screen.
                IHTMLElement parent = this._sourceElement.offsetParent;
                while (parent != null)
                {
                    top += parent.offsetTop;
                    left += parent.offsetLeft;

                    if (parent.offsetTop == 0 && parent.offsetLeft == 0)
                    {
                        top++;
                        left++;
                    }

                    parent = parent.offsetParent;
                }

                if (_browser != null)
                {
                    //get the browser information, get the real position on screen.
                    top += _browser.ClientTop + 1;
                    left += _browser.ClientLeft + 1;

                    top -= _browser.ScrollTop;
                    left -= _browser.ScrollLeft;
                }

                if (this._rect.Left != left || this._rect.Top != top || this._rect.Width != width || this._rect.Height != height)
                {
                    // we will calculate the center point of this object.
                    CalCenterPoint(left, top, width, height);
                    this._rect = new Rectangle(left, top, width, height);
                }

                return _rect;
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
            return "";
        }

        /* GetAroundText(IHTMLElement element)
       * return the text around the control.
       */
        public static string GetAroundText(IHTMLElement element)
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
                    this._browser.Active();

                    //if the object is not visible, then move it.
                    ScrollIntoView();

                    //get the center point of the object, and move mouse to it.
                    MouseOp.MoveTo(_centerPoint);

                    //after move mouse to the control, wait for 0.2s, make it looks like human action.
                    Thread.Sleep(200 * 1);
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

        public virtual void MouseClick()
        {
            if (_centerPoint != null && _centerPoint.X > 0 && _centerPoint.Y > 0)
            {
                MouseOp.Click(this._centerPoint);
            }
        }

        /*  void HighLight()
         *  Highlight the object, we will see a red rect around the object.
         */
        public virtual void HighLight()
        {
            try
            {
                _actionFinished.WaitOne();
                ThreadPool.QueueUserWorkItem(HighLightRectCallback, null);
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotHighlightObjectException("Can not highlight the object: " + ex.ToString());
            }
        }

        /* bool IsVisible()
       * return true if it is a visible object.
       */
        public virtual bool IsVisible()
        {
            string isVisible;

            if (this._sourceElement.offsetWidth < 1 || this._sourceElement.offsetHeight < 1)
            {
                return false;
            }

            if (!TryGetProperty(this._sourceElement, "visibility", out isVisible))
            {
                return true && IsDisplayed(this._sourceElement);
            }
            else
            {
                return String.Compare(isVisible, "HIDDEN", true) == 0;
            }
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

        public virtual void Focus()
        {
            (this._sourceElement as IHTMLElement2).focus();
        }

        #endregion

        #region private methods

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

        /* bool IsDisplay(IHTMLElement element)
        * Check the style
        */
        protected virtual bool IsDisplayed(IHTMLElement element)
        {
            if (element == null)
            {
                return false;
            }

            string isDisabled;
            if (HTMLTestObject.TryGetProperty(element, "disabled", out isDisabled))
            {
                if (String.Compare("true", isDisabled, true) == 0)
                {
                    return false;
                }
            }

            string isDisplayed;
            if (HTMLTestObject.TryGetProperty(element, "style", out isDisplayed))
            {
                isDisplayed = isDisplayed.Replace(" ", "");
                if (isDisplayed.IndexOf("display:none", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            return true;
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

            int currentWidth = _browser.ClientLeft + _browser.ClientWidth;
            int currentHeight = _browser.ClientTop + _browser.ClientHeight;

            int oriTop = _browser.ScrollTop;
            int oriLeft = _browser.ScrollLeft;

            //if the object is not visible, then move the scrollbar.
            if (right > currentWidth || buttom > currentHeight)
            {
                this._sourceElement.scrollIntoView(false);
                if (oriTop == _browser.ScrollTop && oriLeft == _browser.ScrollLeft)
                {
                    //!!!need update here!!!
                    int newTop = _browser.ClientTop + _browser.ClientHeight - 10 - this._rect.Height / 2;
                    this._rect = new Rectangle(this._rect.Left, newTop, this._rect.Width, this._rect.Height);
                    CalCenterPoint(this._rect.Left, newTop, this._rect.Width, this._rect.Height);
                    //this._browser.Find(this._browser.GetTitle());

                    //System.Threading.Thread.Sleep(1 * 1000);
                    //times++;
                    // continue;
                }
                else
                {
                    //re-calculate the position, because we had move it.
                    GetRectOnScreen();
                }
            }
            else if (this._rect.Top < this._browser.ScrollTop || this._rect.Left < this._browser.ScrollLeft)
            {
                this._sourceElement.scrollIntoView(true);

                if (oriTop == _browser.ScrollTop && oriLeft == _browser.ScrollLeft)
                {
                    int newTop = _browser.ClientTop + 10 - this._rect.Height / 2;
                    this._rect = new Rectangle(this._rect.Left, newTop, this._rect.Width, this._rect.Height);
                    CalCenterPoint(this._rect.Left, newTop, this._rect.Width, this._rect.Height);
                }
                else
                {
                    GetRectOnScreen();
                }
            }
        }

        /* void HighLightRectCallback(Object obj)
         * callback function to highlight the object.
         */
        protected virtual void HighLightRectCallback(Object obj)
        {
            HighLightRect(false);
        }

        /* void HighLightRect(bool isWindowsControl)
         * Highlight the object.
         * is it is not a windows control, we need to minus the browser position.
         */
        protected virtual void HighLightRect(bool isWindowsControl)
        {
            try
            {
                int left = this._rect.Left;
                int top = this._rect.Top;
                int width = this._rect.Width;
                int height = this._rect.Height;

                //if the control is not a windows standard control,we need to minus the browser top and left.
                //because if it is NOT a windows control, then we consider it is a HTML control, when we get the handle,
                //the handle is belonged to "Internet Explorer_Server", it not include the menu bar...
                //so we need to minus the menu bar height and top to get the actual position.
                if (!isWindowsControl)
                {
                    left -= _browser.ClientLeft;
                    top -= _browser.ClientTop;
                }

                ScreenCaptruer.HighlightScreenRect(left, top, width, height, 1000);
            }
            catch (Exception ex)
            {
                throw new CannotHighlightObjectException("Can not high light object: " + ex.ToString());
            }
        }

        /* CalCenterPoint(int top, int left, int width, int height)
         * get the center point of the control
         */
        protected virtual Point CalCenterPoint(int left, int top, int width, int height)
        {
            _centerPoint.X = left + width / 2;
            _centerPoint.Y = top + height / 2;

            return _centerPoint;
        }

        #endregion

        #endregion
    }
}
