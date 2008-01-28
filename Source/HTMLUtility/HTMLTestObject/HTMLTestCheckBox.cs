/********************************************************************
*                      AutoTester     
*                        Wan,Yu
* AutoTester is a free software, you can use it in any commercial work. 
* But you CAN NOT redistribute it and/or modify it.
*--------------------------------------------------------------------
* Component: HTMLTestCheckBox.cs
*
* Description: This class defines the actions provide by CheckBox.
*              The important actions include "Check","UnCheck".
* 
* History: 2007/09/04 wan,yu Init version
*          2008/01/24 wan,yu update, add GetLabelForCheckBox(); 
*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;
using Shrinerain.AutoTester.Win32;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestCheckBox : HTMLTestGUIObject, ICheckable, IShowInfo
    {

        #region fields

        //the HTML element of HTMLTestCheckBox.
        protected IHTMLInputElement _checkBoxElement;

        #endregion

        #region properties


        #endregion

        #region methods

        #region ctor

        public HTMLTestCheckBox(IHTMLElement element)
            : base(element)
        {

            this._type = HTMLTestObjectType.CheckBox;

            try
            {
                this._checkBoxElement = (IHTMLInputElement)element;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get IHTMLInputElement: " + ex.Message);
            }

        }

        #endregion

        #region public methods

        #region ICheckable Members

        public void Check()
        {
            try
            {
                if (!IsChecked())
                {
                    Click();
                }
            }
            catch (CannotPerformActionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform Check action on checkbox: " + ex.Message);
            }

        }

        public void UnCheck()
        {
            try
            {
                if (IsChecked())
                {
                    Click();
                }
            }
            catch (CannotPerformActionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not perform UnCheck action on checkbox: " + ex.Message);
            }
        }

        public bool IsChecked()
        {
            try
            {
                return _checkBoxElement.@checked;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not get status of checkbox: " + ex.Message);
            }
        }

        #endregion

        #region IClickable Members

        public void Click()
        {
            try
            {
                _actionFinished.WaitOne();

                Hover();

                MouseOp.Click();

                _actionFinished.Set();
            }
            catch (CannotPerformActionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CannotPerformActionException("Can not click on the checkbox: " + ex.Message);
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
            return "Check";
        }

        public void PerformDefaultAction(object para)
        {
            Check();
        }

        #endregion

        #region IShowInfo Members

        public string GetText()
        {
            return LabelText;
        }

        public string GetFontStyle()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string GetFontFamily()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /* string GetLabelForCheckBox(IHTMLElement element)
         * return the text around check box, firstly we will try current cell, then try right cell and up cell.
         */
        public static string GetLabelForCheckBox(IHTMLElement element)
        {
            try
            {
                //firstly, try to get text in the same cell/span/div/label
                string label = GetAroundText(element);

                //for checkbox, we think the text on the right is it's label.
                if (!String.IsNullOrEmpty(label) && !String.IsNullOrEmpty(label.Split('\t')[1]))
                {
                    return label.Split('\t')[1];
                }

                //if failed, try to get text from left cell or up cell.

                IHTMLElement cellParentElement = element.parentElement;

                if (cellParentElement != null && cellParentElement.tagName == "TD")
                {
                    IHTMLTableCell parentCellElement = (IHTMLTableCell)cellParentElement;
                    int cellId = parentCellElement.cellIndex;

                    IHTMLTableRow parentRowElement = (IHTMLTableRow)cellParentElement.parentElement;
                    int rowId = parentRowElement.rowIndex;

                    object index = (object)(cellId + 1);

                    //get the right cell.
                    IHTMLElement nextCellElement = (IHTMLElement)parentRowElement.cells.item(index, index);

                    if (HTMLTestObject.TryGetValueByProperty(nextCellElement, "innerText", out label))
                    {
                        return label;
                    }

                    //not found, try the left cell.
                    index = (object)(cellId - 1);

                    IHTMLElement prevCellElement = (IHTMLElement)parentRowElement.cells.item(index, index);

                    if (HTMLTestObject.TryGetValueByProperty(nextCellElement, "innerText", out label))
                    {
                        return label;
                    }

                    //still not found, we will try the up cell.
                    //of coz, if we want to try up row, current row can NOT be the first row, that means the row id should >0
                    if (rowId > 0)
                    {
                        //                                                     cell              row           table
                        IHTMLTableSection tableSecElement = (IHTMLTableSection)cellParentElement.parentElement.parentElement;

                        index = (object)(rowId - 1);
                        IHTMLTableRow upRowElement = (IHTMLTableRow)tableSecElement.rows.item(index, index);

                        index = (object)cellId;
                        IHTMLElement upCellElement = (IHTMLElement)upRowElement.cells.item(index, index);

                        if (HTMLTestObject.TryGetValueByProperty(upCellElement, "innerText", out label))
                        {
                            return label;
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
        #endregion

        #endregion

        #region private methods

        /* string GetAroundText()
         * return the text around the check box.
         */
        protected override string GetLabelText()
        {
            return GetLabelForCheckBox(this._sourceElement);
        }

        #endregion

        #endregion

    }
}
