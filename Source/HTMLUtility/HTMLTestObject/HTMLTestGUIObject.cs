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
using Shrinerain.AutoTester.Helper;
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

        //we may have some string around the control.
        protected string _labelText;

        //reg to match <>, html tag.
        protected static Regex _regHtml = new Regex("<[^>]+?>", RegexOptions.Compiled);

        //label splitter
        protected const string _labelSplitter = "__shrinerain__";

        //when finish action, sleep for a moment.
        protected bool _isDelayAfterAction = true;
        protected const int _delayTime = 1;
        protected bool _isUnderAction = false;

        //if set the flag to ture, we will not control the actual mouse and keyboard, just send windows message.
        //then we will not see the mouse move.
        protected bool _sendMsgOnly = false;

        #endregion

        #region properties

        public virtual Rectangle Rect
        {
            get { return _rect; }
            set
            {
                if (value != null)
                {
                    _rect = value;
                }
            }
        }

        public virtual int X
        {
            get
            {
                return _rect.X;
            }
        }

        public virtual int Y
        {
            get
            {
                return _rect.Y;
            }
        }

        public virtual int Width
        {
            get
            {
                return _rect.Width;
            }
        }

        public virtual int Height
        {
            get
            {
                return _rect.Height;
            }
        }

        public bool IsUnderAction
        {
            get
            {
                return _isUnderAction;
            }
        }

        public bool IsDelayAfterAction
        {
            get { return _isDelayAfterAction; }
            set { _isDelayAfterAction = value; }
        }

        //when set the html browser, we can start to calculate the position
        public override HTMLTestBrowser Browser
        {
            set
            {
                this._browser = value;
                GetRectOnScreen();
            }
            get
            {
                return this._browser;
            }
        }

        public virtual string LabelText
        {
            get
            {
                //for performance issue, we won't get the label text at creator,
                //we will get the label text on demand.
                if (String.IsNullOrEmpty(_labelText))
                {
                    _labelText = GetLabelText();
                }

                return _labelText;
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
            //when init, get the position information.
            //GetRectOnScreen();
        }

        public HTMLTestGUIObject(IHTMLElement element)
            : base(element)
        {
            //GetRectOnScreen();
        }

        ~HTMLTestGUIObject()
        {
            Dispose();
        }

        #endregion

        #region public methods

        /* void Dispose()
         * When GC, close AutoResetEvent
         */
        public override void Dispose()
        {
            try
            {
                base.Dispose();
                if (_actionFinished != null)
                {
                    //_actionFinished.Close();
                    //_actionFinished = null;
                }
                GC.SuppressFinalize(this);
            }
            catch
            {

            }
        }

        /* Point GetCenterPoint()
         * Get the center point of the object.
         * Some actions like click, we need to find the center point, and move mouse to the point.
         */
        public virtual Point GetCenterPoint()
        {
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
                int top = _sourceElement.offsetTop;
                int left = _sourceElement.offsetLeft;
                int width = _sourceElement.offsetWidth;
                int height = _sourceElement.offsetHeight;

                //if width or height is 0, error.
                if (width <= 0 || height <= 0)
                {
                    throw new CannotGetObjectPositionException("width and height of object can not be 0.");
                }

                //find parent object, calculate 
                //the offsetTop/offsetLeft... is the distance between current object and it's parent object.
                //so we need a loop to get the actual position on the screen.
                IHTMLElement parent = _sourceElement.offsetParent;
                while (parent != null)
                {
                    top += parent.offsetTop;
                    left += parent.offsetLeft;
                    parent = parent.offsetParent;
                }

                //get the browser information, get the real position on screen.
                top += _browser.ClientTop;
                left += _browser.ClientLeft;

                top -= _browser.ScrollTop;
                left -= _browser.ScrollLeft;

                // we will calculate the center point of this object.
                CalCenterPoint(left, top, width, height);

                this._rect = new Rectangle(left, top, width, height);

                return _rect;
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotGetObjectPositionException("Can not get object position: " + ex.Message);
            }
        }


        /* Bitmap GetControlPrint()
         * return the image of the object.
         */
        public virtual Bitmap GetControlPrint()
        {
            try
            {
                //we need to add 2, then we can get the "right" print, don't ask me why, ask Microsoft...
                return new Bitmap(ImageHelper.CaptureScreenArea(_rect.Left + 2, _rect.Top + 2, _rect.Width, _rect.Height));
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotSaveControlPrintException("Can not get control print: " + ex.Message);
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
                throw new CannotSaveControlPrintException("Can not save control print to a jpg file: " + ex.Message);
            }
        }


        /* GetAroundText(IHTMLElement element)
       * return the text around the control.
       */
        public static string GetAroundText(IHTMLElement element)
        {
            try
            {

                if (element == null)
                {
                    return null;
                }

                string aroundText = null;

                IHTMLElement parent = element.parentElement;

                if (parent != null)
                {
                    string tag = parent.tagName;

                    if (tag == "SPAN" || tag == "TD" || tag == "DIV" || tag == "LABEL" || tag == "FONT")
                    {
                        if (HTMLTestObject.TryGetProperty(parent, "innerText", out aroundText))
                        {

                            IHTMLElementCollection brotherElements = (IHTMLElementCollection)parent.children;

                            if (brotherElements.length > 1)
                            {
                                int currentObjIndex = 0;

                                object name;
                                object index;

                                string myHTML = element.outerHTML;
                                string parentHTML = parent.innerHTML;

                                //we guess the position of the control by checking it's HTML code index.
                                int curPos = parentHTML.IndexOf(myHTML);
                                if (curPos > parentHTML.Length / 2)
                                {
                                    for (int i = brotherElements.length - 1; i >= 0; i--)
                                    {
                                        name = (object)i;
                                        index = (object)i;

                                        if (element == brotherElements.item(name, index))
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
                                        name = (object)i;
                                        index = (object)i;

                                        if (element == brotherElements.item(name, index))
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

                                        if (prevElement == null
                                            || prevElement.tagName == "A"
                                            || prevElement.tagName == "INPUT"
                                            || prevElement.tagName == "BUTTON"
                                            || !HTMLTestObject.TryGetProperty(prevElement, "innerText"))
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

                                        if (nextElement == null
                                            || nextElement.tagName == "A"
                                            || nextElement.tagName == "INPUT"
                                            || nextElement.tagName == "BUTTON"
                                            || !HTMLTestObject.TryGetProperty(nextElement, "innerText"))
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
                                if (HTMLTestObject.TryGetProperty(element, "innerText", out selfText))
                                {
                                    aroundText = aroundText.Replace(selfText, "");
                                }
                            }

                            return aroundText.Trim();
                        }
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }

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
                            //get the up cell.
                            index = (object)(rowId - deepth);
                            IHTMLTableRow upRowElement = (IHTMLTableRow)tableSecElement.rows.item(index, index);

                            index = (object)cellId;
                            IHTMLElement upCellElement = (IHTMLElement)upRowElement.cells.item(index, index);

                            searchedElement = upCellElement;

                        }
                    }
                    else if (direction == 1)
                    {
                        if (cellId + deepth < parentRowElement.cells.length)
                        {
                            index = (object)(cellId + deepth);

                            //get the right cell.
                            IHTMLElement rightElement = (IHTMLElement)parentRowElement.cells.item(index, index);

                            searchedElement = rightElement;
                        }
                    }
                    else if (direction == 2)
                    {
                        if (rowId + deepth < tableSecElement.rows.length)
                        {
                            //get the down cell.
                            index = (object)(rowId + deepth);
                            IHTMLTableRow downRowElement = (IHTMLTableRow)tableSecElement.rows.item(index, index);

                            index = (object)cellId;
                            IHTMLElement downCellElement = (IHTMLElement)downRowElement.cells.item(index, index);

                            searchedElement = downCellElement;
                        }
                    }
                    else if (direction == 3)
                    {

                        if (cellId - deepth >= 0)
                        {
                            index = (object)(cellId - deepth);

                            //get the left cell.
                            IHTMLElement leftElement = (IHTMLElement)parentRowElement.cells.item(index, index);

                            searchedElement = leftElement;
                        }
                    }

                    //try get the "innerText" property, it is the text in the cell.
                    if (HTMLTestObject.TryGetProperty(searchedElement, "innerText", out label))
                    {
                        return label;
                    }

                }

                return null;
            }
            catch
            {
                return null;
            }
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
                if (!this._isVisible)
                {
                    throw new CannotPerformActionException("Object is not visible.");
                }

                if (!_sendMsgOnly)
                {
                    this._browser.Active();

                    //if the object is not visible, then move it.
                    ScrollIntoView(false);

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
                throw new CannotPerformActionException("Can not perform Hover action:" + ex.Message);
            }
        }


        /*  void HighLight()
         *  Highlight the object, we will see a red rect around the object.
         */
        public override void HighLight()
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
                throw new CannotHighlightObjectException("Can not highlight the object: " + ex.Message);
            }

        }

        #endregion

        #region private methods

        /* void ScrollIntoView(bool toTop)
         * if the object is out of page view, scroll it to make it is visible.
         * Like some very long page, we can not see everything, we need to move the scrollbar to see
         * the bottom object.
         */
        protected virtual void ScrollIntoView(bool toTop)
        {
            //first, get the current position.
            int right = this._rect.X + this._rect.Width;
            int buttom = this._rect.Y + this._rect.Height;

            int currentWidth = _browser.ClientLeft + _browser.ClientWidth;
            int currentHeight = _browser.ClientTop + _browser.ClientHeight;

            //if the object is not visible, then move the scrollbar.
            if (right > currentWidth || buttom > currentHeight)
            {
                this._sourceElement.scrollIntoView(toTop);

                Thread.Sleep(1000 * 1);

                //re-calculate the position, because we had move it.
                this.Rect = GetRectOnScreen();
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

                IntPtr handle = Win32API.WindowFromPoint(left + 1, top + 1);

                //if the control is not a windows standard control,we need to minus the browser top and left.
                //because if it is NOT a windows control, then we consider it is a HTML control, when we get the handle,
                //the handle is belonged to "Internet Explorer_Server", it not include the menu bar...
                //so we need to minus the menu bar height and top to get the actual position.

                if (!isWindowsControl)
                {
                    left -= _browser.ClientLeft;
                    top -= _browser.ClientTop;
                }

                IntPtr hDC = Win32API.GetWindowDC(handle);
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    using (Graphics g = Graphics.FromHdc(hDC))
                    {
                        g.DrawRectangle(pen, left, top, width, height);
                    }
                }
                Win32API.ReleaseDC(handle, hDC);

                // the red rect last for 1 seconds.
                Thread.Sleep(1000 * 1);

                //refresh the window
                Win32API.InvalidateRect(handle, IntPtr.Zero, 1);
                Win32API.UpdateWindow(handle);
                Win32API.RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, Win32API.RDW_FRAME | Win32API.RDW_INVALIDATE | Win32API.RDW_UPDATENOW | Win32API.RDW_ALLCHILDREN);

                _actionFinished.Set();
            }
            catch (Exception ex)
            {
                throw new CannotHighlightObjectException("Can not high light object: " + ex.Message);
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

        /* string GetLabelText()
         * return the text around this control.
         */
        protected virtual string GetLabelText()
        {
            return GetAroundText(this._sourceElement);
        }

        #endregion

        #endregion

    }
}
