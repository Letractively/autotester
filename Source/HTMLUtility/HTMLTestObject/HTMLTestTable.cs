/********************************************************************
  *                      AutoTester     
  *                        Wan,Yu
  * AutoTester is a free software, you can use it in any commercial work. 
  * But you CAN NOT redistribute it and/or modify it.
  *--------------------------------------------------------------------
  * Component: HTMLTestTable.cs
  *
  * Description: This class defines the actions for <Table>.
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
    public class HTMLTestTable : HTMLTestGUIObject, IText, ITable
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
                throw new CannotBuildObjectException("Can not convert to ITHMLTable element: " + ex.ToString());
            }

            try
            {
                //get row count
                _rowCount = _tableElement.rows.length;
                //get col count of the first row.
                _tableRowElement = (IHTMLTableRow)_tableElement.rows.item((object)0, (object)0);
                _colCount = _tableRowElement.cells.length;
            }
            catch (Exception ex)
            {
                throw new CannotBuildObjectException("Can not get row count and/or col count: " + ex.ToString());
            }
        }

        #endregion

        #region public methods

        public virtual int GetRowCount()
        {
            return _rowCount;
        }

        public virtual int GetColCount()
        {
            return _colCount;
        }

        #region IMatrix Members

        /* int GetRowCount(int col)
         * return the row count of expected column.
         * for HTML testing, just return the whole count.
         */
        public virtual int GetRowCount(int col)
        {
            try
            {
                return _tableElement.rows.length;
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get row count of column: " + ex.ToString());
            }
        }

        /* int GetColCount(int row)
         * return the column count of expected row.
         */
        public virtual int GetColCount(int row)
        {
            try
            {
                _tableRowElement = (IHTMLTableRow)_tableElement.rows.item((object)row, (object)row);
                return _tableRowElement.cells.length;
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get column count of row:" + row.ToString() + ": " + ex.ToString());
            }
        }

        /* string GetTextByCell(int row, int col)
         * return the text in expected cell.
         */
        public virtual string GetTextByCell(int row, int col)
        {
            try
            {
                IHTMLElement element = GetElementByCell(row, col);
                string text;
                if (HTMLTestObject.TryGetProperty(element, "innerText", out text))
                {
                    return text;
                }
                else
                {
                    throw new PropertyNotFoundException("Can not get text of cell:[" + row.ToString() + "," + col.ToString() + "]");
                }
            }
            catch (TestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PropertyNotFoundException("Can not get text of cell:[" + row.ToString() + "," + col.ToString() + "]: " + ex.ToString());
            }
        }

        /* HTMLTestGUIObject GetObjectByCell(int row, int col)
         * return the first test object at expected cell.
         */
        public virtual HTMLTestGUIObject GetObjectByCell(int row, int col, HTMLTestObjectType objType)
        {
            foreach (Object i in GetObjectsByCell(row, col))
            {
                //check each object, return the object of the expceted type.
                if (i != null && i is HTMLTestGUIObject)
                {
                    if ((i as HTMLTestGUIObject).Type == objType)
                    {
                        return (HTMLTestGUIObject)i;
                    }
                }
            }

            throw new ObjectNotFoundException("Can not find " + objType.ToString() + " at [" + row.ToString() + "," + col.ToString() + "].");
        }

        /* object[] GetObjectsByCell(int row, int col)
         * return the children elements of an expected cell.
         */
        public virtual object[] GetObjectsByCell(int row, int col)
        {
            try
            {
                IHTMLElement cellChild = (IHTMLElement)GetElementByCell(row, col);
                IHTMLElementCollection cellChildren = (IHTMLElementCollection)cellChild.all;
                if (cellChildren.length > 0)
                {
                    HTMLTestGUIObject[] tmpObjects = new HTMLTestGUIObject[cellChildren.length];
                    IHTMLElement tmpElement;

                    for (int i = 0; i < cellChildren.length; i++)
                    {
                        object index = (object)i;
                        try
                        {
                            tmpElement = (IHTMLElement)cellChildren.item(index, index);
                            tmpObjects[i] = HTMLTestObjectFactory.BuildHTMLTestObject(tmpElement, this._browser, this._pool);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    return tmpObjects;
                }
                else
                {
                    throw new ObjectNotFoundException("Can not get objects by cell[" + row.ToString() + "],[" + col.ToString() + "].");
                }
            }
            catch (ObjectNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not get objects by cell[" + row.ToString() + "],[" + col.ToString() + "]: " + ex.ToString());
            }
        }

        #endregion

        #region IInteractive Members

        public virtual void Focus()
        {
            throw new NotImplementedException();
        }

        public virtual string GetAction()
        {
            throw new NotImplementedException();
        }

        public virtual void DoAction(object parameter)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IText Members

        public virtual string GetText()
        {
            return GetTextByCell(0, 0);
        }

        public override string GetLabel()
        {
            return null;
        }

        public virtual string GetFontFamily()
        {
            throw new NotImplementedException();
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

        #region IContainer Members

        public virtual object[] GetChildren()
        {
            throw new NotImplementedException();
        }

        public virtual Object GetChild(int childIndex)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region private methods

        /* IHTMLElement GetElementByCell(int row, int col)
         * return the element at expected cell.
         */
        protected virtual IHTMLElement GetElementByCell(int row, int col)
        {
            // the index must in the range.
            if (row < 0 || col < 0 || row >= _rowCount || col >= GetColCount(col))
            {
                throw new ObjectNotFoundException("Can not get element by cell[" + row.ToString() + "," + col.ToString() + "], index out of range.");
            }

            try
            {
                //get the expected row.
                _tableRowElement = (IHTMLTableRow)_tableElement.rows.item((object)row, (object)row);
                //return the expected cell.
                return (IHTMLElement)_tableRowElement.cells.item((object)col, (object)col);
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundException("Can not get element by cell[" + row.ToString() + "," + col.ToString() + "]: " + ex.ToString());
            }

        }

        #endregion

        #endregion
    }
}
