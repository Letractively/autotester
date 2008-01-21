/********************************************************************
  *                      AutoTester     
  *                        Wan,Yu
  * AutoTester is a free software, you can use it in any commercial work. 
  * But you CAN NOT redistribute it and/or modify it.
  *--------------------------------------------------------------------
  * Component: HTMLTestTable.cs
  *
  * Description: This class defines the actions for <Table>.
  *              NEED UPDATE!!! 
  *
  * History: 2007/09/04 wan,yu Init version
  *
  *********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

using mshtml;

using Shrinerain.AutoTester.Core;

namespace Shrinerain.AutoTester.HTMLUtility
{
    public class HTMLTestTable : HTMLTestGUIObject, IClickable, IShowInfo, ISelectable, IMatrix
    {


        #region fields

        //HTML element for a table, we have table, row, col and cell.
        protected IHTMLTable _tableElement;
        protected IHTMLTableRow _tableRowElement;
        protected IHTMLTableCol _tableColElement;
        protected IHTMLTableCell _tableCellElement;

        protected int _rowCount;
        protected int _colCount;

        #endregion

        #region properties

        public int RowCount
        {
            get { return _rowCount; }
        }

        public int ColCount
        {
            get { return _colCount; }
        }

        #endregion

        #region methods

        #region ctor

        public HTMLTestTable(IHTMLElement element)
            : base(element)
        {
            this._type = HTMLTestObjectType.Table;

            try
            {
                _tableElement = (IHTMLTable)element;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not convert to ITHMLTable element: " + ex.Message);
            }

            try
            {
                _rowCount = _tableElement.rows.length;
                _colCount = _tableElement.cols;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get row count and/or col count: " + ex.Message);
            }
        }

        #endregion

        #region public methods

        public virtual int GetRowCount()
        {
            return GetRowCount(0);
        }

        public virtual int GetColCount()
        {
            return GetColCount(0);
        }

        #region IMatrix Members

        /* int GetRowCount(int col)
         * return the row count of expected column.
         * for HTML testing, just return the whole count.
         */
        public virtual int GetRowCount(int col)
        {
            return _tableElement.rows.length;
        }

        /* int GetColCount(int row)
         * return the column count of expected row.
         */
        public virtual int GetColCount(int row)
        {
            return _tableElement.cols;
        }

        /* HTMLTestGUIObject GetObjectByCell(int row, int col)
         * return the first test object at expected cell.
         */
        public virtual HTMLTestGUIObject GetObjectByCell(int row, int col)
        {
            return (HTMLTestGUIObject)GetObjectsByCell(row, col)[0];
        }

        public virtual object[] GetObjectsByCell(int row, int col)
        {
            // the index must in the range.
            if (row < 0 || col < 0 || row >= _rowCount || col >= _colCount)
            {
                return null;
            }

            try
            {
                IHTMLElementCollection tmpCol = (IHTMLElementCollection)_tableElement.rows.item((object)row, (object)col);

                HTMLTestGUIObject[] tmpObjects = null;

                if (tmpCol.length > 0)
                {
                    tmpObjects = new HTMLTestGUIObject[tmpCol.length];

                    object name;
                    object index;

                    IHTMLElement tmpElement;

                    for (int i = 0; i < tmpCol.length; i++)
                    {
                        name = (object)i;
                        index = (object)i;

                        try
                        {
                            tmpElement = (IHTMLElement)tmpCol.item(name, index);

                            tmpObjects[i] = _htmlObjPool.BuildObjectByType(tmpElement);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                return tmpObjects;

            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not get objects by cell[" + row.ToString() + "],[" + col.ToString() + "]: " + ex.Message);
            }

        }

        #endregion

        #region IClickable Members

        public virtual void Click()
        {
            throw new NotImplementedException();
        }

        public virtual void DoubleClick()
        {
            throw new NotImplementedException();
        }

        public virtual void RightClick()
        {
            throw new NotImplementedException();
        }

        public virtual void MiddleClick()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInteractive Members

        public virtual void Focus()
        {
            throw new NotImplementedException();
        }

        public virtual object GetDefaultAction()
        {
            throw new NotImplementedException();
        }

        public virtual void PerformDefaultAction(object parameter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IShowInfo Members

        public virtual string GetText()
        {
            throw new NotImplementedException();
        }

        public virtual string GetFontStyle()
        {
            throw new NotImplementedException();
        }

        public virtual string GetFontFamily()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISelectable Members

        public virtual void Select(string str)
        {
            throw new NotImplementedException();
        }

        public virtual void SelectByIndex(int index)
        {
            throw new NotImplementedException();
        }

        public virtual void SelectMulti(string[] values)
        {

        }

        public virtual string[] GetAllValues()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IContainer Members

        public virtual object[] GetChildren()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private methods


        #endregion

        #endregion

    }
}
